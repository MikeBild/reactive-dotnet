using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using ReactiveSamples.MessageBus;

namespace ReactiveSamples.Tests;

public class MessageBusTests
{
    private sealed record AddTodo(string Title) : ICommandMessage;
    private sealed record TodoAdded(string Title) : IEventMessage;

    [Fact]
    public void Routes_by_message_type()
    {
        using var bus = new RxMessageBus();
        var commands = new List<AddTodo>();
        var events = new List<TodoAdded>();
        bus.Register<AddTodo>(commands.Add);
        bus.Register<TodoAdded>(events.Add);

        bus.Send(new AddTodo("Rx lernen"));
        bus.Publish(new TodoAdded("Rx lernen"));

        Assert.Equal([new AddTodo("Rx lernen")], commands);
        Assert.Equal([new TodoAdded("Rx lernen")], events);
    }

    [Fact]
    public void Predicate_filters_messages()
    {
        using var bus = new RxMessageBus();
        var urgent = new List<AddTodo>();
        bus.Register<AddTodo>(urgent.Add, predicate: m => m.Title.StartsWith('!'));

        bus.Send(new AddTodo("später"));
        bus.Send(new AddTodo("!sofort"));

        Assert.Equal([new AddTodo("!sofort")], urgent);
    }

    [Fact]
    public void A_throwing_handler_does_not_kill_the_bus()
    {
        // Original: catch → _subject.OnError(e) → Bus für alle tot.
        using var bus = new RxMessageBus();
        var failures = new List<HandlerFailure>();
        var received = new List<AddTodo>();
        bus.Errors.Subscribe(failures.Add);
        bus.Register<AddTodo>(m => throw new InvalidOperationException(m.Title));
        bus.Register<AddTodo>(received.Add);

        bus.Send(new AddTodo("eins"));
        bus.Send(new AddTodo("zwei"));

        Assert.Equal(2, received.Count);
        Assert.Equal(2, failures.Count);
        Assert.Equal("eins", failures[0].Exception.Message);
    }

    [Fact]
    public void ObserveOn_moves_delivery_to_the_scheduler()
    {
        using var bus = new RxMessageBus();
        var scheduler = new TestScheduler(); // steht hier für den UI-Dispatcher
        var received = new List<AddTodo>();
        bus.Register<AddTodo>(received.Add, observeOn: scheduler);

        bus.Send(new AddTodo("x"));
        Assert.Empty(received);   // noch nicht zugestellt …
        scheduler.Start();
        Assert.Single(received);  // … erst, wenn der Scheduler dran ist
    }

    [Fact]
    public void SubscribeOn_only_moves_the_subscription_which_was_the_bug_in_2011()
    {
        using var bus = new RxMessageBus();
        var scheduler = new TestScheduler();
        var received = new List<AddTodo>();
        bus.Messages<AddTodo>().SubscribeOn(scheduler).Subscribe(received.Add);

        bus.Send(new AddTodo("verloren")); // Abo existiert noch gar nicht → Nachricht geht verloren
        scheduler.Start();
        bus.Send(new AddTodo("synchron"));  // danach: Zustellung direkt auf dem Aufrufer-Thread

        Assert.Equal([new AddTodo("synchron")], received);
    }
}
