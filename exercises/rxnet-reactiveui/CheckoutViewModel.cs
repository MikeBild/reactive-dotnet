// Lektion rxnet-reactiveui: Ausblick ReactiveUI
//
// Aufgabe: Bau das Warenkorb-ViewModel mit ReactiveObject, WhenAnyValue,
// ObservableAsPropertyHelper und einem ReactiveCommand und teste es ohne UI.
//
// ReactiveUI 26 mit System.Reactive kommt aus dem Paket ReactiveUI.Reactive.
// Die Typen liegen in zwei Namespaces:
//  - ReactiveUI.Reactive: ReactiveObject, RaiseAndSetIfChanged, ReactiveCommand
//  - ReactiveUI.Binding.Reactive: WhenAnyValue, ToProperty, ObservableAsPropertyHelper

using ReactiveUI.Binding.Reactive;
using ReactiveUI.Reactive;

namespace Exercises.RxNetReactiveUi;

public sealed class CheckoutViewModel : ReactiveObject
{
    // TODO: Feld für Items (leere Liste) und ein ObservableAsPropertyHelper<int> für TotalCents

    public CheckoutViewModel(Func<IReadOnlyList<CartItem>, IObservable<string>> placeOrder, IScheduler scheduler)
    {
        // TODO: TotalCents: this.WhenAnyValue(x => x.Items).Select(Cart.TotalCents).ToProperty(this, x => x.TotalCents, scheduler)
        // TODO: PlaceOrder: ReactiveCommand.CreateFromObservable(() => placeOrder(Items), canPlace, scheduler)
        //       canPlace ist ein Stream: Items nicht leer
        throw new NotImplementedException("TODO: CheckoutViewModel");
    }

    public IReadOnlyList<CartItem> Items
    {
        get => throw new NotImplementedException("TODO: Items lesen");
        set => throw new NotImplementedException("TODO: Items mit this.RaiseAndSetIfChanged setzen");
    }

    public int TotalCents => throw new NotImplementedException("TODO: TotalCents aus dem ObservableAsPropertyHelper");

    public ReactiveCommand<Unit, string> PlaceOrder => throw new NotImplementedException("TODO: PlaceOrder");

    public void Add(Product product, int quantity = 1) =>
        Items = [.. Items, new CartItem(product.Id, quantity, product.PriceCents)];
}
