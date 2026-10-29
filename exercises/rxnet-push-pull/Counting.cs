// Lektion rxnet-push-pull: Push statt Pull
//
// Aufgabe: Schreib einen Iterator mit yield return für die Zahlen eins bis zehn und
// ein Observable mit denselben Werten. Vergleiche, wer jeweils den nächsten Wert anstößt.
//
// onProduce meldet, wann ein Wert entsteht. So sehen die Tests, wer das Tempo bestimmt.

namespace Exercises.RxNetPushPull;

public static class Counting
{
    /// <summary>Pull: Der Consumer holt jeden Wert mit MoveNext.</summary>
    public static IEnumerable<int> CountTo(int max, Action<int>? onProduce = null)
    {
        // TODO: for-Schleife mit onProduce?.Invoke(n) und yield return n
        throw new NotImplementedException("TODO: CountTo");
    }

    /// <summary>Push: Der Producer schickt alle Werte, sobald jemand Subscribe aufruft.</summary>
    public static IObservable<int> CountToObservable(int max, Action<int>? onProduce = null)
    {
        // TODO: Observable.Create, darin OnNext für jeden Wert und am Ende OnCompleted
        throw new NotImplementedException("TODO: CountToObservable");
    }
}
