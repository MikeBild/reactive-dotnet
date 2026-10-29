namespace Exercises.RxNetWhy;

public class FeaturesTests
{
    [Fact]
    public async Task System_Reactive_ist_installiert_und_laeuft()
    {
        var values = await Observable.Range(1, 3).ToArray();
        Assert.Equal([1, 2, 3], values);
    }

    [Fact]
    public void Du_hast_mindestens_drei_Features_notiert() =>
        Assert.True(Features.All.Count >= 3, $"Erst {Features.All.Count} Features notiert.");

    [Fact]
    public void Jedes_Feature_hat_Namen_Bedarf_und_Begruendung()
    {
        Assert.NotEmpty(Features.All);
        Assert.All(Features.All, feature =>
        {
            Assert.False(string.IsNullOrWhiteSpace(feature.Name));
            Assert.NotEmpty(feature.Needs);
            Assert.True(feature.Why.Trim().Length > 10, $"Begründung für „{feature.Name}“ ist zu kurz.");
        });
    }
}
