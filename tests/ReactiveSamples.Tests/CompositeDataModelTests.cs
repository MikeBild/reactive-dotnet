using System.Reactive.Linq;
using System.Text;
using ReactiveSamples.Composite;

namespace ReactiveSamples.Tests;

public class CompositeDataModelTests
{
    // Daten aus RxSamples/composite_data_model (posts.json, comments.json).
    private const string PostsJson = """
        [{"postId":1,"title":"first title","body":"first body","authorId":1},
         {"postId":2,"title":"second title","body":"second body","authorId":1},
         {"postId":3,"title":"third title","body":"third body","authorId":3}]
        """;

    private const string CommentsJson = """
        [{"commentId":1,"postId":1,"body":"first comment"},
         {"commentId":2,"postId":1,"body":"second comment"},
         {"commentId":3,"postId":2,"body":"first comment"}]
        """;

    private static Func<Task<Stream>> Open(string json) => () => Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(json)));
    private static Func<Task<Stream>> Missing => () => Task.FromException<Stream>(new FileNotFoundException("comments.json"));

    [Fact]
    public async Task Merges_posts_and_comments_into_one_view_per_post()
    {
        var views = await CompositeDataModel.Compose(
                CompositeDataModel.FromJson<PostCreated>(Open(PostsJson)),
                CompositeDataModel.FromJson<CommentAdded>(Open(CommentsJson)))
            .ToList();

        var byId = views.OrderBy(v => v.PostId).ToList();
        Assert.Equal([2, 1, 0], byId.Select(v => v.CommentCount));
        Assert.Equal("first title", byId[0].Title);
        Assert.Equal(["first comment", "second comment"], byId[0].Comments);
    }

    [Fact]
    public async Task A_failing_source_degrades_to_empty_instead_of_failing_everything()
    {
        var views = await CompositeDataModel.Compose(
                CompositeDataModel.FromJson<PostCreated>(Open(PostsJson)),
                CompositeDataModel.FromJson<CommentAdded>(Missing))
            .ToList();

        Assert.Equal(3, views.Count);
        Assert.All(views, v => Assert.Equal(0, v.CommentCount));
    }
}
