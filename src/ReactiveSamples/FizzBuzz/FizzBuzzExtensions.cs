using System.Reactive.Linq;

namespace ReactiveSamples.FizzBuzz;

/// <summary>
/// Modernisierung von Rx_FizzBuzz (2010): eigener Operator als Extension-Method auf IObservable.
/// Die if/else-Kaskade wird zu einem Switch-Ausdruck über ein Tuple-Pattern.
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

    /// <summary>Ein eigener Operator ist nur eine Funktion IObservable&lt;A&gt; → IObservable&lt;B&gt; (RxJS: pipeable operator).</summary>
    public static IObservable<string> FizzBuzz(this IObservable<int> source) => source.Select(ToFizzBuzz);

    /// <summary>Dieselbe Logik als IAsyncEnumerable (Pull statt Push) – .NET 10 bringt async LINQ im BCL mit.</summary>
    public static IAsyncEnumerable<string> FizzBuzz(this IAsyncEnumerable<int> source) => source.Select(ToFizzBuzz);
}
