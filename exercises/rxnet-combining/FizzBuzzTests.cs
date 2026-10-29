namespace Exercises.RxNetCombining;

public class FizzBuzzTests
{
    [Fact]
    public async Task Cycle_wiederholt_die_Werte_endlos()
    {
        var values = await FizzBuzz.Cycle("a", "b").Take(5).ToArray();
        Assert.Equal(["a", "b", "a", "b", "a"], values);
    }

    [Fact]
    public async Task Die_ersten_fuenfzehn_Werte_stimmen()
    {
        var values = await FizzBuzz.Create(15).ToArray();
        Assert.Equal(
            ["1", "2", "Fizz", "4", "Buzz", "Fizz", "7", "8", "Fizz", "Buzz", "11", "Fizz", "13", "14", "FizzBuzz"],
            values);
    }

    [Fact]
    public async Task Liefert_hundert_Werte_und_endet()
    {
        var values = await FizzBuzz.Create().ToArray();
        Assert.Equal(100, values.Length);
        Assert.Equal("Buzz", values[^1]);
    }
}
