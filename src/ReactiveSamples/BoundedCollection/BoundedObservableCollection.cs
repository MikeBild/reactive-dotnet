using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace ReactiveSamples.BoundedCollection;

/// <summary>
/// Modernisierung von Gist "LRUObservableCollection" (2012).
/// Ehrlicher Name: das Original ist kein LRU-Cache, sondern ein FIFO-Puffer fester Länge
/// ("die letzten N Einträge", z. B. für ein Log-Fenster in der UI).
/// Fehler im Original: Insert(0, x) bei vollem Puffer entfernte per RemoveAt(0) genau das
/// gerade eingefügte Element. Hier wird immer das älteste Element entfernt.
/// </summary>
public sealed class BoundedObservableCollection<T> : ObservableCollection<T>
{
    private readonly int _capacity;
    private readonly List<T> _insertionOrder = []; // ältestes Element vorn

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

    /// <summary>Brücke Event → Observable (RxJS: fromEvent).</summary>
    public IObservable<NotifyCollectionChangedEventArgs> Changes =>
        Observable
            .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => CollectionChanged += h,
                h => CollectionChanged -= h)
            .Select(e => e.EventArgs);
}

/// <summary>
/// Dieselbe Idee ohne veränderliche Collection: ein gleitendes Fenster als Operator.
/// Scan ist das Rx-Pendant zu reduce, das jeden Zwischenstand emittiert (RxJS: scan).
/// </summary>
public static class SlidingWindowExtensions
{
    public static IObservable<ImmutableQueue<T>> LastN<T>(this IObservable<T> source, int capacity) =>
        source.Scan(ImmutableQueue<T>.Empty, (window, item) =>
        {
            var next = window.Enqueue(item);
            return next.Count() > capacity ? next.Dequeue() : next;
        });

    /// <summary>Eingebaute Alternative: Buffer(count, skip) liefert überlappende Fenster – aber erst, wenn sie voll sind.</summary>
    public static IObservable<IList<T>> FullWindows<T>(this IObservable<T> source, int capacity) =>
        source.Buffer(capacity, 1).Where(window => window.Count == capacity);
}
