// Lesson rxnet-combining: Which Operator combines your streams?
//
// Task: Solve FizzBuzz reactively by combining the numbers one to one hundred with two
// cyclic streams for Fizz and Buzz using Zip.
//
// Without modulo: Fizz is every third, Buzz every fifth value of an endless cycle.

namespace Exercises.RxNetCombining;

public static class FizzBuzz
{
    /// <summary>Repeats the values endlessly: Cycle(["a", "b"]) yields a, b, a, b, …</summary>
    public static IObservable<T> Cycle<T>(params T[] values)
    {
        // TODO: ToObservable and Repeat
        throw new NotImplementedException("TODO: Cycle");
    }

    /// <summary>FizzBuzz from 1 to max as a stream of strings.</summary>
    public static IObservable<string> Create(int max = 100)
    {
        // TODO: Combine Observable.Range(1, max) first with Cycle("", "", "Fizz") and the result
        //       with Cycle("", "", "", "", "Buzz") using Zip. If fizz + buzz is empty,
        //       use the number.
        throw new NotImplementedException("TODO: FizzBuzz.Create");
    }
}
