namespace Exercises.RxNetWhy;

public class FeaturesTests
{
    [Fact]
    public async Task System_Reactive_is_installed_and_runs()
    {
        var values = await Observable.Range(1, 3).ToArray();
        Assert.Equal([1, 2, 3], values);
    }

    [Fact]
    public void You_have_noted_at_least_three_features() =>
        Assert.True(Features.All.Count >= 3, $"Only {Features.All.Count} of 3 features noted. Add the remaining ones to Features.All.");

    [Fact]
    public void Every_feature_has_a_name_needs_and_a_reason()
    {
        Assert.NotEmpty(Features.All);
        Assert.All(Features.All, feature =>
        {
            Assert.False(string.IsNullOrWhiteSpace(feature.Name));
            Assert.NotEmpty(feature.Needs);
            Assert.True(feature.Why.Trim().Length > 10, $"The reason for “{feature.Name}” is too short. Write one sentence on why it is a stream.");
        });
    }
}
