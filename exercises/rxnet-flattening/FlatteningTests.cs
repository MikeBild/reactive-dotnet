namespace Exercises.RxNetFlattening;

public class FlatteningTests : ReactiveTest
{
    // In diesen Tests ist ein Tick eine Zeiteinheit. Die Zeiten sind absolut.

    private static IReadOnlyList<Product> Found(string name) => [new Product(name, name, 990, 1)];

    [Fact]
    public void SearchResults_bricht_die_laufende_Suche_ab_wenn_ein_neuer_Begriff_kommt()
    {
        var scheduler = new TestScheduler();
        var terms = scheduler.CreateHotObservable(OnNext(210, "Brems"), OnNext(250, "Zünd"));
        var slow = scheduler.CreateColdObservable(OnNext(100, Found("Bremsscheibe")), OnCompleted<IReadOnlyList<Product>>(100));
        var fast = scheduler.CreateColdObservable(OnNext(10, Found("Zündkerze")), OnCompleted<IReadOnlyList<Product>>(10));

        var observer = scheduler.Start(() => Flattening.SearchResults(terms, term => term == "Brems" ? slow : fast));

        Assert.Equal(["Zündkerze"], observer.Messages.Where(m => m.Value.HasValue).Select(m => m.Value.Value[0].Name));
        Assert.Equal(260, observer.Messages[0].Time);
        Assert.Equal(Subscribe(210, 250), Assert.Single(slow.Subscriptions)); // abgebrochen
    }

    [Fact]
    public void Autosave_speichert_jede_Aenderung_in_Reihenfolge()
    {
        var scheduler = new TestScheduler();
        var changes = scheduler.CreateHotObservable(OnNext(210, "a"), OnNext(215, "b"), OnNext(220, "c"));
        // a dauert länger als b und c, trotzdem muss a zuerst gespeichert sein
        Func<string, IObservable<string>> save = change => scheduler.CreateColdObservable(
            OnNext(change == "a" ? 50 : 5, change), OnCompleted<string>(change == "a" ? 50 : 5));

        var observer = scheduler.Start(() => Flattening.Autosave(changes, save));

        Assert.Equal(
            [OnNext(260, "a"), OnNext(265, "b"), OnNext(270, "c")],
            observer.Messages);
    }

    [Fact]
    public void UploadQueue_laedt_parallel_aber_hoechstens_maxConcurrent_gleichzeitig()
    {
        var scheduler = new TestScheduler();
        var files = scheduler.CreateHotObservable(OnNext(210, "1"), OnNext(210, "2"), OnNext(210, "3"));
        var running = 0;
        var maxRunning = 0;
        Func<string, IObservable<string>> upload = file => Observable.Defer(() =>
        {
            maxRunning = Math.Max(maxRunning, ++running);
            return Observable.Return(file).Delay(TimeSpan.FromTicks(100), scheduler).Finally(() => running--);
        });

        var observer = scheduler.Start(() => Flattening.UploadQueue(files, upload, maxConcurrent: 2));

        Assert.Equal(["1", "2", "3"], observer.Messages.Where(m => m.Value.HasValue).Select(m => m.Value.Value).Order());
        Assert.Equal(2, maxRunning);
    }

    [Fact]
    public void LoginOnClick_ignoriert_Klicks_solange_der_Login_laeuft()
    {
        var scheduler = new TestScheduler();
        var clicks = scheduler.CreateHotObservable(
            OnNext(210, Unit.Default), OnNext(220, Unit.Default), OnNext(300, Unit.Default));
        var logins = 0;
        Func<IObservable<string>> login = () => Observable.Defer(() =>
        {
            logins++;
            return scheduler.CreateColdObservable(OnNext(50, $"session {logins}"), OnCompleted<string>(50));
        });

        var observer = scheduler.Start(() => Flattening.LoginOnClick(clicks, login));

        Assert.Equal(2, logins); // der Doppelklick bei 220 startet keinen zweiten Login
        Assert.Equal([OnNext(260, "session 1"), OnNext(350, "session 2")], observer.Messages);
    }
}
