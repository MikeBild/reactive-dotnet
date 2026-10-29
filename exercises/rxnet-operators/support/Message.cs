namespace Exercises.RxNetOperators;

/// <summary>Eine Nachricht vom Server. Id identifiziert sie, bei Wiederholungen kommt dieselbe Id noch einmal.</summary>
public sealed record Message(string Type, string Id, object? Payload);

/// <summary>Payload der Nachrichten vom Typ "order-status".</summary>
public sealed record OrderStatusChanged(string OrderId, OrderStatus Status);
