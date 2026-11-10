namespace Exercises.RxNetWhy;

/// <summary>Where do the values come from?</summary>
public enum Source { Ui, WebSocket, ServerSentEvents, Polling, Queue, Timer, Other }

/// <summary>What do you need once the values are there? The points from the checklist in the handout.</summary>
public enum Need { Combine, Cancel, Throttle, Buffer, Retry }

/// <summary>A feature in which values arrive over time.</summary>
public sealed record StreamFeature(string Name, Source Source, IReadOnlyList<Need> Needs, string Why);
