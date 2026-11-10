namespace Exercises.RxNetErrorHandling;

/// <summary>Result of a request as a value: success or failure, never an aborted stream.</summary>
public abstract record LoadResult<T>(string Url);

public sealed record Loaded<T>(string Url, T Value) : LoadResult<T>(Url);

public sealed record Failed<T>(string Url, string Message) : LoadResult<T>(Url);
