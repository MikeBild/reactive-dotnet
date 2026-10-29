namespace Exercises.RxNetPushPull;

public class CountingTests
{
    [Fact]
    public void Pull_der_Iterator_rechnet_erst_wenn_der_Consumer_MoveNext_aufruft()
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
    public void Pull_der_Iterator_liefert_eins_bis_zehn() =>
        Assert.Equal(Enumerable.Range(1, 10), Counting.CountTo(10));

    [Fact]
    public void Push_ohne_Subscribe_entsteht_kein_Wert()
    {
        var produced = new List<int>();
        Counting.CountToObservable(10, produced.Add);
        Assert.Empty(produced);
    }

    [Fact]
    public void Push_nach_Subscribe_schickt_der_Producer_alle_Werte_und_OnCompleted()
    {
        var received = new List<int>();
        var completed = false;

        using var subscription = Counting.CountToObservable(10)
            .Subscribe(received.Add, () => completed = true);

        Assert.Equal(Enumerable.Range(1, 10), received);
        Assert.True(completed);
    }

    [Fact]
    public void Push_der_Producer_bestimmt_das_Tempo_nicht_der_Consumer()
    {
        var order = new List<string>();
        using var subscription = Counting.CountToObservable(3, n => order.Add($"produziert {n}"))
            .Subscribe(n => order.Add($"empfangen {n}"));

        Assert.Equal(
            ["produziert 1", "empfangen 1", "produziert 2", "empfangen 2", "produziert 3", "empfangen 3"],
            order);
    }
}
