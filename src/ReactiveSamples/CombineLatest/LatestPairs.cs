using System.Reactive.Linq;

namespace ReactiveSamples.CombineLatest;

/// <summary>
/// Modernization of Rx_Subject_Example: CombineLatest of two sources.
/// Earlier, two Subjects were fed by hand. Today the Operator is a Pure Function
/// over two IObservable – where the values come from (event, timer, TestScheduler) does not matter.
/// Result as a ValueTuple instead of Tuple&lt;int,string&gt;.
/// </summary>
public static class LatestPairs
{
    public static IObservable<(int Number, string Text)> Combine(IObservable<int> numbers, IObservable<string> texts) =>
        numbers.CombineLatest(texts, (number, text) => (number, text));

    public static string Format((int Number, string Text) pair) => $"{pair.Number},{pair.Text}";
}
