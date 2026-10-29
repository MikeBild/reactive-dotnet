using System.Collections.Immutable;
using System.Reactive.Linq;

namespace ReactiveSamples.Coins;

/// <summary>
/// Modernisierung der Gists "Coin Changer with LINQ" und "... without shared state" (2013).
/// Original 1: <c>select new { ..., Spare = (amount %= n) }</c> – Seiteneffekt in der Query,
///   Ergebnis hängt von Auswertungsreihenfolge und Mehrfach-Enumeration ab.
/// Original 2: Dictionary&lt;string,int&gt; als Akkumulator ("Data hack").
/// Heute: record als unveränderlicher Zustand + Aggregate/Scan. Weil IEnumerable und IObservable
/// dual sind, läuft derselbe Algorithmus ohne Änderung auch als Push-Stream.
/// </summary>
public sealed record CoinCount(int Coin, int Count);

public sealed record ChangeState(int Remainder, ImmutableList<CoinCount> Coins)
{
    public static ChangeState Start(int amount) => new(amount, []);

    public ChangeState Take(int coin) => (Remainder / coin) switch
    {
        0 => this,
        var count => new(Remainder % coin, Coins.Add(new CoinCount(coin, count))),
    };
}

public static class CoinChanger
{
    public static readonly IReadOnlyList<int> EuroCents = [1, 2, 5, 10, 20, 50, 100];

    /// <summary>Pull: klassisches LINQ to Objects.</summary>
    public static IReadOnlyList<CoinCount> Change(int amount, IEnumerable<int>? coins = null) =>
        (coins ?? EuroCents)
            .OrderDescending()
            .Aggregate(ChangeState.Start(amount), (state, coin) => state.Take(coin))
            .Coins;

    /// <summary>Push: derselbe Fold über IObservable. Scan emittiert jeden Zwischenschritt (RxJS: scan).</summary>
    public static IObservable<ChangeState> ChangeSteps(int amount, IObservable<int> coinsDescending) =>
        coinsDescending.Scan(ChangeState.Start(amount), (state, coin) => state.Take(coin));

    /// <summary>Push mit Endergebnis: Aggregate (RxJS: reduce) liefert genau einen Wert beim Abschluss.</summary>
    public static IObservable<IReadOnlyList<CoinCount>> ChangeAsObservable(int amount, IEnumerable<int>? coins = null) =>
        (coins ?? EuroCents)
            .OrderDescending()
            .ToObservable()
            .Aggregate(ChangeState.Start(amount), (state, coin) => state.Take(coin))
            .Select(state => (IReadOnlyList<CoinCount>)state.Coins);
}
