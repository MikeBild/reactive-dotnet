// Lesson rxnet-dynamicdata: A look at DynamicData
//
// Task: Keep the products in a SourceCache, filter them live by search term,
// sort by price, and bind the result to a ReadOnlyObservableCollection.
//
// DynamicData is to lists what Rx is to single values: Instead of the whole list,
// only the changes (ChangeSets) flow through the pipeline.

using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;

namespace Exercises.RxNetDynamicData;

public sealed class ProductCatalog : IDisposable
{
    // TODO: SourceCache<Product, string> with the Id as key, BehaviorSubject<string> for the search term,
    //       one field for the ReadOnlyObservableCollection and one for the Subscription

    public ProductCatalog()
    {
        // TODO: _products.Connect()
        //         .Filter(stream of Func<Product, bool> from the search term, case-insensitive)
        //         .SortAndBind(out _visible, SortExpressionComparer<Product>.Ascending(p => p.PriceCents))
        //         .Subscribe()
        throw new NotImplementedException("TODO: ProductCatalog");
    }

    /// <summary>The filtered view, sorted by price. The UI view binds only to this collection.</summary>
    public ReadOnlyObservableCollection<Product> Visible => throw new NotImplementedException("TODO: Visible");

    public void Upsert(params Product[] products) => throw new NotImplementedException("TODO: AddOrUpdate");

    public void Remove(string productId) => throw new NotImplementedException("TODO: RemoveKey");

    public void Search(string term) => throw new NotImplementedException("TODO: Suchbegriff mit OnNext");

    public void Dispose()
    {
        // TODO: Dispose the Subscription, SourceCache, and Subject
    }
}
