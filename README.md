*English · [Deutsch](README.de.md)*

# Exercises: Reactive programming with Rx.NET

Exercise repo for the workshop “Reactive programming with Rx.NET”. Each lesson has a starting point, tests, and a reference solution. The examples come from the workshop shop (product search, order status, cart). On top of that come Mike Bild’s Rx.NET samples from 2010 to 2013, brought up to .NET 10 and System.Reactive 7.

## What you learn

- Create Observables, combine them, and process them with your own Operators
- Use Flattening, error handling, Multicasting, and Backpressure deliberately
- Test time-dependent code with TestScheduler and virtual time

## Prerequisites

- **.NET SDK 10** – the version is in [`global.json`](global.json).

## Setup

```bash
git clone https://github.com/MikeBild/reactive-dotnet.git
cd reactive-dotnet
dotnet build
dotnet test   # samples and all reference solutions, everything green (CI checks it the same way)
```

## How to work on an exercise

1. Open the file in the lesson folder, for example `exercises/rxnet-flattening/Flattening.cs`. The task is at the top, and `TODO` comments mark the gaps.
2. Run the lesson’s tests. The filter is the lesson’s namespace:

   ```bash
   dotnet test exercises/Exercises.Start.Tests --filter "FullyQualifiedName~RxNetFlattening"
   ```

3. Fill the gaps until all tests are green.
4. If you get stuck, compare with `exercises/<lesson>/solution/`.

**Red tests are intended at the start.** The starting points throw `NotImplementedException`, so the start tests fail until you have filled the gaps. A few tests are already green, such as `System_Reactive_ist_installiert_und_laeuft` (System.Reactive is installed and runs). In `rxnet-testing`, the test itself is the task: there the open tests are skipped until you remove `Skip`.

| Command | What happens |
| --- | --- |
| `dotnet test exercises/Exercises.Start.Tests` | All core lessons and “UI state as a stream of actions” against your starting points |
| `dotnet test exercises/Extras.Start.Tests` | The extra chapters on ReactiveUI and DynamicData against your starting points |

## Exercises

The order follows the workshop agenda. In all tests, time runs on the `TestScheduler`, so no test actually waits.

| Lesson | Folder | Filter | Task |
| --- | --- | --- | --- |
| When is reactive programming worth it? | [`exercises/rxnet-why`](exercises/rxnet-why) | `RxNetWhy` | Write down three features from your project in which values arrive over time, and check each one against the checklist from the handout. |
| Push instead of pull | [`exercises/rxnet-push-pull`](exercises/rxnet-push-pull) | `RxNetPushPull` | Write an iterator with yield return for the numbers one to ten and an Observable with the same values. Compare who triggers the next value in each case. |
| Nothing happens without Subscribe | [`exercises/rxnet-observable-contract`](exercises/rxnet-observable-contract) | `RxNetObservableContract` | Use Observable.Create to write a ticker that counts up on an IScheduler, signals OnCompleted after five values, and stops the timer on Dispose. |
| Almost everything becomes an Observable | [`exercises/rxnet-creation`](exercises/rxnet-creation) | `RxNetCreation` | Use creation methods to build one shared log stream from an event, a timer, and a task, in which every line carries its source as a prefix. |
| Operators are extension methods on IObservable | [`exercises/rxnet-operators`](exercises/rxnet-operators) | `RxNetOperators` | Process order status messages: filter by type, drop duplicates with DistinctUntilChanged, and count with Scan, as your own Extension Methods. |
| Every subscription needs an end | [`exercises/rxnet-subscription-cleanup`](exercises/rxnet-subscription-cleanup) | `RxNetSubscriptionCleanup` | Build a component with three subscriptions to Interval and an event, and end them all together through a CompositeDisposable and TakeUntil. |
| Which Operator combines your streams? | [`exercises/rxnet-combining`](exercises/rxnet-combining) | `RxNetCombining` | Solve FizzBuzz reactively by combining the numbers one to one hundred with two cyclic streams for Fizz and Buzz using Zip. |
| Cold or hot, who starts the producer? | [`exercises/rxnet-cold-hot`](exercises/rxnet-cold-hot) | `RxNetColdHot` | Subscribe twice to an Observable from Observable.Defer, count the requests with Do, and then share the execution with Publish and RefCount. |
| Tasks and Observables work together | [`exercises/rxnet-tasks-async`](exercises/rxnet-tasks-async) | `RxNetTasksAsync` | Wrap an async method with a CancellationToken using FromAsync, check in the test that Dispose triggers cancellation, and get the result back with await. |
| Switch, run in parallel, queue, or ignore? | [`exercises/rxnet-flattening`](exercises/rxnet-flattening) | `RxNetFlattening` | Build product search, autosave, upload queue, and login button, each with one Flattening strategy, and explain every choice in a code comment. |
| An error ends the stream | [`exercises/rxnet-error-handling`](exercises/rxnet-error-handling) | `RxNetErrorHandling` | Load three URLs with SelectMany, give each request a timeout, retry twice with backoff, and return an error value on failure instead of aborting. |
| Subjects share values, Publish shares work | [`exercises/rxnet-subjects-multicasting`](exercises/rxnet-subjects-multicasting) | `RxNetSubjectsMulticasting` | Share a simulated WebSocket connection between several subscribers so that it opens only once and closes on the last Dispose. |
| What if more events arrive than you can process? | [`exercises/rxnet-time-backpressure`](exercises/rxnet-time-backpressure) | `RxNetTimeBackpressure` | From a fast sensor stream, show a throttled live value with Sample and an average per time window with Buffer. |
| Test time without waiting | [`exercises/rxnet-testing`](exercises/rxnet-testing) | `RxNetTesting` | Write xUnit tests for the finished product search with TestScheduler and virtual time: Throttle, duplicates, cancellation, and the error case. |

### Extra chapters

| Lesson | Folder | Filter | Task |
| --- | --- | --- | --- |
| UI state as a stream of actions | [`exercises/rxnet-ui-state`](exercises/rxnet-ui-state) | `RxNetUiState` | Build a ViewModel with actions as a stream, manage the state with Scan through pure Reducers, and keep the last five searches as an LRU list. |
| What does ReactiveUI handle for your ViewModel? | [`exercises/rxnet-reactiveui`](exercises/rxnet-reactiveui) | `RxNetReactiveUi` | Build the cart ViewModel with ReactiveObject, WhenAnyValue, ObservableAsPropertyHelper, and a ReactiveCommand, and test it without a UI. |
| How do large lists stay reactive and fast? | [`exercises/rxnet-dynamicdata`](exercises/rxnet-dynamicdata) | `RxNetDynamicData` | Keep the products in a SourceCache, filter them live by search term, sort by price, and bind the result to a ReadOnlyObservableCollection. |

“UI state as a stream of actions” needs no additional packages and therefore runs with the core lessons. ReactiveUI and DynamicData live in separate projects; the reasoning is under [Packages](#packages).

## Structure of the exercises

```text
exercises/
├── Shop/Shop.cs                  Workshop shop: Product, CartItem, Customer, OrderStatus
├── rxnet-flattening/
│   ├── Flattening.cs             Starting point with task and TODOs
│   ├── FlatteningTests.cs        Tests, the same for starting point and reference solution
│   └── solution/Flattening.cs    Reference solution with reasoning in the comment
├── Exercises.Start(.Tests)/      Projects for the core lessons against your starting points
├── Exercises.Solution(.Tests)/   the same tests against the reference solutions
└── Extras.*/                     the same for ReactiveUI and DynamicData
```

The project folders contain only a `.csproj`. Which files they include is defined in `exercises/Directory.Build.targets`. Some lessons have helper files under `support/` that you do not need to change, such as `FakeSocketServer.cs` or the finished product search in `rxnet-testing`.

## Packages

- `System.Reactive` 7.0.0 in the samples library and in the exercises
- `Microsoft.Reactive.Testing` 7.0.0 in the tests, for `TestScheduler` and virtual time, plus xUnit 2.9.3
- `ReactiveUI.Reactive` 26.0.1, only in `Extras.*`: The lesson shows MVVM on Rx with `ReactiveObject`, `WhenAnyValue`, `ObservableAsPropertyHelper`, and `ReactiveCommand`. Since version 26, the `ReactiveUI` package no longer depends on System.Reactive. The variant for System.Reactive is called `ReactiveUI.Reactive`, and its types live in `ReactiveUI.Reactive` and `ReactiveUI.Binding.Reactive`.
- `DynamicData` 9.5.0, only in `Extras.*`: The lesson shows reactive lists with `SourceCache`, `Filter`, and `SortAndBind`.
- Everything else comes from the BCL (Base Class Library), including `System.Threading.Channels` and async LINQ for `IAsyncEnumerable`.

ReactiveUI and DynamicData live in separate projects because `ReactiveUI.Reactive` brings a Source Generator that generates a second Extension Method `Switch`. There, `.Switch()` from System.Reactive is ambiguous, but the core lessons need it.

## Pitfalls the exercises show

- `Publish().RefCount()` does not restart after `OnCompleted` the way `share()` does in RxJS. Late subscribers only get `OnCompleted` (`rxnet-cold-hot`).
- `Observable.Zip(a, b, c, …)` with three sources does not end with two synchronous, endless cycles. Pairwise with `a.Zip(b, …).Zip(c, …)` it works (`rxnet-combining`).
- `StartWith([])` binds to `params T[]` and yields no initial value at all. For an empty list as the initial value, write `StartWith((IReadOnlyList<CartItem>)[])` (`rxnet-ui-state`).
- Rx.NET has no `exhaustMap`. The lesson `rxnet-flattening` builds it as an Extension Method.

## Rx.NET samples 2010–2013

Each sample is a small class in `src/ReactiveSamples/<Sample>/`. There are xUnit tests in `tests/ReactiveSamples.Tests/` and a runnable demo in `samples/ReactiveSamples.Cli`. The comments in the code describe what the original did, what is problematic about it today, and what has changed.

```bash
# List all demos
dotnet run --project samples/ReactiveSamples.Cli

# Run one demo
dotnet run --project samples/ReactiveSamples.Cli -- sharedresource
```

### Overview: old → new

| Sample (folder, demo) | Source (year) | What was modernized |
|---|---|---|
| **ObserverBasics** (`observer`) | Gists [Rx_Observer_Observables](https://gist.github.com/MikeBild/568281), [variant](https://gist.github.com/MikeBild/4368ce550a6cff6f62fb) (2010/11) | The hand-written `Producer` returned `this` from `Subscribe`. As a result, one `Dispose` removed all Observers, and `OnCompleted` was never sent. Now `Observable.Create` takes care of that. `OnError` printed “finished.”, which is now fixed. `results.Run` from Ix no longer exists. Tests cover cold and hot (`Publish`/`Connect`). |
| **FizzBuzz** (`fizzbuzz`) | Gist [Rx_FizzBuzz](https://gist.github.com/MikeBild/570401) (2010) | The if/else cascade is now a switch expression over a tuple. The custom Operator is an Extension Method, and the same logic also exists for `IAsyncEnumerable`. `await observable.ToArray()` replaces `Console.ReadLine`. |
| **CombineLatest** (`combinelatest`) | Gist [Rx_Subject_Example](https://gist.github.com/MikeBild/9c1b5cfb4f00915f7299) | The Operator is a Pure Function over two `IObservable`. In the test, `CreateHotObservable` replaces the two Subjects. ValueTuple instead of `Tuple<int,string>`. |
| **VirtualTimeTests** (tests only) | Gist [SimpleRxTesting](https://gist.github.com/MikeBild/3947466) (2012) | `CollectionAssert.AreEqual` became xUnit `Assert.Equal`. There are `ReactiveTest.OnNext(ticks, value)` assertions with exact points in time, the trap with `Start()` and tick 1000 is documented, and `FirstAsync().ToTask()` runs in virtual time. |
| **MessageProcessing** (`messages`) | Gist [Rx_MessageProcessing](https://gist.github.com/MikeBild/0be350cdf6f53959e950) (2012) | `Tuple` and two almost identical classes are now `record`s. The reflection comparer with `object ==` and an inconsistent `GetHashCode` is replaced by a type-safe comparer. A characterization test pins down the original rule (“first OR second message equal”). |
| **SharedResource** (`sharedresource`) | [RxSamples/LastOneMakesLightOff](https://github.com/MikeBild/RxSamples/tree/master/LastOneMakesLightOff) (2013), identical to gist [thelastoneturnslightoff](https://gist.github.com/MikeBild/4055567) | Stays with `Observable.Using` + `Publish().RefCount()`. Instead of `Debug.WriteLine` and `Console.ReadLine`, there are testable logs and a `TestScheduler`. The tests check one resource for all subscribers, release on the last `Dispose`, and a new resource when you subscribe again. |
| **ChannelAffinity** (`channels`) | Gist [last one close the door](https://gist.github.com/MikeBild/f58cd8f9c085a2899fa0) (2013) | `GroupBy` with one `EventLoopScheduler` per channel stays. New: the scheduler is disposed when the group ends; in the original, the threads leaked. The busy wait on a non-thread-safe counter and `Environment.Exit` in `Dispose` are replaced by `ToList().Timeout().ToTask()`, that is, completion with `await` and a timeout. `Command` and `Channel` are `record`s. |
| **BoundedCollection** (`bounded`) | Gist [LRUObservableCollection](https://gist.github.com/MikeBild/4066272) (2012) | The honest name is FIFO buffer, not LRU. Bug in the original: `Insert(0, x)` on a full buffer removed the element just inserted. The insertion order is now also tracked for `Remove` and the indexer. `FromEventPattern` turns `CollectionChanged` into an Observable. There are two Rx alternatives without mutation: `Scan` as a sliding window and `Buffer(n, 1)`. |
| **Coins** (`coins`) | Gists [Coin Changer with LINQ](https://gist.github.com/MikeBild/6789264), [without shared state](https://gist.github.com/MikeBild/6791315) (2013) | `amount %= n` in the query was a Side Effect, and the `Dictionary` served as an accumulator. Now the state is an immutable `record`, computed with `Aggregate`/`Scan`. The same fold runs over `IEnumerable` and `IObservable` (duality). |
| **MessageBus** (`bus`) | [ReactiveMVVM/Bus](https://github.com/MikeBild/ReactiveMVVM/tree/master/source/ReactiveMVVM/Bus) (2011) | Two bugs are fixed. First, `catch → _subject.OnError(e)` ended the bus for all subscribers. Errors are now isolated and run as a separate `Errors` stream. Second, `SubscribeOn` did not move the delivery; `ObserveOn` was meant. Tests cover both cases. In addition, there are marker interfaces and `record` messages. |
| **Saga** (`saga`) | Gist [ZeroMqDocumentSaga](https://gist.github.com/MikeBild/1839871) (2011) | Routing Slip in-process on the `RxMessageBus` instead of ZeroMQ and Minimod. Instead of mutating the message, `record` + `with` creates a new one. The bookmark is an `enum` instead of a string. |
| **Composite** (`composite`) | [RxSamples/composite_data_model](https://github.com/MikeBild/RxSamples/tree/master/composite_data_model) (2013, originally RxJS) | Port to C#: `Merge` → `GroupBy` → `Aggregate` per group, `takeLast(1)` in the original. If a source fails, `Catch(Empty)` delivers an empty stream. The events are typed (`record`s, pattern matching), and the read model is immutable. |
| **Backpressure** (`backpressure`) | *new* | No original. Added because Backpressure is a topic in the RxJS workshop. Lossy with `Sample`/`Buffer`, lossless via pull with a bounded `Channel<T>` → `IAsyncEnumerable`. If the consumer stops early, the waiting producer is released. |

### What is solved differently today

- **ReactiveMVVM (Silverlight, Autofac, `ViewModelField<T>`, `NotifyObject`, custom commands):** Only the bus is ported. Silverlight no longer exists, a service locator over a static Autofac container counts as an anti-pattern, and ready-made libraries cover the remaining parts today:
  - [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) uses a Source Generator to create `[ObservableProperty]` (instead of `ViewModelField<T>`/`NotifyObject`) and `[RelayCommand]` (instead of `DelegateCommand`/`SendCommand`), and with `WeakReferenceMessenger` it brings its own messenger (instead of `RxMessageBus`).
  - [ReactiveUI](https://www.reactiveui.net/) is the direct successor of the idea “MVVM on Rx”. It includes `WhenAnyValue` (property changes as `IObservable`), `ReactiveCommand` (commands with `CanExecute` as an Observable, built-in error and busy streams), and `MessageBus`.
  - DI today runs through `Microsoft.Extensions.DependencyInjection` with constructor injection instead of `SilverlightAppHost.Container.Resolve<T>()`.
  - Rx 7 moves UI integration into separate packages: `System.Reactive.Wpf`, `System.Reactive.WindowsForms`, `System.Reactive.Uwp` (see the [Rx 7 release notes](https://github.com/dotnet/reactive/blob/main/Rx.NET/Documentation/ReleaseHistory/Rx.v7.md)). So for `ObserveOnDispatcher`, you need one of these packages.
- **ZeroMQ saga:** The Minimod packages are orphaned. Today a message broker (RabbitMQ, NATS, Azure Service Bus, Kafka) handles this together with a saga or workflow framework (MassTransit, NServiceBus, Wolverine, Temporal). That includes persistence, retries, outbox, and idempotency, which an in-memory solution does not offer. The sample now only shows the pattern.
- **Use Subjects sparingly:** In the originals, `Subject`s were everywhere (command stream, samples, bus). Today the rule is: use a Subject only where the imperative world and Rx meet, such as in the bus, and expose only `AsObservable()`. For tests, use `CreateHotObservable`; for events, `FromEventPattern`; for async, `FromAsync`.
- **`Console.ReadLine`/`Thread.Sleep` as synchronization:** Replaced by `await` on Observables (`ToTask`, `FirstAsync`, `ToArray`) and, in tests, by virtual time.
- **Real channels instead of Rx:** For pure producer/consumer queues with Backpressure, `System.Threading.Channels` is often the better choice today. Rx remains strong at composing events over time.

### Relation to the RxJS concepts

| Concept | RxJS | Rx.NET | Sample |
|---|---|---|---|
| Observable / Observer | `new Observable(sub => …)`, `subscribe({next, error, complete})` | `Observable.Create`, `IObserver<T>` (`OnNext`, `OnError`, `OnCompleted`), `Subscribe` returns `IDisposable` | ObserverBasics |
| Creating | `of`, `from`, `interval`, `fromEvent`, `defer` | `Return`, `ToObservable`, `Interval`, `FromEventPattern`, `Defer`, `FromAsync` | ObserverBasics, BoundedCollection, Composite |
| Transforming | `map`, `filter`, `scan`, `reduce` | `Select`, `Where`, `Scan`, `Aggregate` (LINQ names) | FizzBuzz, Coins |
| Higher order | `mergeMap`, `groupBy`, `concatMap`, `switchMap` | `SelectMany`, `GroupBy`, `Select(...).Concat()`, `Select(...).Switch()` | ChannelAffinity, Composite |
| Combining | `combineLatest`, `merge`, `zip` | `CombineLatest`, `Merge`, `Zip` | CombineLatest, Composite |
| Duplicates | `distinctUntilChanged`, `distinctUntilKeyChanged` | `DistinctUntilChanged(keySelector / comparer)` | MessageProcessing |
| Errors | `catchError`, `retry` | `Catch`, `Retry`, `OnErrorResumeNext` | Composite, MessageBus |
| Resources | `finalize`, `using` | `Finally`, `Observable.Using` | SharedResource, ChannelAffinity |
| Hot / Cold | `share()`, `shareReplay()`, `connectable()` | `Publish().RefCount()`, `Replay(1).RefCount()`, `Publish()` + `Connect()` | SharedResource, FizzBuzz, ObserverBasics |
| Subjects | `Subject`, `BehaviorSubject`, `ReplaySubject` | same names, use sparingly | MessageBus |
| Schedulers | `asyncScheduler`, `queueScheduler`, `observeOn`, `subscribeOn` | `TaskPoolScheduler`, `EventLoopScheduler` (own thread), `CurrentThreadScheduler`, `ObserveOn`, `SubscribeOn` | ChannelAffinity, MessageBus, Backpressure |
| Time | `debounceTime`, `throttleTime`, `sampleTime`, `bufferTime` | **`Throttle` = `debounceTime`**, `Sample`, `Buffer(TimeSpan)` | Backpressure |
| Testing with virtual time | `TestScheduler`, marble tests `cold('-a-b|')` | `TestScheduler`, `CreateHot/ColdObservable`, `ReactiveTest.OnNext(ticks, v)` | VirtualTimeTests, CombineLatest, SharedResource, Backpressure |
| Promise/async interop | `firstValueFrom`, `lastValueFrom`, `from(promise)` | `FirstAsync().ToTask()`, `await observable` (last value), `FromAsync` | VirtualTimeTests, Composite |
| Backpressure | does not exist: push only; lossy via `sample`/`throttle`/`buffer` | the same; lossless via pull with `IAsyncEnumerable` + `Channel<T>` | Backpressure |

Pitfalls when you switch:

- `Throttle` in Rx.NET is `debounceTime` in RxJS. There is no real `throttleTime` built in; `Sample` comes closest.
- `await` on an `IObservable` returns the **last** value, not the first.
- `TestScheduler.Start(create)` subscribes at tick 200 and disposes at tick 1000. One tick is 100 ns, so Operators based on seconds deliver nothing there.
- `SubscribeOn` only moves subscribing. For delivery on a specific thread (UI, event loop), you need `ObserveOn`.

### Deliberately left out

- Gists without an Rx connection: Composite MVC4, MockHttpContext, TreeExtentions, ExpressionTranslator, DelegateAdjuster, DisplayNameByResourceKeyAttribute, CretaeManualleExpression, Code Kata PickAkin (plain LINQ), ECB_with_events (event wiring without Rx).
- From `RxSamples/composite_data_model`, only `index.html` is ported. `index2.html`/`index3.html` are not ported. They wrap the same pipeline in a fluent `Projection` wrapper with `andUrl`/`nextUrl` and also include `authors.json`. The port already shows the principle.
- Gist [ViewModelField](https://gist.github.com/MikeBild/1031650) and the rest of ReactiveMVVM (navigation, XAML triggers, commands): this is specific to Silverlight and covered today by CommunityToolkit.Mvvm or ReactiveUI (see above). A WPF port would also not build cross-platform.

## Workshop and slides

- Workshop: [“Reactive programming with Rx.NET”](https://workshops.mikebild.dev/w/reactive-programming-dotnet/remote/)
- Slides: [slides.mikebild.dev/introduction-reactive-programming.html](https://slides.mikebild.dev/introduction-reactive-programming.html)

## License

[MIT](LICENSE)
