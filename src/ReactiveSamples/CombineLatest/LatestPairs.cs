using System.Reactive.Linq;

namespace ReactiveSamples.CombineLatest;

/// <summary>
/// Modernisierung von Rx_Subject_Example: CombineLatest zweier Quellen.
/// Früher wurden zwei Subjects von Hand befeuert. Heute ist der Operator eine Pure Function
/// über zwei IObservable – woher die Werte kommen (Event, Timer, TestScheduler), ist egal.
/// Ergebnis als ValueTuple statt Tuple&lt;int,string&gt;.
/// </summary>
public static class LatestPairs
{
    public static IObservable<(int Number, string Text)> Combine(IObservable<int> numbers, IObservable<string> texts) =>
        numbers.CombineLatest(texts, (number, text) => (number, text));

    public static string Format((int Number, string Text) pair) => $"{pair.Number},{pair.Text}";
}
