namespace Exercises.RxNetFlattening;

public static class Flattening
{
    /// <summary>Suche: nur das Ergebnis zum neuesten Begriff zählt.</summary>
    public static IObservable<IReadOnlyList<Product>> SearchResults(
        IObservable<string> terms, Func<string, IObservable<IReadOnlyList<Product>>> search) =>
        // Switch: Ein neuer Begriff macht die laufende Suche wertlos, also abbrechen und wechseln.
        // Lesende Requests darfst du gefahrlos abbrechen.
        terms.Select(search).Switch();

    /// <summary>Autosave: jede Änderung speichern, in der Reihenfolge, in der sie entstanden ist.</summary>
    public static IObservable<T> Autosave<T>(IObservable<T> changes, Func<T, IObservable<T>> save) =>
        // Concat: Keine Änderung darf verloren gehen, und die Reihenfolge muss stimmen.
        // Switch könnte einen halben Speichervorgang abbrechen, Merge die Reihenfolge vertauschen.
        changes.Select(save).Concat();

    /// <summary>Upload-Queue: Dateien parallel hochladen, aber höchstens maxConcurrent gleichzeitig.</summary>
    public static IObservable<string> UploadQueue(
        IObservable<string> files, Func<string, IObservable<string>> upload, int maxConcurrent) =>
        // Merge mit Limit: Uploads sind unabhängig, parallel ist schneller.
        // Ohne Limit (SelectMany) liefen hundert Dateien als hundert Requests gleichzeitig.
        files.Select(upload).Merge(maxConcurrent);

    /// <summary>Login: Ein Doppelklick darf keinen zweiten Request starten, solange der erste läuft.</summary>
    public static IObservable<TSession> LoginOnClick<TSession>(IObservable<Unit> clicks, Func<IObservable<TSession>> login) =>
        // Exhaust: Solange ein Login läuft, werden weitere Klicks ignoriert.
        clicks.ExhaustMap(_ => login());

    /// <summary>Ignoriert neue Werte, solange der innere Stream des vorigen Werts noch läuft.</summary>
    public static IObservable<TResult> ExhaustMap<TSource, TResult>(
        this IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector) =>
        // Defer: Jede Subscription bekommt ihr eigenes Flag, sonst teilten sich zwei Subscriber den Zustand.
        Observable.Defer(() =>
        {
            var running = 0;
            return source
                .Where(_ => Interlocked.CompareExchange(ref running, 1, 0) == 0)
                .SelectMany(value => selector(value).Finally(() => Volatile.Write(ref running, 0)));
        });
}
