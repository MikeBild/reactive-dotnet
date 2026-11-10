using System.Globalization;

namespace Exercises.RxNetCombining;

public static class FizzBuzz
{
    /// <summary>Repeats the values endlessly: Cycle(["a", "b"]) yields a, b, a, b, …</summary>
    public static IObservable<T> Cycle<T>(params T[] values) =>
        // Synchronous and endless: If you subscribe to this alone, only Take gets you out again.
        values.ToObservable().Repeat();

    /// <summary>FizzBuzz from 1 to max as a stream of strings.</summary>
    public static IObservable<string> Create(int max = 100) =>
        // Zip pairs the nth value of both sources and ends as soon as the shorter source completes,
        // here Range. Then it unsubscribes from the endless cycle.
        // CombineLatest would be wrong: It pairs the latest state, not value by value.
        //
        // Zip is applied twice, pair by pair. The variant Observable.Zip(a, b, c, …) with three
        // sources at once does not end with two synchronous, endless cycles. Try it.
        Observable.Range(1, max)
            .Zip(Cycle("", "", "Fizz"), (number, fizz) => (number, fizz))
            .Zip(Cycle("", "", "", "", "Buzz"), (pair, buzz) => pair.fizz + buzz is { Length: > 0 } word
                ? word
                : pair.number.ToString(CultureInfo.InvariantCulture));
}
