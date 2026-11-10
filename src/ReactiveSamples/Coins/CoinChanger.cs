using System.Collections.Immutable;
using System.Reactive.Linq;

namespace ReactiveSamples.Coins;

/// <summary>
/// Modernization of the gists "Coin Changer with LINQ" and "... without shared state" (2013).
/// Original 1: <c>select new { ..., Spare = (amount %= n) }</c> – Side Effect in the query,
///   the result depends on evaluation order and multiple enumeration.
/// Original 2: Dictionary&lt;string,int&gt; as an accumulator ("Data hack").
/// Today: record as immutable state + Aggregate/Scan. Because IEnumerable and IObservable
/// are dual, the same algorithm also runs unchanged as a push stream.
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

    /// <summary>Pull: classic LINQ to Objects.</summary>
    public static IReadOnlyList<CoinCount> Change(int amount, IEnumerable<int>? coins = null) =>
        (coins ?? EuroCents)
            .OrderDescending()
            .Aggregate(ChangeState.Start(amount), (state, coin) => state.Take(coin))
            .Coins;

    /// <summary>Push: the same fold over IObservable. Scan emits every intermediate step (RxJS: scan).</summary>
    public static IObservable<ChangeState> ChangeSteps(int amount, IObservable<int> coinsDescending) =>
        coinsDescending.Scan(ChangeState.Start(amount), (state, coin) => state.Take(coin));

    /// <summary>Push with a final result: Aggregate (RxJS: reduce) emits exactly one value on completion.</summary>
    public static IObservable<IReadOnlyList<CoinCount>> ChangeAsObservable(int amount, IEnumerable<int>? coins = null) =>
        (coins ?? EuroCents)
            .OrderDescending()
            .ToObservable()
            .Aggregate(ChangeState.Start(amount), (state, coin) => state.Take(coin))
            .Select(state => (IReadOnlyList<CoinCount>)state.Coins);
}
