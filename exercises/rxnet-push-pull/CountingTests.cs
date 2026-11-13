namespace Exercises.RxNetPushPull;

public class CountingTests
{
    [Fact]
    public void Pull_the_iterator_computes_only_when_the_consumer_calls_MoveNext()
    {
        var produced = new List<int>();
        using var numbers = Counting.CountTo(10, produced.Add).GetEnumerator();

        Assert.Empty(produced);
        Assert.True(numbers.MoveNext());
        Assert.Equal(1, numbers.Current);
        Assert.True(numbers.MoveNext());
        Assert.Equal(2, numbers.Current);
        Assert.Equal([1, 2], produced);
    }

    [Fact]
    public void Pull_the_iterator_yields_one_to_ten() =>
        Assert.Equal(Enumerable.Range(1, 10), Counting.CountTo(10));

    [Fact]
    public void Push_without_Subscribe_no_value_is_produced()
    {
        var produced = new List<int>();
        Counting.CountToObservable(10, produced.Add);
        Assert.Empty(produced);
    }

    [Fact]
    public void Push_after_Subscribe_the_producer_sends_all_values_and_OnCompleted()
    {
        var received = new List<int>();
        var completed = false;

        using var subscription = Counting.CountToObservable(10)
            .Subscribe(received.Add, () => completed = true);

        Assert.Equal(Enumerable.Range(1, 10), received);
        Assert.True(completed);
    }

    [Fact]
    public void Push_the_producer_sets_the_pace_not_the_consumer()
    {
        var order = new List<string>();
        using var subscription = Counting.CountToObservable(3, n => order.Add($"produced {n}"))
            .Subscribe(n => order.Add($"received {n}"));

        Assert.Equal(
            ["produced 1", "received 1", "produced 2", "received 2", "produced 3", "received 3"],
            order);
    }
}
