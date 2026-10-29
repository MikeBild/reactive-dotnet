namespace Exercises.RxNetCreation;

/// <summary>Ein Button mit einem klassischen .NET-Event, wie in WPF, WinForms oder MAUI.</summary>
public sealed class Button
{
    public event EventHandler? Clicked;

    /// <summary>Wie viele Handler gerade registriert sind. Damit prüfen die Tests das Abmelden.</summary>
    public int HandlerCount => Clicked?.GetInvocationList().Length ?? 0;

    public void Click() => Clicked?.Invoke(this, EventArgs.Empty);
}
