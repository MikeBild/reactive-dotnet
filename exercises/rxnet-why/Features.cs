// Lesson rxnet-why: When is reactive programming worth it?
//
// Task: Write down three features from your project in which values arrive over
// time, and check each one against the checklist from the handout.
//
// There is no Rx code here. The tests only check that the project builds,
// System.Reactive runs, and your list is complete.

namespace Exercises.RxNetWhy;

public static class Features
{
    // TODO: Add three features, for example:
    // new("Product search while typing", Source.Ui, [Need.Throttle, Need.Cancel],
    //     "Every keystroke is a new value, outdated requests must be canceled."),
    public static IReadOnlyList<StreamFeature> All { get; } = [];
}
