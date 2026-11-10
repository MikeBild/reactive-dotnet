namespace Exercises.RxNetUiState;

/// <summary>Every user action in the cart is a value.</summary>
public abstract record CartAction;

public sealed record AddItem(CartItem Item) : CartAction;

public sealed record RemoveItem(string ProductId) : CartAction;
