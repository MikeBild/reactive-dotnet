using System.Reactive.Linq;

namespace ReactiveSamples.FizzBuzz;

/// <summary>
/// Modernization of Rx_FizzBuzz (2010): custom Operator as an Extension Method on IObservable.
/// The if/else cascade becomes a switch expression over a tuple pattern.
/// </summary>
public static class FizzBuzzExtensions
{
    public static string ToFizzBuzz(this int number) => (number % 3, number % 5) switch
    {
        (0, 0) => "FizzBuzz",
        (0, _) => "Fizz",
        (_, 0) => "Buzz",
        _ => number.ToString(System.Globalization.CultureInfo.InvariantCulture),
    };

    /// <summary>A custom Operator is just a function IObservable&lt;A&gt; → IObservable&lt;B&gt; (RxJS: pipeable operator).</summary>
    public static IObservable<string> FizzBuzz(this IObservable<int> source) => source.Select(ToFizzBuzz);

    /// <summary>The same logic as IAsyncEnumerable (pull instead of push) – .NET 10 ships async LINQ in the BCL.</summary>
    public static IAsyncEnumerable<string> FizzBuzz(this IAsyncEnumerable<int> source) => source.Select(ToFizzBuzz);
}
