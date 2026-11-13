using System.Collections.Specialized;

namespace Exercises.RxNetDynamicData;

public class ProductCatalogTests
{
    private static readonly Product Disc = new("p1", "Bremsscheibe", 4_990, 12);
    private static readonly Product Pads = new("p2", "Bremsbeläge", 2_490, 30);
    private static readonly Product Plug = new("p3", "Zündkerze", 790, 40);

    private static IEnumerable<string> Names(ProductCatalog catalog) => catalog.Visible.Select(p => p.Name);

    [Fact]
    public void Visible_is_sorted_by_price()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Pads, Plug);

        Assert.Equal(["Zündkerze", "Bremsbeläge", "Bremsscheibe"], Names(catalog));
    }

    [Fact]
    public void The_search_term_filters_live_case_insensitively()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Pads, Plug);

        catalog.Search("brems");
        Assert.Equal(["Bremsbeläge", "Bremsscheibe"], Names(catalog));

        catalog.Search("");
        Assert.Equal(3, catalog.Visible.Count);
    }

    [Fact]
    public void New_and_changed_products_appear_in_the_right_place()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Plug);
        catalog.Search("e");

        catalog.Upsert(Pads);
        catalog.Upsert(Disc with { PriceCents = 590 }); // special offer

        Assert.Equal(["Bremsscheibe", "Zündkerze", "Bremsbeläge"], Names(catalog));
    }

    [Fact]
    public void Remove_takes_the_product_out_of_the_view()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Pads, Plug);

        catalog.Remove(Pads.Id);

        Assert.Equal(["Zündkerze", "Bremsscheibe"], Names(catalog));
    }

    [Fact]
    public void A_new_product_changes_the_collection_precisely_instead_of_rebuilding_it()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Plug);
        var actions = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)catalog.Visible).CollectionChanged += (_, e) => actions.Add(e.Action);

        catalog.Upsert(Pads);

        Assert.Equal([NotifyCollectionChangedAction.Add], actions);
    }
}
