using System;

public enum Formats
{
    HoloLens, Projection, Heatmap
}

public enum Tasks
{
    PointDistance, SegmentDistance, CurveComparison, AttributeUnderstanding
}

public class Design
{
    public static string TASK_DESCRIPTION_ATTRIBUTE =

@"NEW TASK: Attribute Understanding

You will see a chromosome colored in some way. You will be provided with a description of an area of the chromosome.
Your task is to identify the area described, and then to identify which color it is mostly comprised of.
Once you are done, press the trigger button and a menu will pop up.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_CURVE =
@"
    NEW TASK: Curve Distance Comparison

You will see three chromosomes: a white; red; and blue one.
Your task is to identify which of the red and blue chromosomes is most similar to the white one.
Once you are done, press the trigger button and a menu will pop up.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_SELECTION =
@"NEW TASK: Selection

You will see a scatterplot with some points colored red.
Your task is to select all these red points. A selected point turns green.
Once you are done, the program will automatically start the next trial.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_SEGMENT =
@"NEW TASK: Segment

You will see a scatterplot with two colored segments; a red segment and a blue segment.
Your task is to decide which of the segments is longer; the red or the blue.
Once you know the answer, press the trigger button and a menu will pop up.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_DISTANCE =
@"NEW TASK: Distance

You will see a scatterplot with two pairs of colored nodes; a red pair and a blue pair.
Your task is to decide which pair of points is coloser together; the red pair or the blue pair.
Once you know the answer, press the trigger button and a menu will pop up.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_CLUSTER =
@"NEW TASK: Clusters

You will see a scatterplot with points forming clusters, i.e. regions with close points in the scatterplot.
Your task is to observe the cube from each of the 3 different orthogonal sides. Some of the
clusters will overlap. Tell us the lowest number of clusters you could observe.
Once you know the answer, press the trigger button and a menu will pop up.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_CUTTINGPLANE =
@"NEW TASK: Cutting Plane

You will see a completely dense cube in which there are three clusters
of red points hidden. Position the cutting plane so that the most of the red
points are on the plane. If you are good, press the trigger button.

Call the instructor before continuing.";

    public static string TRAINING_END =
@"Looks good!

The next 10 trials will be recorded. Try to be as fast and precise as possible.
";

    public static string CSV_HEADER = "SUBJECT,TRIAL,TECHNIQUE,TASK,TRAINING,DATA, ANSWER, ERROR, TIME, TRACKING_MAIN, TRACKING_CURSOR, TRACKING_PLANE ";

    public static char LINE_BREAK = '#';
    public static int TRACKING_FRAME_RATE = 5;

    public static String text = "";
}