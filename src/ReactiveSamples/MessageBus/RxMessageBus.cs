using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveSamples.MessageBus;

/// <summary>
/// Modernisierung von ReactiveMVVM/Bus (Silverlight, 2011): In-Memory-Message-Bus auf Rx-Basis.
///
/// Zwei lehrreiche Fehler im Original:
/// 1. <c>catch (Exception e) { _subject.OnError(e); }</c> – ein einziger werfender Handler
///    beendete den Subject und damit den Bus für ALLE Abonnenten dauerhaft
///    (nach OnError ist ein Observable terminiert). Hier wird der Fehler isoliert und als
///    eigener Stream <see cref="Errors"/> gemeldet.
/// 2. <c>Register(action, scheduler)</c> nutzte SubscribeOn – das verlegt nur den Moment des
///    Abonnierens, nicht die Zustellung. Gemeint war ObserveOn (z. B. UI-Thread).
///
/// Ein Subject ist hier legitim: der Bus IST eine imperative Brücke ("Quelle ohne Observable").
/// Nach außen wird aber nur IObservable gezeigt, nie der Subject selbst.
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

    /// <summary>Typisierter Teilstrom – das eigentliche Rx-Interface des Busses.</summary>
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
            stream = stream.ObserveOn(observeOn); // nicht SubscribeOn!
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
