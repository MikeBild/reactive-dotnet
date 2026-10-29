namespace Exercises.RxNetErrorHandling;

/// <summary>Ergebnis eines Requests als Wert: Erfolg oder Fehler, nie ein abgebrochener Stream.</summary>
public abstract record LoadResult<T>(string Url);

public sealed record Loaded<T>(string Url, T Value) : LoadResult<T>(Url);

public sealed record Failed<T>(string Url, string Message) : LoadResult<T>(Url);
