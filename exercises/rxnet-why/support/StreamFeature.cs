namespace Exercises.RxNetWhy;

/// <summary>Woher kommen die Werte?</summary>
public enum Source { Ui, WebSocket, ServerSentEvents, Polling, Queue, Timer, Other }

/// <summary>Was brauchst du, wenn die Werte da sind? Die Punkte aus der Checkliste im Handout.</summary>
public enum Need { Combine, Cancel, Throttle, Buffer, Retry }

/// <summary>Ein Feature, bei dem Werte über die Zeit eintreffen.</summary>
public sealed record StreamFeature(string Name, Source Source, IReadOnlyList<Need> Needs, string Why);
