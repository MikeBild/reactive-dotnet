using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text.Json;

namespace ReactiveSamples.Composite;

/// <summary>
/// C#-Portierung von RxSamples/composite_data_model (RxJS, 2013, "SOA micro services – composite data model"):
/// Zwei Quellen (Posts, Kommentare) werden gemischt (Merge), nach postId gruppiert (GroupBy)
/// und pro Gruppe zu einem zusammengesetzten Lesemodell gefaltet (Aggregate; im Original select + takeLast(1)).
/// Fällt eine Quelle aus, liefert sie einfach nichts (Catch → Empty; im Original catchException).
///
/// Modern: typisierte Events als records statt {type, data}-Objekten, Pattern-Matching statt
/// Funktions-Lookup per String, unveränderlicher Zustand statt state.comments.push.
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

    /// <summary>Lädt eine JSON-Quelle als Observable; Fehler (z. B. Datei fehlt) → leerer Strom.</summary>
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
