using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;

namespace Exercises.RxNetDynamicData;

public sealed class ProductCatalog : IDisposable
{
    // SourceCache keeps the products by Id. Every change is sent as a ChangeSet,
    // so only Add, Update, or Remove of the affected entries instead of the whole list.
    private readonly SourceCache<Product, string> _products = new(product => product.Id);
    private readonly BehaviorSubject<string> _term = new("");
    private readonly ReadOnlyObservableCollection<Product> _visible;
    private readonly IDisposable _subscription;

    public ProductCatalog()
    {
        // The filter is itself a stream: A new search term filters the existing items again.
        var matches = _term.Select(term => (Func<Product, bool>)(product =>
            product.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));

        _subscription = _products
            .Connect()
            .Filter(matches)
            // SortAndBind sorts and writes the ChangeSets into the collection for UI binding.
            .SortAndBind(out _visible, SortExpressionComparer<Product>.Ascending(product => product.PriceCents))
            .Subscribe();
    }

    /// <summary>The filtered view, sorted by price. The UI view binds only to this collection.</summary>
    public ReadOnlyObservableCollection<Product> Visible => _visible;

    public void Upsert(params Product[] products) => _products.AddOrUpdate(products);

    public void Remove(string productId) => _products.RemoveKey(productId);

    public void Search(string term) => _term.OnNext(term.Trim());

    public void Dispose()
    {
        _subscription.Dispose();
        _products.Dispose();
        _term.Dispose();
    }
}
