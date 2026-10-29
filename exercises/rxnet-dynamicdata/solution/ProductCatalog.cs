using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;

namespace Exercises.RxNetDynamicData;

public sealed class ProductCatalog : IDisposable
{
    // SourceCache hält die Produkte nach Id. Jede Änderung wird als ChangeSet verschickt,
    // also nur Add, Update oder Remove der betroffenen Einträge statt der ganzen Liste.
    private readonly SourceCache<Product, string> _products = new(product => product.Id);
    private readonly BehaviorSubject<string> _term = new("");
    private readonly ReadOnlyObservableCollection<Product> _visible;
    private readonly IDisposable _subscription;

    public ProductCatalog()
    {
        // Der Filter ist selbst ein Stream: Ein neuer Suchbegriff filtert den Bestand neu.
        var matches = _term.Select(term => (Func<Product, bool>)(product =>
            product.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));

        _subscription = _products
            .Connect()
            .Filter(matches)
            // SortAndBind sortiert und schreibt die ChangeSets in die Collection für die UI-Bindung.
            .SortAndBind(out _visible, SortExpressionComparer<Product>.Ascending(product => product.PriceCents))
            .Subscribe();
    }

    /// <summary>Die gefilterte, nach Preis sortierte Sicht. Die View bindet nur an diese Collection.</summary>
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
