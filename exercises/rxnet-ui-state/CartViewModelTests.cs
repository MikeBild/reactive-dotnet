namespace Exercises.RxNetUiState;

public class CartViewModelTests
{
    private static readonly Product Brake = new("p1", "Bremsscheibe", 4_990, 12);
    private static readonly Product Plug = new("p2", "Zündkerze", 790, 40);

    private static List<T> Watch<T>(IObservable<T> source, List<IDisposable> subscriptions)
    {
        var values = new List<T>();
        subscriptions.Add(source.Subscribe(values.Add));
        return values;
    }

    [Fact]
    public void ReduceCart_fuegt_hinzu_erhoeht_die_Menge_und_entfernt()
    {
        IReadOnlyList<CartItem> cart = [];
        cart = Reducers.ReduceCart(cart, new AddItem(new CartItem("p1", 1, 4_990)));
        cart = Reducers.ReduceCart(cart, new AddItem(new CartItem("p1", 2, 4_990)));
        cart = Reducers.ReduceCart(cart, new AddItem(new CartItem("p2", 1, 790)));
        Assert.Equal([new CartItem("p1", 3, 4_990), new CartItem("p2", 1, 790)], cart);

        cart = Reducers.ReduceCart(cart, new RemoveItem("p1"));
        Assert.Equal([new CartItem("p2", 1, 790)], cart);
    }

    [Fact]
    public void ReduceCart_aendert_die_alte_Liste_nicht()
    {
        IReadOnlyList<CartItem> before = [new CartItem("p1", 1, 4_990)];
        var after = Reducers.ReduceCart(before, new AddItem(new CartItem("p2", 1, 790)));
        Assert.Single(before);
        Assert.NotSame(before, after);
    }

    [Fact]
    public void PushRecent_haelt_die_letzten_fuenf_ohne_Duplikate_die_neueste_vorn()
    {
        IReadOnlyList<string> recent = [];
        foreach (var term in new[] { "a", "b", "c", "b", "d", "e", "f", "  " })
            recent = Reducers.PushRecent(recent, term);

        Assert.Equal(["f", "e", "d", "b", "c"], recent);
    }

    [Fact]
    public void TotalCents_und_CanPlace_folgen_den_Aktionen()
    {
        var subscriptions = new List<IDisposable>();
        var vm = new CartViewModel();
        var totals = Watch(vm.TotalCents, subscriptions);
        var canPlace = Watch(vm.CanPlace, subscriptions);

        vm.Add(Brake);
        vm.Add(Plug, 4);
        vm.SetPlacing(true);
        vm.SetPlacing(false);
        vm.Remove(Brake.Id);
        vm.Remove(Plug.Id);

        Assert.Equal([0, 4_990, 4_990 + 4 * 790, 4 * 790, 0], totals);
        Assert.Equal([false, true, false, true, false], canPlace);
        subscriptions.ForEach(s => s.Dispose());
    }

    [Fact]
    public void Spaete_Subscriber_bekommen_sofort_den_aktuellen_Warenkorb()
    {
        var vm = new CartViewModel();
        using var first = vm.Cart.Subscribe(_ => { });
        vm.Add(Brake);

        var seen = new List<IReadOnlyList<CartItem>>();
        using var late = vm.Cart.Subscribe(seen.Add);

        Assert.Equal([new CartItem("p1", 1, 4_990)], Assert.Single(seen));
    }

    [Fact]
    public void RecentSearches_liefert_die_LRU_Liste()
    {
        var subscriptions = new List<IDisposable>();
        var vm = new CartViewModel();
        var recent = Watch(vm.RecentSearches, subscriptions);

        vm.Search("Bremse");
        vm.Search("Kerze");
        vm.Search("Bremse");

        Assert.Equal(["Bremse", "Kerze"], recent[^1]);
        subscriptions.ForEach(s => s.Dispose());
    }
}
