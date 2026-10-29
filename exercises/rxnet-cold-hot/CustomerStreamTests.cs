namespace Exercises.RxNetColdHot;

public class CustomerStreamTests
{
    private static readonly Customer Ada = new("c42", "Ada");

    private sealed class Counter
    {
        public int Loads;
        public int Requests;
        public Task<Customer> Load(CancellationToken _) { Loads++; return Task.FromResult(Ada); }
        public void OnRequest() => Requests++;
    }

    [Fact]
    public void Ohne_Subscribe_laeuft_kein_Request()
    {
        var counter = new Counter();
        CustomerStream.Cold(counter.Load, counter.OnRequest, new TestScheduler());
        Assert.Equal(0, counter.Loads);
    }

    [Fact]
    public void Cold_zwei_Subscriber_starten_zwei_Requests()
    {
        var scheduler = new TestScheduler();
        var counter = new Counter();
        var customer = CustomerStream.Cold(counter.Load, counter.OnRequest, scheduler);
        var header = new List<Customer>();
        var sidebar = new List<Customer>();

        using var a = customer.Subscribe(header.Add);
        using var b = customer.Subscribe(sidebar.Add);
        scheduler.AdvanceBy(1);

        Assert.Equal(2, counter.Loads);
        Assert.Equal(2, counter.Requests);
        Assert.Equal([Ada], header);
        Assert.Equal([Ada], sidebar);
    }

    [Fact]
    public void Shared_zwei_gleichzeitige_Subscriber_teilen_einen_Request()
    {
        var scheduler = new TestScheduler();
        var counter = new Counter();
        var customer = CustomerStream.Shared(counter.Load, counter.OnRequest, scheduler);
        var header = new List<Customer>();
        var sidebar = new List<Customer>();

        using var a = customer.Subscribe(header.Add);
        using var b = customer.Subscribe(sidebar.Add);
        scheduler.AdvanceBy(1);

        Assert.Equal(1, counter.Loads);
        Assert.Equal(1, counter.Requests);
        Assert.Equal([Ada], header);
        Assert.Equal([Ada], sidebar);
    }

    [Fact]
    public void Shared_nach_dem_Ende_bekommt_ein_spaeter_Subscriber_nur_OnCompleted()
    {
        // Anders als share() in RxJS startet Publish().RefCount() nach OnCompleted nicht neu:
        // Das Subject hinter Publish ist beendet, späte Subscriber bekommen nur noch OnCompleted.
        var scheduler = new TestScheduler();
        var counter = new Counter();
        var customer = CustomerStream.Shared(counter.Load, counter.OnRequest, scheduler);

        using var first = customer.Subscribe(_ => { });
        scheduler.AdvanceBy(1);

        var late = new List<Customer>();
        var lateCompleted = false;
        using var second = customer.Subscribe(late.Add, () => lateCompleted = true);
        scheduler.AdvanceBy(1);

        Assert.Equal(1, counter.Loads);
        Assert.Empty(late);
        Assert.True(lateCompleted);
    }
}
