// Lesson rxnet-reactiveui: A look at ReactiveUI
//
// Task: Build the cart ViewModel with ReactiveObject, WhenAnyValue,
// ObservableAsPropertyHelper, and a ReactiveCommand, and test it without a UI.
//
// ReactiveUI 26 with System.Reactive comes from the package ReactiveUI.Reactive.
// The types live in two namespaces:
//  - ReactiveUI.Reactive: ReactiveObject, RaiseAndSetIfChanged, ReactiveCommand
//  - ReactiveUI.Binding.Reactive: WhenAnyValue, ToProperty, ObservableAsPropertyHelper

using ReactiveUI.Binding.Reactive;
using ReactiveUI.Reactive;

namespace Exercises.RxNetReactiveUi;

public sealed class CheckoutViewModel : ReactiveObject
{
    // TODO: Field for Items (empty list) and an ObservableAsPropertyHelper<int> for TotalCents

    public CheckoutViewModel(Func<IReadOnlyList<CartItem>, IObservable<string>> placeOrder, IScheduler scheduler)
    {
        // TODO: TotalCents: this.WhenAnyValue(x => x.Items).Select(Cart.TotalCents).ToProperty(this, x => x.TotalCents, scheduler)
        // TODO: PlaceOrder: ReactiveCommand.CreateFromObservable(() => placeOrder(Items), canPlace, scheduler)
        //       canPlace is a stream: Items not empty
        throw new NotImplementedException("TODO: CheckoutViewModel");
    }

    public IReadOnlyList<CartItem> Items
    {
        get => throw new NotImplementedException("TODO: read Items");
        set => throw new NotImplementedException("TODO: set Items with this.RaiseAndSetIfChanged");
    }

    public int TotalCents => throw new NotImplementedException("TODO: TotalCents from the ObservableAsPropertyHelper");

    public ReactiveCommand<Unit, string> PlaceOrder => throw new NotImplementedException("TODO: PlaceOrder");

    public void Add(Product product, int quantity = 1) =>
        Items = [.. Items, new CartItem(product.Id, quantity, product.PriceCents)];
}
