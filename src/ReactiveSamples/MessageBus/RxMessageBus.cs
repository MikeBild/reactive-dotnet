using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveSamples.MessageBus;

/// <summary>
/// Modernization of ReactiveMVVM/Bus (Silverlight, 2011): in-memory message bus based on Rx.
///
/// Two instructive bugs in the original:
/// 1. <c>catch (Exception e) { _subject.OnError(e); }</c> – a single throwing handler
///    terminated the Subject and with it the bus for ALL subscribers permanently
///    (after OnError, an Observable is terminated). Here the error is isolated and reported
///    as a separate stream <see cref="Errors"/>.
/// 2. <c>Register(action, scheduler)</c> used SubscribeOn – that only moves the moment of
///    subscribing, not the delivery. ObserveOn was meant (e.g. UI thread).
///
/// A Subject is legitimate here: the bus IS an imperative bridge ("source without an Observable").
/// But only IObservable is exposed, never the Subject itself.
/// </summary>
public interface IMessage;
public interface ICommandMessage : IMessage;
public interface IEventMessage : IMessage;

public sealed record HandlerFailure(IMessage Message, Exception Exception);

public sealed class RxMessageBus : IDisposable
{
    private readonly Subject<IMessage> _messages = new();
    private readonly Subject<HandlerFailure> _errors = new();

    public IObservable<HandlerFailure> Errors => _errors.AsObservable();

    public void Send(ICommandMessage command) => _messages.OnNext(command);

    public void Publish(IEventMessage @event) => _messages.OnNext(@event);

    /// <summary>Typed substream – the actual Rx interface of the bus.</summary>
    public IObservable<T> Messages<T>() where T : IMessage => _messages.OfType<T>();

    public IDisposable Register<T>(Action<T> handler, Func<T, bool>? predicate = null, IScheduler? observeOn = null)
        where T : IMessage
    {
        var stream = Messages<T>();
        if (predicate is not null)
        {
            stream = stream.Where(predicate);
        }
        if (observeOn is not null)
        {
            stream = stream.ObserveOn(observeOn); // not SubscribeOn
        }
        return stream.Subscribe(message =>
        {
            try
            {
                handler(message);
            }
            catch (Exception ex)
            {
                _errors.OnNext(new HandlerFailure(message, ex));
            }
        });
    }

    public void Dispose()
    {
        _messages.OnCompleted();
        _errors.OnCompleted();
        _messages.Dispose();
        _errors.Dispose();
    }
}
