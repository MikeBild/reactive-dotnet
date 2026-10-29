namespace Exercises.RxNetPushPull;

public static class Counting
{
    /// <summary>Pull: Der Consumer holt jeden Wert mit MoveNext.</summary>
    public static IEnumerable<int> CountTo(int max, Action<int>? onProduce = null)
    {
        // Der Iterator läuft erst weiter, wenn foreach oder MoveNext den nächsten Wert verlangt.
        for (var n = 1; n <= max; n++)
        {
            onProduce?.Invoke(n);
            yield return n;
        }
    }

    /// <summary>Push: Der Producer schickt alle Werte, sobald jemand Subscribe aufruft.</summary>
    public static IObservable<int> CountToObservable(int max, Action<int>? onProduce = null) =>
        Observable.Create<int>(observer =>
        {
            // Hier entscheidet der Producer, wann der nächste Wert kommt, der Observer reagiert nur.
            for (var n = 1; n <= max; n++)
            {
                onProduce?.Invoke(n);
                observer.OnNext(n);
            }
            observer.OnCompleted();
            return Disposable.Empty;
        });
}
