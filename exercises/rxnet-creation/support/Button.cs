namespace Exercises.RxNetCreation;

/// <summary>A button with a classic .NET event, as in WPF, WinForms, or MAUI.</summary>
public sealed class Button
{
    public event EventHandler? Clicked;

    /// <summary>How many handlers are currently registered. The tests use this to check unsubscribing.</summary>
    public int HandlerCount => Clicked?.GetInvocationList().Length ?? 0;

    public void Click() => Clicked?.Invoke(this, EventArgs.Empty);
}
