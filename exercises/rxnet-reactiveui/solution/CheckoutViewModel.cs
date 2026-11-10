using ReactiveUI.Binding.Reactive;
using ReactiveUI.Reactive;

namespace Exercises.RxNetReactiveUi;

public sealed class CheckoutViewModel : ReactiveObject
{
    private IReadOnlyList<CartItem> _items = [];
    private readonly ObservableAsPropertyHelper<int> _totalCents;

    public CheckoutViewModel(Func<IReadOnlyList<CartItem>, IObservable<string>> placeOrder, IScheduler scheduler)
    {
        // WhenAnyValue turns PropertyChanged into an IObservable, ToProperty turns it back into a property.
        _totalCents = this
            .WhenAnyValue(x => x.Items)
            .Select(Cart.TotalCents)
            .ToProperty(this, x => x.TotalCents, scheduler);

        // CanExecute is a stream: The command is only active when the cart is not empty.
        var canPlace = this.WhenAnyValue(x => x.Items).Select(items => items.Count > 0);
        PlaceOrder = ReactiveCommand.CreateFromObservable(() => placeOrder(Items), canPlace, scheduler);
    }

    public IReadOnlyList<CartItem> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public int TotalCents => _totalCents.Value;

    public ReactiveCommand<Unit, string> PlaceOrder { get; }

    public void Add(Product product, int quantity = 1) =>
        Items = [.. Items, new CartItem(product.Id, quantity, product.PriceCents)];
}
