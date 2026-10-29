using System.Collections.Specialized;

namespace Exercises.RxNetDynamicData;

public class ProductCatalogTests
{
    private static readonly Product Disc = new("p1", "Bremsscheibe", 4_990, 12);
    private static readonly Product Pads = new("p2", "Bremsbeläge", 2_490, 30);
    private static readonly Product Plug = new("p3", "Zündkerze", 790, 40);

    private static IEnumerable<string> Names(ProductCatalog catalog) => catalog.Visible.Select(p => p.Name);

    [Fact]
    public void Visible_ist_nach_Preis_sortiert()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Pads, Plug);

        Assert.Equal(["Zündkerze", "Bremsbeläge", "Bremsscheibe"], Names(catalog));
    }

    [Fact]
    public void Der_Suchbegriff_filtert_live_ohne_Gross_und_Kleinschreibung()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Pads, Plug);

        catalog.Search("brems");
        Assert.Equal(["Bremsbeläge", "Bremsscheibe"], Names(catalog));

        catalog.Search("");
        Assert.Equal(3, catalog.Visible.Count);
    }

    [Fact]
    public void Neue_und_geaenderte_Produkte_erscheinen_an_der_richtigen_Stelle()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Plug);
        catalog.Search("e");

        catalog.Upsert(Pads);
        catalog.Upsert(Disc with { PriceCents = 590 }); // Sonderangebot

        Assert.Equal(["Bremsscheibe", "Zündkerze", "Bremsbeläge"], Names(catalog));
    }

    [Fact]
    public void Remove_entfernt_das_Produkt_aus_der_Sicht()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Pads, Plug);

        catalog.Remove(Pads.Id);

        Assert.Equal(["Zündkerze", "Bremsscheibe"], Names(catalog));
    }

    [Fact]
    public void Ein_neues_Produkt_aendert_die_Collection_gezielt_statt_sie_neu_aufzubauen()
    {
        using var catalog = new ProductCatalog();
        catalog.Upsert(Disc, Plug);
        var actions = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)catalog.Visible).CollectionChanged += (_, e) => actions.Add(e.Action);

        catalog.Upsert(Pads);

        Assert.Equal([NotifyCollectionChangedAction.Add], actions);
    }
}
