// Lesson rxnet-push-pull: Push instead of pull
//
// Task: Write an iterator with yield return for the numbers one to ten and
// an Observable with the same values. Compare who triggers the next value in each case.
//
// onProduce reports when a value is produced. This way the tests see who sets the pace.

namespace Exercises.RxNetPushPull;

public static class Counting
{
    /// <summary>Pull: The consumer fetches every value with MoveNext.</summary>
    public static IEnumerable<int> CountTo(int max, Action<int>? onProduce = null)
    {
        // TODO: for loop with onProduce?.Invoke(n) and yield return n
        throw new NotImplementedException("TODO: CountTo");
    }

    /// <summary>Push: The producer sends all values as soon as someone calls Subscribe.</summary>
    public static IObservable<int> CountToObservable(int max, Action<int>? onProduce = null)
    {
        // TODO: Observable.Create, inside it OnNext for every value and OnCompleted at the end
        throw new NotImplementedException("TODO: CountToObservable");
    }
}
