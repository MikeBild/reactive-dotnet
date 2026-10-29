using System.Reactive.Linq;
using ReactiveSamples.FizzBuzz;

namespace ReactiveSamples.Tests;

public class FizzBuzzTests
{
    [Theory]
    [InlineData(1, "1")]
    [InlineData(3, "Fizz")]
    [InlineData(5, "Buzz")]
    [InlineData(15, "FizzBuzz")]
    [InlineData(98, "98")]
    public void Maps_numbers(int number, string expected) => Assert.Equal(expected, number.ToFizzBuzz());

    [Fact]
    public async Task Operator_over_observable()
    {
        var result = await Observable.Range(1, 15).FizzBuzz().ToArray();
        Assert.Equal("FizzBuzz", result[^1]);
        Assert.Equal(["1", "2", "Fizz", "4", "Buzz"], result[..5]);
    }

    [Fact]
    public async Task Same_logic_over_IAsyncEnumerable()
    {
        var pushed = await Observable.Range(1, 30).FizzBuzz().ToArray();
        var pulled = await AsyncEnumerable.Range(1, 30).FizzBuzz().ToArrayAsync();
        Assert.Equal(pushed, pulled);
    }

    [Fact]
    public void Publish_Connect_multicasts_one_execution_to_two_observers()
    {
        var executions = 0;
        var source = Observable.Defer(() => { executions++; return Observable.Range(1, 100); });
        var console = new List<string>();
        var debug = new List<string>();

        var connectable = source.FizzBuzz().Publish();
        connectable.Subscribe(console.Add);
        connectable.Subscribe(debug.Add);
        Assert.Empty(console); // bis Connect passiert nichts

        using (connectable.Connect())
        {
            Assert.Equal(1, executions);
            Assert.Equal(100, console.Count);
            Assert.Equal(console, debug);
        }
    }
}
