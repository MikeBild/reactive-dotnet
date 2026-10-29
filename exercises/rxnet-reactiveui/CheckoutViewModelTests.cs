using ReactiveUI.Reactive;

namespace Exercises.RxNetReactiveUi;

public class CheckoutViewModelTests
{
    private static readonly Product Brake = new("p1", "Bremsscheibe", 4_990, 12);
    private static readonly Product Plug = new("p2", "Zündkerze", 790, 40);

    [Fact]
    public void TotalCents_folgt_den_Positionen()
    {
        var scheduler = new TestScheduler();
        var vm = new CheckoutViewModel(_ => Observable.Return("o1"), scheduler);

        vm.Add(Brake);
        vm.Add(Plug, 4);
        scheduler.AdvanceBy(1);

        Assert.Equal(4_990 + 4 * 790, vm.TotalCents);
    }

    [Fact]
    public void TotalCents_meldet_PropertyChanged()
    {
        var scheduler = new TestScheduler();
        var vm = new CheckoutViewModel(_ => Observable.Return("o1"), scheduler);
        var changed = new List<string?>();
        _ = vm.TotalCents; // die Property einmal lesen, dann ist sie aktiv
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Add(Brake);
        scheduler.AdvanceBy(1);

        Assert.Contains(nameof(CheckoutViewModel.Items), changed);
        Assert.Contains(nameof(CheckoutViewModel.TotalCents), changed);
    }

    [Fact]
    public void PlaceOrder_ist_bei_leerem_Warenkorb_nicht_ausfuehrbar()
    {
        var scheduler = new TestScheduler();
        var vm = new CheckoutViewModel(_ => Observable.Return("o1"), scheduler);
        var canExecute = new List<bool>();
        using var subscription = vm.PlaceOrder.CanExecute.Subscribe(canExecute.Add);
        scheduler.AdvanceBy(1);

        vm.Add(Brake);
        scheduler.AdvanceBy(1);

        Assert.Equal([false, true], canExecute.Distinct());
    }

    [Fact]
    public void PlaceOrder_schickt_die_Positionen_und_liefert_die_Bestellnummer()
    {
        var scheduler = new TestScheduler();
        IReadOnlyList<CartItem>? sent = null;
        var vm = new CheckoutViewModel(items =>
        {
            sent = items;
            return Observable.Return("o42");
        }, scheduler);
        vm.Add(Brake);
        scheduler.AdvanceBy(1);

        var orderIds = new List<string>();
        using var results = vm.PlaceOrder.Subscribe(orderIds.Add);
        using var execution = vm.PlaceOrder.Execute().Subscribe(_ => { }, _ => { });
        scheduler.AdvanceBy(1);

        Assert.Equal(["o42"], orderIds);
        Assert.Equal([new CartItem("p1", 1, 4_990)], sent);
    }

    [Fact]
    public void Fehler_beim_Bestellen_landen_in_ThrownExceptions()
    {
        var scheduler = new TestScheduler();
        var vm = new CheckoutViewModel(_ => Observable.Throw<string>(new InvalidOperationException("Bestand reicht nicht")), scheduler);
        vm.Add(Brake);
        scheduler.AdvanceBy(1);

        var errors = new List<Exception>();
        using var subscription = vm.PlaceOrder.ThrownExceptions.Subscribe(errors.Add);
        using var execution = vm.PlaceOrder.Execute().Subscribe(_ => { }, _ => { });
        scheduler.AdvanceBy(1);

        Assert.Equal("Bestand reicht nicht", Assert.Single(errors).Message);
    }
}
