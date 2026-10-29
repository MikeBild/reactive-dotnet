using System.Collections.Specialized;
using System.Reactive.Linq;
using ReactiveSamples.BoundedCollection;

namespace ReactiveSamples.Tests;

public class BoundedCollectionTests
{
    [Fact]
    public void Keeps_the_last_n_items()
    {
        var collection = new BoundedObservableCollection<int>(3);
        foreach (var i in Enumerable.Range(1, 5))
        {
            collection.Add(i);
        }
        Assert.Equal([3, 4, 5], collection);
    }

    [Fact]
    public void Insert_at_front_evicts_the_oldest_not_the_new_item()
    {
        // Das Original entfernte per RemoveAt(0) genau das eben eingefügte Element.
        var collection = new BoundedObservableCollection<int>(3) { 1, 2, 3 };
        collection.Insert(0, 9);
        Assert.Equal([9, 2, 3], collection);
    }

    [Fact]
    public void Manual_removes_keep_the_eviction_order_in_sync()
    {
        var collection = new BoundedObservableCollection<int>(3) { 1, 2, 3 };
        collection.Remove(1);
        collection.Add(4);
        collection.Add(5);
        Assert.Equal([3, 4, 5], collection);
    }

    [Fact]
    public void Collection_changes_are_observable()
    {
        var collection = new BoundedObservableCollection<string>(2);
        var actions = new List<NotifyCollectionChangedAction>();
        using var _ = collection.Changes.Select(e => e.Action).Subscribe(actions.Add);

        collection.Add("a");
        collection.Add("b");
        collection.Add("c");

        Assert.Equal(
            [NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove],
            actions);
    }

    [Fact]
    public async Task Sliding_window_with_Scan()
    {
        var windows = await Observable.Range(1, 5).LastN(3).Select(w => string.Join(",", w)).ToArray();
        Assert.Equal(["1", "1,2", "1,2,3", "2,3,4", "3,4,5"], windows);
    }

    [Fact]
    public async Task Full_windows_with_Buffer()
    {
        var windows = await Observable.Range(1, 5).FullWindows(3).Select(w => string.Join(",", w)).ToArray();
        Assert.Equal(["1,2,3", "2,3,4", "3,4,5"], windows);
    }
}
