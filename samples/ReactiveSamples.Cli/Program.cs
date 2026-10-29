using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using ReactiveSamples.Backpressure;
using ReactiveSamples.BoundedCollection;
using ReactiveSamples.ChannelAffinity;
using ReactiveSamples.Coins;
using ReactiveSamples.CombineLatest;
using ReactiveSamples.Composite;
using ReactiveSamples.FizzBuzz;
using ReactiveSamples.MessageBus;
using ReactiveSamples.MessageProcessing;
using ReactiveSamples.ObserverBasics;
using ReactiveSamples.Saga;
using ReactiveSamples.SharedResource;

// Aufruf: dotnet run --project samples/ReactiveSamples.Cli -- <beispiel>
var demos = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
{
    ["observer"] = () =>
    {
        NameProducer.Cold().Subscribe(new ConsoleObserver<string>("a"));
        var hot = NameProducer.Cold().Publish();
        hot.Subscribe(new ConsoleObserver<string>("b"));
        hot.Subscribe(new ConsoleObserver<string>("c"));
        hot.Connect();
        return Task.CompletedTask;
    },
    ["fizzbuzz"] = async () =>
    {
        await Observable.Range(1, 100).FizzBuzz().Do(Console.WriteLine);
    },
    ["combinelatest"] = async () =>
    {
        var numbers = Observable.Interval(TimeSpan.FromMilliseconds(300)).Select(i => (int)i).Take(4);
        var texts = Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(i => $"text{i}").Take(3);
        await LatestPairs.Combine(numbers, texts).Do(p => Console.WriteLine(LatestPairs.Format(p))).DefaultIfEmpty();
    },
    ["messages"] = async () =>
    {
        MessagePair[] input =
        [
            new(new OrderMessage("Message1"), new InvoiceMessage("Message1")),
            new(new OrderMessage("Message2"), new InvoiceMessage("Message2")),
            new(new OrderMessage("Message2"), new InvoiceMessage("Message2")),
            new(new OrderMessage("Message3"), new InvoiceMessage("Message3")),
        ];
        await input.ToObservable().DistinctByEitherData().Do(Console.WriteLine);
    },
    ["sharedresource"] = async () =>
    {
        var log = new LoggingList();
        var stream = SharedResourceStream.Create(log, DefaultScheduler.Instance, ticks: 5);
        var first = stream.Subscribe(v => Console.WriteLine($"first: {v}"));
        var second = stream.Subscribe(v => Console.WriteLine($"second: {v}"));
        await Task.Delay(2500);
        Console.WriteLine("first unsubscribes");
        first.Dispose();
        await Task.Delay(1500);
        Console.WriteLine("second unsubscribes");
        second.Dispose();
    },
    ["channels"] = async () =>
    {
        var commands = Observable.Interval(TimeSpan.FromMilliseconds(100))
            .Take(12)
            .Select(i => new Command(new Channel((int)(i % 3)), (int)(i / 3), TimeSpan.FromMilliseconds(Random.Shared.Next(50, 400))));
        var processed = await ChannelProcessor.DrainAsync(
            commands,
            c => { Thread.Sleep(c.Work); Console.WriteLine($"{c.Channel} #{c.Sequence} on thread {Environment.CurrentManagedThreadId}"); },
            TimeSpan.FromSeconds(15));
        Console.WriteLine($"Drained {processed.Count} commands – last one closed the door.");
    },
    ["bounded"] = async () =>
    {
        var log = new BoundedObservableCollection<string>(3);
        using var _ = log.Changes.Subscribe(e => Console.WriteLine($"{e.Action}: {string.Join(",", log)}"));
        foreach (var line in new[] { "a", "b", "c", "d", "e" })
        {
            log.Add(line);
        }
        await Observable.Range(1, 5).LastN(3).Do(w => Console.WriteLine($"LastN: {string.Join(",", w)}"));
    },
    ["coins"] = async () =>
    {
        foreach (var c in CoinChanger.Change(99)) Console.WriteLine(c);
        await CoinChanger.ChangeSteps(99, CoinChanger.EuroCents.OrderDescending().ToObservable()).Do(s => Console.WriteLine($"remainder {s.Remainder}"));
    },
    ["bus"] = () =>
    {
        using var bus = new RxMessageBus();
        bus.Errors.Subscribe(f => Console.WriteLine($"handler failed: {f.Exception.Message}"));
        bus.Register<Greet>(g => Console.WriteLine($"Hello {g.Name}"));
        bus.Register<Greet>(g => throw new InvalidOperationException($"no greeting for {g.Name}"), predicate: g => g.Name == "Bob");
        bus.Send(new Greet("Alice"));
        bus.Send(new Greet("Bob"));
        bus.Send(new Greet("Carol")); // der Bus lebt noch
        return Task.CompletedTask;
    },
    ["saga"] = () =>
    {
        using var bus = new RxMessageBus();
        var (completed, wiring) = DocumentSaga.Wire(bus);
        using var _ = wiring;
        using var __ = completed.Subscribe(d => Console.WriteLine($"{d.ThankYouMessage} for filling document {d.DocumentId}: {d.Chapter1}/{d.Chapter2}"));
        bus.Send(ChaptersDocument.New("Thank you"));
        bus.Send(ChaptersDocument.New("Thanx"));
        return Task.CompletedTask;
    },
    ["composite"] = async () =>
    {
        static Func<Task<Stream>> Json(string s) => () => Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(s)));
        var posts = CompositeDataModel.FromJson<PostCreated>(Json("""[{"postId":1,"title":"first","body":"b","authorId":1},{"postId":2,"title":"second","body":"b","authorId":1}]"""));
        var comments = CompositeDataModel.FromJson<CommentAdded>(() => Task.FromResult<Stream>(File.OpenRead("does-not-exist.json"))); // fehlt → leer
        await CompositeDataModel.Compose(posts, comments).Do(v => Console.WriteLine($"{v.PostId} {v.Title}: {v.CommentCount} comments"));
    },
    ["backpressure"] = async () =>
    {
        var fast = Observable.Interval(TimeSpan.FromMilliseconds(10)).Take(200);
        Console.WriteLine("Sample(100ms):");
        await fast.LatestPer(TimeSpan.FromMilliseconds(100), DefaultScheduler.Instance).Do(v => Console.Write($"{v} "));
        Console.WriteLine();
        Console.WriteLine("Bounded channel (capacity 5, DropOldest), slow consumer:");
        await foreach (var v in fast.ToBoundedAsyncEnumerable(capacity: 5))
        {
            Console.Write($"{v} ");
            await Task.Delay(50);
        }
        Console.WriteLine();
    },
};

if (args.Length == 0 || !demos.TryGetValue(args[0], out var demo))
{
    Console.WriteLine("Beispiele: " + string.Join(", ", demos.Keys));
    return args.Length == 0 ? 0 : 1;
}

await demo();
return 0;

sealed record Greet(string Name) : ICommandMessage;

sealed class ConsoleObserver<T>(string name) : IObserver<T>
{
    public void OnNext(T value) => Console.WriteLine($"{name}: {value}");
    public void OnError(Exception error) => Console.WriteLine($"{name}: error {error.Message}");
    public void OnCompleted() => Console.WriteLine($"{name}: completed");
}

sealed class LoggingList : List<string>, IList<string>
{
    void ICollection<string>.Add(string item)
    {
        Console.WriteLine($"[{item}]");
        Add(item);
    }
}
