# reactive-dotnet

Mike Bilds Rx.NET-Beispiele aus den Jahren 2010 bis 2013, auf .NET 10 und System.Reactive 7 gebracht. Das Repo ist die Grundlage für die .NET-Variante des Workshops „Reactive Programming“ und enthält die Übungen zum Workshop „Reactive Programming mit Rx.NET“, siehe [Übungen](#übungen-zum-workshop-reactive-programming-mit-rxnet).

Jedes Beispiel ist eine kleine Klasse in `src/ReactiveSamples/<Beispiel>/`. Dazu gibt es xUnit-Tests in `tests/ReactiveSamples.Tests/` und eine ausführbare Demo in `samples/ReactiveSamples.Cli`. Die Kommentare im Code beschreiben jeweils, was das Original machte, was daran heute problematisch ist und was sich geändert hat.

## Ausführen und testen

Voraussetzung ist das .NET SDK 10.

```bash
dotnet build
dotnet test   # Samples und Lösungen der Übungen

# Alle Demos auflisten
dotnet run --project samples/ReactiveSamples.Cli

# Eine Demo starten
dotnet run --project samples/ReactiveSamples.Cli -- sharedresource
```

Pakete:

- `System.Reactive` 7.0.0 in der Bibliothek
- `Microsoft.Reactive.Testing` 7.0.0 in den Tests, für `TestScheduler` und virtuelle Zeit
- Alles andere kommt aus dem BCL, auch `System.Threading.Channels` und das asynchrone LINQ für `IAsyncEnumerable`.

## Übungen zum Workshop „Reactive Programming mit Rx.NET“

Zu jeder Lektion des Workshops gibt es unter `exercises/<lektion>/` einen Startpunkt, eine Lösung und Tests. Die Reihenfolge folgt der Agenda, die Beispiele kommen aus dem Werkstatt-Shop (Produktsuche, Bestellstatus, Warenkorb). Zeit läuft in allen Tests über den `TestScheduler`, kein Test wartet echt.

### So arbeitest du

1. Öffne die Datei im Lektionsordner, zum Beispiel `exercises/rxnet-flattening/Flattening.cs`. Oben steht die Aufgabe, `TODO`-Kommentare markieren die Lücken.
2. Starte die Tests der Lektion. Der Filter ist der Namespace der Lektion:

   ```bash
   dotnet test exercises/Exercises.Start.Tests --filter "FullyQualifiedName~RxNetFlattening"
   ```

3. Füll die Lücken, bis alle Tests grün sind.
4. Hängst du fest, vergleich mit `exercises/<lektion>/solution/`.

**Rote Tests sind am Anfang gewollt.** Die Startpunkte werfen `NotImplementedException`, deshalb schlagen die Start-Tests fehl, solange du die Lücken nicht gefüllt hast. Ein paar Tests sind schon grün, etwa „System.Reactive ist installiert und läuft“. In `rxnet-testing` ist der Test selbst die Aufgabe: Dort sind die offenen Tests übersprungen, bis du `Skip` entfernst.

| Befehl | Was passiert |
| --- | --- |
| `dotnet test exercises/Exercises.Start.Tests` | Alle Kern-Lektionen gegen deine Startpunkte |
| `dotnet test exercises/Extras.Start.Tests` | Die Zusatz-Lektionen ReactiveUI und DynamicData gegen deine Startpunkte |
| `dotnet test exercises/Exercises.Start.Tests --filter "FullyQualifiedName~RxNetFlattening"` | Nur eine Lektion |
| `dotnet test` | Samples und alle Lösungen, alles grün (so prüft es auch die CI) |

### Lektionen

| Lektion | Ordner | Filter | Aufgabe |
| --- | --- | --- | --- |
| Wann lohnt sich reaktive Programmierung? | [`exercises/rxnet-why`](exercises/rxnet-why) | `RxNetWhy` | Schreib drei Features aus deinem Projekt auf, bei denen Werte über die Zeit eintreffen, und prüf jedes mit der Checkliste aus dem Handout. |
| Push statt Pull | [`exercises/rxnet-push-pull`](exercises/rxnet-push-pull) | `RxNetPushPull` | Schreib einen Iterator mit yield return für die Zahlen eins bis zehn und ein Observable mit denselben Werten. Vergleiche, wer jeweils den nächsten Wert anstößt. |
| Ohne Subscribe passiert nichts | [`exercises/rxnet-observable-contract`](exercises/rxnet-observable-contract) | `RxNetObservableContract` | Schreib mit Observable.Create einen Ticker, der auf einem IScheduler hochzählt, nach fünf Werten OnCompleted meldet und beim Dispose den Timer stoppt. |
| Fast alles wird zum Observable | [`exercises/rxnet-creation`](exercises/rxnet-creation) | `RxNetCreation` | Bau mit Creation-Methoden aus einem Event, einem Timer und einem Task einen gemeinsamen Log-Stream, in dem jede Zeile ihre Quelle als Präfix trägt. |
| Operatoren sind Extension Methods | [`exercises/rxnet-operators`](exercises/rxnet-operators) | `RxNetOperators` | Verarbeite Nachrichten zum Bestellstatus: nach Typ filtern, Duplikate mit DistinctUntilChanged weglassen und mit Scan zählen, als eigene Extension Methods. |
| Jede Subscription braucht ein Ende | [`exercises/rxnet-subscription-cleanup`](exercises/rxnet-subscription-cleanup) | `RxNetSubscriptionCleanup` | Bau eine Komponente mit drei Subscriptions auf Interval und ein Event und beende alle gemeinsam über ein CompositeDisposable und TakeUntil. |
| Streams kombinieren | [`exercises/rxnet-combining`](exercises/rxnet-combining) | `RxNetCombining` | Löse FizzBuzz reaktiv, indem du die Zahlen eins bis hundert mit zwei zyklischen Streams für Fizz und Buzz per Zip kombinierst. |
| Cold oder Hot | [`exercises/rxnet-cold-hot`](exercises/rxnet-cold-hot) | `RxNetColdHot` | Abonniere ein Observable aus Observable.Defer zweimal, zähle die Requests mit Do und teile die Ausführung danach mit Publish und RefCount. |
| Tasks und Observables | [`exercises/rxnet-tasks-async`](exercises/rxnet-tasks-async) | `RxNetTasksAsync` | Bind eine async-Methode mit CancellationToken per FromAsync ein, prüf im Test, dass Dispose den Abbruch auslöst, und hol das Ergebnis mit await zurück. |
| Flattening | [`exercises/rxnet-flattening`](exercises/rxnet-flattening) | `RxNetFlattening` | Bau Produktsuche, Autosave, Upload-Queue und Login-Button mit je einer Flattening-Strategie und begründe jede Wahl im Code-Kommentar. |
| Fehlerbehandlung | [`exercises/rxnet-error-handling`](exercises/rxnet-error-handling) | `RxNetErrorHandling` | Lade drei URLs mit SelectMany, gib jedem Request ein Timeout, wiederhole zweimal mit Backoff und liefere bei Fehlern einen Fehlerwert statt abzubrechen. |
| Subjects und Multicasting | [`exercises/rxnet-subjects-multicasting`](exercises/rxnet-subjects-multicasting) | `RxNetSubjectsMulticasting` | Teile eine simulierte WebSocket-Verbindung zwischen mehreren Subscribern, sodass sie nur einmal öffnet und beim letzten Dispose schließt. |
| Zeit und Backpressure | [`exercises/rxnet-time-backpressure`](exercises/rxnet-time-backpressure) | `RxNetTimeBackpressure` | Zeig aus einem schnellen Sensor-Stream einen gedrosselten Live-Wert mit Sample und einen Durchschnitt pro Zeitfenster mit Buffer an. |
| Zeit testen ohne zu warten | [`exercises/rxnet-testing`](exercises/rxnet-testing) | `RxNetTesting` | Schreib xUnit-Tests für die fertige Produktsuche mit TestScheduler und virtueller Zeit: Throttle, Duplikate, Abbruch und Fehlerfall. |
| UI-Zustand als Stream (Zusatz) | [`exercises/rxnet-ui-state`](exercises/rxnet-ui-state) | `RxNetUiState` | Bau ein ViewModel mit Aktionen als Stream, führe den Zustand mit Scan über reine Reducer und halte die letzten fünf Suchen als LRU-Liste. |
| Ausblick ReactiveUI (Zusatz) | [`exercises/rxnet-reactiveui`](exercises/rxnet-reactiveui) | `RxNetReactiveUi` | Bau das Warenkorb-ViewModel mit ReactiveObject, WhenAnyValue, ObservableAsPropertyHelper und einem ReactiveCommand und teste es ohne UI. |
| Ausblick DynamicData (Zusatz) | [`exercises/rxnet-dynamicdata`](exercises/rxnet-dynamicdata) | `RxNetDynamicData` | Halte die Produkte in einem SourceCache, filtere sie live nach Suchbegriff, sortiere nach Preis und binde das Ergebnis an eine ReadOnlyObservableCollection. |

Die Zusatz-Lektion „UI-Zustand als Stream“ braucht keine weiteren Pakete und läuft mit den Kern-Lektionen. Ihre Tests filterst du wie die anderen über `Exercises.Start.Tests`, die Tests zu ReactiveUI und DynamicData über `Extras.Start.Tests`.

### Aufbau

```text
exercises/
├── Shop/Shop.cs                  Werkstatt-Shop: Product, CartItem, Customer, OrderStatus
├── rxnet-flattening/
│   ├── Flattening.cs             Startpunkt mit Aufgabe und TODOs
│   ├── FlatteningTests.cs        Tests, dieselben für Startpunkt und Lösung
│   └── solution/Flattening.cs    Lösung mit Begründung im Kommentar
├── Exercises.Start(.Tests)/      Projekte für die Kern-Lektionen gegen deine Startpunkte
├── Exercises.Solution(.Tests)/   dieselben Tests gegen die Lösungen
└── Extras.*/                     dasselbe für ReactiveUI und DynamicData
```

Die Projektordner enthalten nur eine `.csproj`. Welche Dateien sie einbinden, steht in `exercises/Directory.Build.targets`. Manche Lektionen haben Hilfsdateien unter `support/`, die du nicht ändern musst, etwa `FakeSocketServer.cs` oder die fertige Produktsuche in `rxnet-testing`.

### Pakete der Übungen

- `System.Reactive` 7.0.0 und `Microsoft.Reactive.Testing` 7.0.0 wie in den Samples, xUnit 2.9.3 für die Tests.
- `ReactiveUI.Reactive` 26.0.1, nur in `Extras.*`: Die Lektion zeigt MVVM auf Rx mit `ReactiveObject`, `WhenAnyValue`, `ObservableAsPropertyHelper` und `ReactiveCommand`. Seit Version 26 hängt das Paket `ReactiveUI` nicht mehr von System.Reactive ab. Die Variante für System.Reactive heißt `ReactiveUI.Reactive`, ihre Typen liegen in `ReactiveUI.Reactive` und `ReactiveUI.Binding.Reactive`.
- `DynamicData` 9.5.0, nur in `Extras.*`: Die Lektion zeigt reaktive Listen mit `SourceCache`, `Filter` und `SortAndBind`.

ReactiveUI und DynamicData stehen in eigenen Projekten, weil `ReactiveUI.Reactive` einen Source Generator mitbringt, der eine zweite Extension Method `Switch` erzeugt. Dort ist `.Switch()` aus System.Reactive mehrdeutig, die Kern-Lektionen brauchen es aber.

### Fallstricke, die die Übungen zeigen

- `Publish().RefCount()` startet nach `OnCompleted` nicht neu wie `share()` in RxJS. Späte Subscriber bekommen nur `OnCompleted` (`rxnet-cold-hot`).
- `Observable.Zip(a, b, c, …)` mit drei Quellen endet bei zwei synchronen, endlosen Zyklen nicht. Paarweise mit `a.Zip(b, …).Zip(c, …)` klappt es (`rxnet-combining`).
- `StartWith([])` bindet an `params T[]` und liefert gar keinen Startwert. Für eine leere Liste als Startwert schreibst du `StartWith((IReadOnlyList<CartItem>)[])` (`rxnet-ui-state`).
- Ein `exhaustMap` gibt es in Rx.NET nicht, die Lektion `rxnet-flattening` baut es als Extension Method.

## Übersicht: alt → neu

| Beispiel (Ordner, Demo) | Quelle (Jahr) | Was modernisiert wurde |
|---|---|---|
| **ObserverBasics** (`observer`) | Gists [Rx_Observer_Observables](https://gist.github.com/MikeBild/568281), [Variante](https://gist.github.com/MikeBild/4368ce550a6cff6f62fb) (2010/11) | Der handgeschriebene `Producer` gab in `Subscribe` `this` zurück. Ein `Dispose` entfernte damit alle Observer, und `OnCompleted` wurde nie gesendet. Jetzt erledigt `Observable.Create` das. `OnError` gab „finished.“ aus, das ist korrigiert. `results.Run` aus Ix existiert nicht mehr. Tests decken cold und hot (`Publish`/`Connect`) ab. |
| **FizzBuzz** (`fizzbuzz`) | Gist [Rx_FizzBuzz](https://gist.github.com/MikeBild/570401) (2010) | Die if/else-Kaskade ist jetzt ein Switch-Ausdruck über ein Tupel. Der eigene Operator ist eine Extension-Method, und dieselbe Logik gibt es zusätzlich für `IAsyncEnumerable`. `await observable.ToArray()` ersetzt `Console.ReadLine`. |
| **CombineLatest** (`combinelatest`) | Gist [Rx_Subject_Example](https://gist.github.com/MikeBild/9c1b5cfb4f00915f7299) | Der Operator ist eine reine Funktion über zwei `IObservable`. Im Test ersetzt `CreateHotObservable` die beiden Subjects. ValueTuple statt `Tuple<int,string>`. |
| **VirtualTimeTests** (nur Tests) | Gist [SimpleRxTesting](https://gist.github.com/MikeBild/3947466) (2012) | `CollectionAssert.AreEqual` wurde zu xUnit-`Assert.Equal`. Es gibt `ReactiveTest.OnNext(ticks, value)`-Assertions mit exakten Zeitpunkten, die Falle mit `Start()` und Tick 1000 ist dokumentiert, und `FirstAsync().ToTask()` läuft in virtueller Zeit. |
| **MessageProcessing** (`messages`) | Gist [Rx_MessageProcessing](https://gist.github.com/MikeBild/0be350cdf6f53959e950) (2012) | `Tuple` und zwei fast gleiche Klassen sind jetzt `record`s. Der Reflection-Comparer mit `object ==` und inkonsistentem `GetHashCode` ist durch einen typsicheren Comparer ersetzt. Ein Charakterisierungstest hält die Originalregel fest („erste ODER zweite Nachricht gleich“). |
| **SharedResource** (`sharedresource`) | [RxSamples/LastOneMakesLightOff](https://github.com/MikeBild/RxSamples/tree/master/LastOneMakesLightOff) (2013), identisch mit Gist [thelastoneturnslightoff](https://gist.github.com/MikeBild/4055567) | Bleibt bei `Observable.Using` + `Publish().RefCount()`. Statt `Debug.WriteLine` und `Console.ReadLine` gibt es testbare Logs und einen `TestScheduler`. Die Tests prüfen: eine Ressource für alle Subscriber, Freigabe beim letzten `Dispose`, neue Ressource beim erneuten Abonnieren. |
| **ChannelAffinity** (`channels`) | Gist [last one close the door](https://gist.github.com/MikeBild/f58cd8f9c085a2899fa0) (2013) | `GroupBy` mit einem `EventLoopScheduler` pro Kanal bleibt. Neu: Der Scheduler wird beim Gruppenende entsorgt, im Original leckten die Threads. Das Busy-Wait auf einem nicht threadsicheren Zähler und `Environment.Exit` in `Dispose` sind ersetzt durch `ToList().Timeout().ToTask()`, also Abschluss mit `await` und Timeout. `Command` und `Channel` sind `record`s. |
| **BoundedCollection** (`bounded`) | Gist [LRUObservableCollection](https://gist.github.com/MikeBild/4066272) (2012) | Der ehrliche Name lautet FIFO-Puffer statt LRU. Fehler im Original: `Insert(0, x)` bei vollem Puffer entfernte das gerade eingefügte Element. Die Einfügereihenfolge wird jetzt auch bei `Remove` und beim Indexer mitgeführt. `FromEventPattern` macht `CollectionChanged` zum Observable. Es gibt zwei Rx-Alternativen ohne Mutation: `Scan` als Sliding Window und `Buffer(n, 1)`. |
| **Coins** (`coins`) | Gists [Coin Changer with LINQ](https://gist.github.com/MikeBild/6789264), [without shared state](https://gist.github.com/MikeBild/6791315) (2013) | `amount %= n` in der Query war ein Seiteneffekt, und das `Dictionary` diente als Akkumulator. Jetzt ist der Zustand ein unveränderlicher `record`, berechnet mit `Aggregate`/`Scan`. Derselbe Fold läuft über `IEnumerable` und `IObservable` (Dualität). |
| **MessageBus** (`bus`) | [ReactiveMVVM/Bus](https://github.com/MikeBild/ReactiveMVVM/tree/master/source/ReactiveMVVM/Bus) (2011) | Zwei Fehler sind behoben. Erstens beendete `catch → _subject.OnError(e)` den Bus für alle Abonnenten. Fehler sind jetzt isoliert und laufen als eigener `Errors`-Stream. Zweitens verlegte `SubscribeOn` die Zustellung nicht, gemeint war `ObserveOn`. Beide Fälle sind mit Tests belegt. Dazu kommen Marker-Interfaces und `record`-Nachrichten. |
| **Saga** (`saga`) | Gist [ZeroMqDocumentSaga](https://gist.github.com/MikeBild/1839871) (2011) | Routing Slip in-process auf dem `RxMessageBus` statt ZeroMQ und Minimod. Statt die Nachricht zu mutieren, entsteht mit `record` + `with` eine neue. Das Lesezeichen ist ein `enum` statt eines Strings. |
| **Composite** (`composite`) | [RxSamples/composite_data_model](https://github.com/MikeBild/RxSamples/tree/master/composite_data_model) (2013, ursprünglich RxJS) | Portierung nach C#: `Merge` → `GroupBy` → `Aggregate` pro Gruppe, im Original `takeLast(1)`. Fällt eine Quelle aus, liefert `Catch(Empty)` einen leeren Strom. Die Events sind typisiert (`record`s, Pattern-Matching) und das Lesemodell ist unveränderlich. |
| **Backpressure** (`backpressure`) | *neu* | Kein Original. Ergänzt, weil Backpressure im RxJS-Workshop ein Thema ist. Verlustbehaftet geht es mit `Sample`/`Buffer`, verlustfrei per Pull mit begrenztem `Channel<T>` → `IAsyncEnumerable`. Bricht der Consumer vorzeitig ab, wird der wartende Producer freigegeben. |

## Was heute anders gelöst wird

- **ReactiveMVVM (Silverlight, Autofac, `ViewModelField<T>`, `NotifyObject`, eigene Commands):** Davon ist nur der Bus portiert. Silverlight gibt es nicht mehr, der Service-Locator über einen statischen Autofac-Container gilt als Anti-Pattern, und die übrigen Bausteine liefern heute fertige Bibliotheken:
  - [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) erzeugt per Source-Generator `[ObservableProperty]` (statt `ViewModelField<T>`/`NotifyObject`), `[RelayCommand]` (statt `DelegateCommand`/`SendCommand`) und bringt mit `WeakReferenceMessenger` einen eigenen Messenger mit (statt `RxMessageBus`).
  - [ReactiveUI](https://www.reactiveui.net/) ist der direkte Nachfolger der Idee „MVVM auf Rx“. Dazu gehören `WhenAnyValue` (Property-Änderungen als `IObservable`), `ReactiveCommand` (Commands mit `CanExecute` als Observable, eingebauter Fehler- und Busy-Stream) und `MessageBus`.
  - DI läuft heute über `Microsoft.Extensions.DependencyInjection` mit Konstruktor-Injection statt `SilverlightAppHost.Container.Resolve<T>()`.
  - Rx 7 verlagert die UI-Anbindung in eigene Pakete: `System.Reactive.Wpf`, `System.Reactive.WindowsForms`, `System.Reactive.Uwp` (siehe [Rx-7-Release-Notes](https://github.com/dotnet/reactive/blob/main/Rx.NET/Documentation/ReleaseHistory/Rx.v7.md)). Für `ObserveOnDispatcher` brauchst du also eines dieser Pakete.
- **ZeroMQ-Saga:** Die Minimod-Pakete sind verwaist. Heute übernimmt das ein Message-Broker (RabbitMQ, NATS, Azure Service Bus, Kafka) mit einem Saga- bzw. Workflow-Framework (MassTransit, NServiceBus, Wolverine, Temporal). Dazu gehören Persistenz, Retries, Outbox und Idempotenz, die eine In-Memory-Lösung nicht bietet. Das Beispiel zeigt nur noch das Muster.
- **Subjects sparsam:** In den Originalen waren `Subject`s allgegenwärtig (Kommandostrom, Beispiele, Bus). Heute gilt: Ein Subject nur dort einsetzen, wo imperative Welt und Rx sich treffen, etwa im Bus, und nach außen nur `AsObservable()` zeigen. Für Tests nimmst du `CreateHotObservable`, für Events `FromEventPattern`, für Async `FromAsync`.
- **`Console.ReadLine`/`Thread.Sleep` als Synchronisation:** Ersetzt durch `await` auf Observables (`ToTask`, `FirstAsync`, `ToArray`) und in Tests durch virtuelle Zeit.
- **Echte Kanäle statt Rx:** Für reine Producer/Consumer-Warteschlangen mit Backpressure ist `System.Threading.Channels` heute oft die bessere Wahl. Rx bleibt stark beim Komponieren von Ereignissen über die Zeit.

## Bezug zu den RxJS-Konzepten des Workshops

| Konzept | RxJS | Rx.NET | Beispiel |
|---|---|---|---|
| Observable / Observer | `new Observable(sub => …)`, `subscribe({next, error, complete})` | `Observable.Create`, `IObserver<T>` (`OnNext`, `OnError`, `OnCompleted`), `Subscribe` liefert `IDisposable` | ObserverBasics |
| Erzeugen | `of`, `from`, `interval`, `fromEvent`, `defer` | `Return`, `ToObservable`, `Interval`, `FromEventPattern`, `Defer`, `FromAsync` | ObserverBasics, BoundedCollection, Composite |
| Transformieren | `map`, `filter`, `scan`, `reduce` | `Select`, `Where`, `Scan`, `Aggregate` (LINQ-Namen) | FizzBuzz, Coins |
| Höhere Ordnung | `mergeMap`, `groupBy`, `concatMap`, `switchMap` | `SelectMany`, `GroupBy`, `Select(...).Concat()`, `Select(...).Switch()` | ChannelAffinity, Composite |
| Kombinieren | `combineLatest`, `merge`, `zip` | `CombineLatest`, `Merge`, `Zip` | CombineLatest, Composite |
| Duplikate | `distinctUntilChanged`, `distinctUntilKeyChanged` | `DistinctUntilChanged(keySelector / comparer)` | MessageProcessing |
| Fehler | `catchError`, `retry` | `Catch`, `Retry`, `OnErrorResumeNext` | Composite, MessageBus |
| Ressourcen | `finalize`, `using` | `Finally`, `Observable.Using` | SharedResource, ChannelAffinity |
| Hot / Cold | `share()`, `shareReplay()`, `connectable()` | `Publish().RefCount()`, `Replay(1).RefCount()`, `Publish()` + `Connect()` | SharedResource, FizzBuzz, ObserverBasics |
| Subjects | `Subject`, `BehaviorSubject`, `ReplaySubject` | gleichnamig, sparsam einsetzen | MessageBus |
| Schedulers | `asyncScheduler`, `queueScheduler`, `observeOn`, `subscribeOn` | `TaskPoolScheduler`, `EventLoopScheduler` (eigener Thread), `CurrentThreadScheduler`, `ObserveOn`, `SubscribeOn` | ChannelAffinity, MessageBus, Backpressure |
| Zeit | `debounceTime`, `throttleTime`, `sampleTime`, `bufferTime` | **`Throttle` = `debounceTime`**, `Sample`, `Buffer(TimeSpan)` | Backpressure |
| Testing mit virtueller Zeit | `TestScheduler`, Marble-Tests `cold('-a-b|')` | `TestScheduler`, `CreateHot/ColdObservable`, `ReactiveTest.OnNext(ticks, v)` | VirtualTimeTests, CombineLatest, SharedResource, Backpressure |
| Promise/async-Interop | `firstValueFrom`, `lastValueFrom`, `from(promise)` | `FirstAsync().ToTask()`, `await observable` (letzter Wert), `FromAsync` | VirtualTimeTests, Composite |
| Backpressure | gibt es nicht: Push-only; lossy über `sample`/`throttle`/`buffer` | ebenso; verlustfrei per Pull mit `IAsyncEnumerable` + `Channel<T>` | Backpressure |

Fallstricke beim Umstieg:

- `Throttle` in Rx.NET ist RxJS' `debounceTime`. Ein echtes `throttleTime` gibt es nicht eingebaut, `Sample` kommt dem am nächsten.
- `await` auf ein `IObservable` liefert den **letzten** Wert, nicht den ersten.
- `TestScheduler.Start(create)` abonniert bei Tick 200 und kündigt bei Tick 1000. Ein Tick sind 100 ns, sekundenbasierte Operatoren liefern dort also nichts.
- `SubscribeOn` verlegt nur das Abonnieren. Für die Zustellung auf einem Thread (UI, Event-Loop) brauchst du `ObserveOn`.

## Bewusst weggelassen

- Gists ohne Rx-Bezug: Composite MVC4, MockHttpContext, TreeExtentions, ExpressionTranslator, DelegateAdjuster, DisplayNameByResourceKeyAttribute, CretaeManualleExpression, Code Kata PickAkin (reines LINQ), ECB_with_events (Event-Verdrahtung ohne Rx).
- Aus `RxSamples/composite_data_model` ist nur `index.html` portiert. Nicht portiert sind `index2.html`/`index3.html`. Sie verpacken dieselbe Pipeline in eine fluente `Projection`-Hülle mit `andUrl`/`nextUrl` und binden zusätzlich `authors.json` ein. Das Prinzip zeigt die Portierung bereits.
- Gist [ViewModelField](https://gist.github.com/MikeBild/1031650) und der Rest von ReactiveMVVM (Navigation, XAML-Trigger, Commands): Das ist Silverlight-spezifisch und heute durch CommunityToolkit.Mvvm oder ReactiveUI abgedeckt (siehe oben). Eine WPF-Portierung würde außerdem nicht plattformübergreifend bauen.
