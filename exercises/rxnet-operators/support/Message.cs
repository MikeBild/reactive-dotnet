namespace Exercises.RxNetOperators;

/// <summary>A message from the server. Id identifies it; on repeats, the same Id arrives again.</summary>
public sealed record Message(string Type, string Id, object? Payload);

/// <summary>Payload of messages of type "order-status".</summary>
public sealed record OrderStatusChanged(string OrderId, OrderStatus Status);
