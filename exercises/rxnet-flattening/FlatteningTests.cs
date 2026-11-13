namespace Exercises.RxNetFlattening;

public class FlatteningTests : ReactiveTest
{
    // In these tests, one tick is one unit of time. The times are absolute.

    private static IReadOnlyList<Product> Found(string name) => [new Product(name, name, 990, 1)];

    [Fact]
    public void SearchResults_cancels_the_running_search_when_a_new_term_arrives()
    {
        var scheduler = new TestScheduler();
        var terms = scheduler.CreateHotObservable(OnNext(210, "Brems"), OnNext(250, "Zünd"));
        var slow = scheduler.CreateColdObservable(OnNext(100, Found("Bremsscheibe")), OnCompleted<IReadOnlyList<Product>>(100));
        var fast = scheduler.CreateColdObservable(OnNext(10, Found("Zündkerze")), OnCompleted<IReadOnlyList<Product>>(10));

        var observer = scheduler.Start(() => Flattening.SearchResults(terms, term => term == "Brems" ? slow : fast));

        Assert.Equal(["Zündkerze"], observer.Messages.Where(m => m.Value.HasValue).Select(m => m.Value.Value[0].Name));
        Assert.Equal(260, observer.Messages[0].Time);
        Assert.Equal(Subscribe(210, 250), Assert.Single(slow.Subscriptions)); // canceled
    }

    [Fact]
    public void Autosave_saves_every_change_in_order()
    {
        var scheduler = new TestScheduler();
        var changes = scheduler.CreateHotObservable(OnNext(210, "a"), OnNext(215, "b"), OnNext(220, "c"));
        // a takes longer than b and c, yet a must be saved first
        Func<string, IObservable<string>> save = change => scheduler.CreateColdObservable(
            OnNext(change == "a" ? 50 : 5, change), OnCompleted<string>(change == "a" ? 50 : 5));

        var observer = scheduler.Start(() => Flattening.Autosave(changes, save));

        Assert.Equal(
            [OnNext(260, "a"), OnNext(265, "b"), OnNext(270, "c")],
            observer.Messages);
    }

    [Fact]
    public void UploadQueue_uploads_in_parallel_but_at_most_maxConcurrent_at_once()
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
    public void LoginOnClick_ignores_clicks_while_the_login_is_running()
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

        Assert.Equal(2, logins); // the double click at 220 does not start a second login
        Assert.Equal([OnNext(260, "session 1"), OnNext(350, "session 2")], observer.Messages);
    }
}
