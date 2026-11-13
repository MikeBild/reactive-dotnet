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
    public void Without_Subscribe_no_request_runs()
    {
        var counter = new Counter();
        CustomerStream.Cold(counter.Load, counter.OnRequest, new TestScheduler());
        Assert.Equal(0, counter.Loads);
    }

    [Fact]
    public void Cold_two_subscribers_start_two_requests()
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
    public void Shared_two_concurrent_subscribers_share_one_request()
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
    public void Shared_after_completion_a_late_subscriber_only_gets_OnCompleted()
    {
        // Unlike share() in RxJS, Publish().RefCount() does not restart after OnCompleted:
        // The Subject behind Publish has completed, late subscribers only get OnCompleted.
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
