namespace Exercises.RxNetSubjectsMulticasting;

/// <summary>
/// A simulated WebSocket connection. It counts how often it is opened and closed,
/// and distributes messages to all open connections.
/// </summary>
public sealed class FakeSocketServer
{
    private readonly List<Action<string>> _listeners = [];

    public int Opened { get; private set; }
    public int Closed { get; private set; }
    public int OpenConnections => Opened - Closed;

    public Connection Connect()
    {
        Opened++;
        return new Connection(this);
    }

    public void Broadcast(string message)
    {
        foreach (var listener in _listeners.ToArray()) listener(message);
    }

    public sealed class Connection(FakeSocketServer server)
    {
        private Action<string>? _listener;

        public void OnMessage(Action<string> listener)
        {
            _listener = listener;
            server._listeners.Add(listener);
        }

        public void Close()
        {
            server.Closed++;
            if (_listener is not null) server._listeners.Remove(_listener);
        }
    }
}
