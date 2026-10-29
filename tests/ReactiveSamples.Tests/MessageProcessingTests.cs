using System.Reactive.Linq;
using ReactiveSamples.MessageProcessing;

namespace ReactiveSamples.Tests;

public class MessageProcessingTests
{
    // Exakt die sieben Eingaben aus dem Gist von 2012.
    private static readonly MessagePair[] Inputs =
    [
        new(new OrderMessage("Message1"), new InvoiceMessage("Message1")),
        new(new OrderMessage("Message2"), new InvoiceMessage("Message2")),
        new(new OrderMessage("Message2"), new InvoiceMessage("Message2")),
        new(new OrderMessage("Message1"), new InvoiceMessage("Message1")),
        new(new OrderMessage("Message2"), new InvoiceMessage("Message1")),
        new(new OrderMessage("Message2"), new InvoiceMessage("Message1")),
        new(new InvoiceMessage("Message3"), new InvoiceMessage("Message3")),
    ];

    [Fact]
    public async Task Characterization_of_the_original_either_rule()
    {
        // Wichtig: DistinctUntilChanged vergleicht immer mit dem zuletzt EMITTIERTEN Wert.
        var result = await Inputs.ToObservable().DistinctByEitherData().Select(p => p.ToString()).ToArray();
        Assert.Equal(["Message1:Message1", "Message2:Message2", "Message1:Message1", "Message3:Message3"], result);
    }

    [Fact]
    public async Task Key_selector_variant()
    {
        var result = await Inputs.ToObservable().DistinctByFirst().Select(p => p.ToString()).ToArray();
        Assert.Equal(["Message1:Message1", "Message2:Message2", "Message1:Message1", "Message2:Message1", "Message3:Message3"], result);
    }

    [Fact]
    public void Records_bring_value_equality_for_free()
    {
        Assert.Equal(new OrderMessage("x", "1"), new OrderMessage("x", "1"));
        Assert.NotEqual<IMessage>(new OrderMessage("x"), new InvoiceMessage("x"));
    }
}
