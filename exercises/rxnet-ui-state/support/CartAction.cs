namespace Exercises.RxNetUiState;

/// <summary>Jede Nutzeraktion im Warenkorb ist ein Wert.</summary>
public abstract record CartAction;

public sealed record AddItem(CartItem Item) : CartAction;

public sealed record RemoveItem(string ProductId) : CartAction;
