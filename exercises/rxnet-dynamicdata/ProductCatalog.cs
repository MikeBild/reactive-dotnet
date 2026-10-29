// Lektion rxnet-dynamicdata: Ausblick DynamicData
//
// Aufgabe: Halte die Produkte in einem SourceCache, filtere sie live nach Suchbegriff,
// sortiere nach Preis und binde das Ergebnis an eine ReadOnlyObservableCollection.
//
// DynamicData ist für Listen, was Rx für einzelne Werte ist: Statt der ganzen Liste
// fließen nur die Änderungen (ChangeSets) durch die Pipeline.

using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;

namespace Exercises.RxNetDynamicData;

public sealed class ProductCatalog : IDisposable
{
    // TODO: SourceCache<Product, string> mit der Id als Key, BehaviorSubject<string> für den Suchbegriff,
    //       ein Feld für die ReadOnlyObservableCollection und eins für die Subscription

    public ProductCatalog()
    {
        // TODO: _products.Connect()
        //         .Filter(Stream von Func<Product, bool> aus dem Suchbegriff, ohne Groß- und Kleinschreibung)
        //         .SortAndBind(out _visible, SortExpressionComparer<Product>.Ascending(p => p.PriceCents))
        //         .Subscribe()
        throw new NotImplementedException("TODO: ProductCatalog");
    }

    /// <summary>Die gefilterte, nach Preis sortierte Sicht. Die View bindet nur an diese Collection.</summary>
    public ReadOnlyObservableCollection<Product> Visible => throw new NotImplementedException("TODO: Visible");

    public void Upsert(params Product[] products) => throw new NotImplementedException("TODO: AddOrUpdate");

    public void Remove(string productId) => throw new NotImplementedException("TODO: RemoveKey");

    public void Search(string term) => throw new NotImplementedException("TODO: Suchbegriff mit OnNext");

    public void Dispose()
    {
        // TODO: Subscription, SourceCache und Subject entsorgen
    }
}
