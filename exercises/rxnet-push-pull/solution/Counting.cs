namespace Exercises.RxNetPushPull;

public static class Counting
{
    /// <summary>Pull: The consumer fetches every value with MoveNext.</summary>
    public static IEnumerable<int> CountTo(int max, Action<int>? onProduce = null)
    {
        // The iterator only continues when foreach or MoveNext asks for the next value.
        for (var n = 1; n <= max; n++)
        {
            onProduce?.Invoke(n);
            yield return n;
        }
    }

    /// <summary>Push: The producer sends all values as soon as someone calls Subscribe.</summary>
    public static IObservable<int> CountToObservable(int max, Action<int>? onProduce = null) =>
        Observable.Create<int>(observer =>
        {
            // Here the producer decides when the next value comes, the Observer only reacts.
            for (var n = 1; n <= max; n++)
            {
                onProduce?.Invoke(n);
                observer.OnNext(n);
            }
            observer.OnCompleted();
            return Disposable.Empty;
        });
}
