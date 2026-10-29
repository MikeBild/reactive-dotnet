// Lektion rxnet-combining: Welcher Operator kombiniert deine Streams?
//
// Aufgabe: Löse FizzBuzz reaktiv, indem du die Zahlen eins bis hundert mit zwei
// zyklischen Streams für Fizz und Buzz per Zip kombinierst.
//
// Ohne Modulo: Fizz ist jeder dritte, Buzz jeder fünfte Wert eines endlosen Zyklus.

namespace Exercises.RxNetCombining;

public static class FizzBuzz
{
    /// <summary>Wiederholt die Werte endlos: Cycle(["a", "b"]) liefert a, b, a, b, …</summary>
    public static IObservable<T> Cycle<T>(params T[] values)
    {
        // TODO: ToObservable und Repeat
        throw new NotImplementedException("TODO: Cycle");
    }

    /// <summary>FizzBuzz von 1 bis max als Stream von Strings.</summary>
    public static IObservable<string> Create(int max = 100)
    {
        // TODO: Observable.Range(1, max) zuerst mit Cycle("", "", "Fizz") und das Ergebnis
        //       mit Cycle("", "", "", "", "Buzz") per Zip kombinieren. Ist fizz + buzz leer,
        //       gilt die Zahl.
        throw new NotImplementedException("TODO: FizzBuzz.Create");
    }
}
