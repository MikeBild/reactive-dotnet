namespace Exercises.RxNetSubscriptionCleanup;

/// <summary>A window with a classic .NET event. It lives longer than any view inside it.</summary>
public sealed class AppWindow
{
    public event EventHandler? Resized;

    /// <summary>How many handlers are currently registered. A forgotten handler is a memory leak.</summary>
    public int HandlerCount => Resized?.GetInvocationList().Length ?? 0;

    public void Resize() => Resized?.Invoke(this, EventArgs.Empty);
}
