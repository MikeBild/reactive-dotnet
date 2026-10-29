// Lektion rxnet-testing: Zeit testen ohne zu warten
//
// Aufgabe: Schreib xUnit-Tests für die fertige Produktsuche mit TestScheduler und
// virtueller Zeit: Throttle, Duplikate, Abbruch und Fehlerfall.
//
// Die Suche steht in support/ProductSearch.cs. Hier ist der Test selbst die Aufgabe:
// Der erste Test zeigt das Muster, die übrigen sind übersprungen. Entferne jeweils
// Skip, schreib den Test und lass ihn laufen:
//
//   dotnet test exercises/Exercises.Start.Tests --filter "FullyQualifiedName~RxNetTesting"
//
// Merkhilfe für den TestScheduler:
//  - Ein Tick sind 100 Nanosekunden, eine Millisekunde sind 10.000 Ticks (Ms unten).
//  - CreateHotObservable(OnNext(zeit, wert), …) läuft unabhängig vom Subscribe (absolute Zeit).
//  - CreateColdObservable(OnNext(zeit, wert), …) startet pro Subscribe neu (relative Zeit).
//  - Start(create, created, subscribed, disposed) gibt einen Observer mit Messages zurück.
//    Ohne Angaben abonniert Start bei Tick 200 und kündigt bei Tick 1000, also nach 0,1 ms.
//    Gib deshalb immer explizite Zeiten an.

namespace Exercises.RxNetTesting;

public class ProductSearchTests : ReactiveTest
{
    private const long Ms = TimeSpan.TicksPerMillisecond;

    [Fact]
    public void Interval_liefert_fuenf_Werte_im_Sekundentakt_ohne_echte_Wartezeit()
    {
        var scheduler = new TestScheduler();

        var observer = scheduler.Start(
            () => Observable.Interval(TimeSpan.FromSeconds(1), scheduler).Take(5),
            created: 0, subscribed: 0, disposed: 10_000 * Ms);

        Assert.Equal(
            [
                OnNext(1_000 * Ms + 1, 0L), OnNext(2_000 * Ms + 1, 1L), OnNext(3_000 * Ms + 1, 2L),
                OnNext(4_000 * Ms + 1, 3L), OnNext(5_000 * Ms + 1, 4L), OnCompleted<long>(5_000 * Ms + 1),
            ],
            observer.Messages);
    }

    [Fact(Skip = "TODO: Throttle testen")]
    public void Die_Suche_wartet_300_ms_Ruhe_ab_und_sucht_nur_den_letzten_Begriff()
    {
        // TODO: Hot Observable mit "B" bei 10 ms und "Brems" bei 20 ms,
        //       search als Cold Observable, das nach 1 ms ["Bremsscheibe"] liefert.
        //       Erwartung: genau eine Suche nach "Brems", Ergebnis bei 321 ms.
    }

    [Fact(Skip = "TODO: Duplikate testen")]
    public void Derselbe_Begriff_zweimal_hintereinander_loest_keine_zweite_Suche_aus()
    {
        // TODO: "Kerze" bei 10 ms und noch einmal bei 500 ms, Suchen zählen.
    }

    [Fact(Skip = "TODO: Abbruch testen")]
    public void Ein_neuer_Begriff_bricht_die_laufende_Suche_ab()
    {
        // TODO: "Brems" mit langsamer Suche (500 ms), "Zünd" 400 ms später mit schneller Suche.
        //       Prüf mit slow.Subscriptions, dass die langsame Suche abgemeldet wurde.
    }

    [Fact(Skip = "TODO: Fehlerfall testen")]
    public void Ein_Fehler_der_Suche_wird_zum_Fehlerzustand_und_die_naechste_Suche_laeuft()
    {
        // TODO: Die Suche nach "kaputt" meldet OnError, die nach "Kerze" liefert ein Ergebnis.
        //       Erwartung: zuerst SearchFailed, danach SearchOk.
    }
}
