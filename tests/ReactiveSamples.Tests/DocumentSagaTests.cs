using ReactiveSamples.MessageBus;
using ReactiveSamples.Saga;

namespace ReactiveSamples.Tests;

public class DocumentSagaTests
{
    [Fact]
    public void Document_travels_through_all_processors()
    {
        using var bus = new RxMessageBus();
        var (completed, wiring) = DocumentSaga.Wire(bus);
        var done = new List<ChaptersDocument>();
        using var _ = completed.Subscribe(done.Add);

        bus.Send(ChaptersDocument.New("Thank you"));
        bus.Send(ChaptersDocument.New("Thanx"));
        wiring.Dispose();

        Assert.Equal(["Thank you", "Thanx"], done.Select(d => d.ThankYouMessage));
        Assert.All(done, d =>
        {
            Assert.Equal(Bookmark.Done, d.Bookmark);
            Assert.Equal("Filled", d.Chapter1);
            Assert.Equal("Filled", d.Chapter2);
        });
    }

    [Fact]
    public void Steps_do_not_mutate_the_original_message()
    {
        using var bus = new RxMessageBus();
        var (_, wiring) = DocumentSaga.Wire(bus);
        var original = ChaptersDocument.New("Danke");

        bus.Send(original);
        wiring.Dispose();

        Assert.Equal(Bookmark.Chapter1, original.Bookmark);
        Assert.Equal("Empty", original.Chapter1);
    }
}
