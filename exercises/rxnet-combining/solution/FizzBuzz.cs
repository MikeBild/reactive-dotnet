using System.Globalization;

namespace Exercises.RxNetCombining;

public static class FizzBuzz
{
    /// <summary>Wiederholt die Werte endlos: Cycle(["a", "b"]) liefert a, b, a, b, …</summary>
    public static IObservable<T> Cycle<T>(params T[] values) =>
        // Synchron und endlos: Wer das allein abonniert, kommt nur mit Take wieder heraus.
        values.ToObservable().Repeat();

    /// <summary>FizzBuzz von 1 bis max als Stream von Strings.</summary>
    public static IObservable<string> Create(int max = 100) =>
        // Zip paart den n-ten Wert beider Quellen und endet, sobald die kürzere Quelle fertig ist,
        // hier Range. Danach meldet es den endlosen Zyklus ab.
        // CombineLatest wäre falsch: Es paart den neuesten Stand, nicht Wert für Wert.
        //
        // Zip wird zweimal paarweise angewendet. Die Variante Observable.Zip(a, b, c, …) mit drei
        // Quellen auf einmal endet bei zwei synchronen, endlosen Zyklen nicht. Probier es aus.
        Observable.Range(1, max)
            .Zip(Cycle("", "", "Fizz"), (number, fizz) => (number, fizz))
            .Zip(Cycle("", "", "", "", "Buzz"), (pair, buzz) => pair.fizz + buzz is { Length: > 0 } word
                ? word
                : pair.number.ToString(CultureInfo.InvariantCulture));
}
