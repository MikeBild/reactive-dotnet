using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text.Json;

namespace ReactiveSamples.Composite;

/// <summary>
/// C# port of RxSamples/composite_data_model (RxJS, 2013, "SOA micro services – composite data model"):
/// Two sources (posts, comments) are merged (Merge), grouped by postId (GroupBy),
/// and folded per group into a composite read model (Aggregate; in the original select + takeLast(1)).
/// If a source fails, it just delivers nothing (Catch → Empty; in the original catchException).
///
/// Modern: typed events as records instead of {type, data} objects, pattern matching instead of
/// function lookup by string, immutable state instead of state.comments.push.
/// </summary>
public abstract record SourceEvent(int PostId);
public sealed record PostCreated(int PostId, string Title, string Body, int AuthorId) : SourceEvent(PostId);
public sealed record CommentAdded(int CommentId, int PostId, string Body) : SourceEvent(PostId);

public sealed record PostView(int PostId, string? Title, string? Body, ImmutableList<string> Comments)
{
    public int CommentCount => Comments.Count;

    public static PostView Empty(int postId) => new(postId, null, null, []);

    public PostView Apply(SourceEvent e) => e switch
    {
        PostCreated p => this with { Title = p.Title, Body = p.Body },
        CommentAdded c => this with { Comments = Comments.Add(c.Body) },
        _ => this,
    };
}

public static class CompositeDataModel
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    /// <summary>Loads a JSON source as an Observable; errors (e.g. missing file) → empty stream.</summary>
    public static IObservable<T> FromJson<T>(Func<Task<Stream>> open) =>
        Observable
            .FromAsync(async ct =>
            {
                await using var stream = await open();
                return await JsonSerializer.DeserializeAsync<T[]>(stream, Json, ct) ?? [];
            })
            .SelectMany(items => items)
            .Catch(Observable.Empty<T>());

    public static IObservable<PostView> Compose(IObservable<PostCreated> posts, IObservable<CommentAdded> comments) =>
        posts.Cast<SourceEvent>()
            .Merge(comments)
            .GroupBy(e => e.PostId)
            .SelectMany(group => group.Aggregate(PostView.Empty(group.Key), (view, e) => view.Apply(e)));
}
