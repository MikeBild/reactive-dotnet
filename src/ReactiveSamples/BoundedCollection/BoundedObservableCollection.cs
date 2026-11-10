using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace ReactiveSamples.BoundedCollection;

/// <summary>
/// Modernization of gist "LRUObservableCollection" (2012).
/// Honest name: the original is not an LRU cache but a fixed-length FIFO buffer
/// ("the last N entries", e.g. for a log window in the UI).
/// Bug in the original: Insert(0, x) on a full buffer removed, via RemoveAt(0), exactly the
/// element just inserted. Here, the oldest element is always removed.
/// </summary>
public sealed class BoundedObservableCollection<T> : ObservableCollection<T>
{
    private readonly int _capacity;
    private readonly List<T> _insertionOrder = []; // oldest element first

    public BoundedObservableCollection(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _capacity = capacity;
    }

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);
        _insertionOrder.Add(item);
        if (Count > _capacity)
        {
            var oldest = _insertionOrder[0];
            _insertionOrder.RemoveAt(0);
            base.RemoveItem(IndexOf(oldest));
        }
    }

    protected override void RemoveItem(int index)
    {
        _insertionOrder.Remove(this[index]);
        base.RemoveItem(index);
    }

    protected override void SetItem(int index, T item)
    {
        _insertionOrder[_insertionOrder.IndexOf(this[index])] = item;
        base.SetItem(index, item);
    }

    protected override void ClearItems()
    {
        _insertionOrder.Clear();
        base.ClearItems();
    }

    /// <summary>Bridge event → Observable (RxJS: fromEvent).</summary>
    public IObservable<NotifyCollectionChangedEventArgs> Changes =>
        Observable
            .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => CollectionChanged += h,
                h => CollectionChanged -= h)
            .Select(e => e.EventArgs);
}

/// <summary>
/// The same idea without a mutable collection: a sliding window as an Operator.
/// Scan is the Rx counterpart to reduce that emits every intermediate state (RxJS: scan).
/// </summary>
public static class SlidingWindowExtensions
{
    public static IObservable<ImmutableQueue<T>> LastN<T>(this IObservable<T> source, int capacity) =>
        source.Scan(ImmutableQueue<T>.Empty, (window, item) =>
        {
            var next = window.Enqueue(item);
            return next.Count() > capacity ? next.Dequeue() : next;
        });

    /// <summary>Built-in alternative: Buffer(count, skip) emits overlapping windows – but only once they are full.</summary>
    public static IObservable<IList<T>> FullWindows<T>(this IObservable<T> source, int capacity) =>
        source.Buffer(capacity, 1).Where(window => window.Count == capacity);
}
