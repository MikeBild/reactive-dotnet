// Lektion rxnet-flattening: Switch, Concat, Merge oder Exhaust?
//
// Aufgabe: Bau Produktsuche, Autosave, Upload-Queue und Login-Button mit je einer
// Flattening-Strategie und begründe jede Wahl im Code-Kommentar.
//
// Alle Services kommen als Parameter rein. Im Test ersetzt sie ein Cold Observable
// aus dem TestScheduler, ganz ohne Mock-Library.
//
// Rx.NET bringt drei Strategien mit: Select(…).Switch(), Select(…).Concat() und
// Select(…).Merge(maxConcurrent) bzw. SelectMany. Ein exhaustMap wie in RxJS gibt es
// nicht. Den schreibst du hier selbst als Extension Method.

namespace Exercises.RxNetFlattening;

public static class Flattening
{
    /// <summary>Suche: nur das Ergebnis zum neuesten Begriff zählt.</summary>
    public static IObservable<IReadOnlyList<Product>> SearchResults(
        IObservable<string> terms, Func<string, IObservable<IReadOnlyList<Product>>> search)
    {
        // TODO: Welche Strategie? Begründung:
        throw new NotImplementedException("TODO: SearchResults");
    }

    /// <summary>Autosave: jede Änderung speichern, in der Reihenfolge, in der sie entstanden ist.</summary>
    public static IObservable<T> Autosave<T>(IObservable<T> changes, Func<T, IObservable<T>> save)
    {
        // TODO: Welche Strategie? Begründung:
        throw new NotImplementedException("TODO: Autosave");
    }

    /// <summary>Upload-Queue: Dateien parallel hochladen, aber höchstens maxConcurrent gleichzeitig.</summary>
    public static IObservable<string> UploadQueue(
        IObservable<string> files, Func<string, IObservable<string>> upload, int maxConcurrent)
    {
        // TODO: Welche Strategie? Begründung:
        throw new NotImplementedException("TODO: UploadQueue");
    }

    /// <summary>Login: Ein Doppelklick darf keinen zweiten Request starten, solange der erste läuft.</summary>
    public static IObservable<TSession> LoginOnClick<TSession>(IObservable<Unit> clicks, Func<IObservable<TSession>> login)
    {
        // TODO: Welche Strategie? Begründung:
        throw new NotImplementedException("TODO: LoginOnClick");
    }

    /// <summary>Ignoriert neue Werte, solange der innere Stream des vorigen Werts noch läuft.</summary>
    public static IObservable<TResult> ExhaustMap<TSource, TResult>(
        this IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
    {
        // TODO: Pro Subscription ein Flag „läuft gerade“ (Observable.Defer),
        //       Where lässt nur Werte durch, wenn nichts läuft,
        //       SelectMany startet den inneren Stream, Finally setzt das Flag zurück.
        throw new NotImplementedException("TODO: ExhaustMap");
    }
}
