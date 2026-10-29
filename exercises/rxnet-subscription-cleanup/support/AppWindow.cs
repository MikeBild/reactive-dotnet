namespace Exercises.RxNetSubscriptionCleanup;

/// <summary>Ein Fenster mit einem klassischen .NET-Event. Es lebt länger als jede Ansicht darin.</summary>
public sealed class AppWindow
{
    public event EventHandler? Resized;

    /// <summary>Wie viele Handler gerade registriert sind. Ein vergessener Handler ist ein Memory Leak.</summary>
    public int HandlerCount => Resized?.GetInvocationList().Length ?? 0;

    public void Resize() => Resized?.Invoke(this, EventArgs.Empty);
}
