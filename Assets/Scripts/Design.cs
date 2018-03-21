using System;
using System.Collections.Generic;
using UnityEngine;

public enum Formats
{
    HoloLens, Projection, Heatmap, Vive
}

public enum Tasks
{
    PointDistance, SegmentDistance, CurveComparison, AttributeUnderstanding, TouchingSegments,
    LargerTad, Triple
}

public class Design
{
    public static List<Color> standardColors = new List<Color>{
        new Color(165/255f,0/255f,38/255f),
        new Color(215/255f,48/255f,39/255f),
        new Color(244/255f,109/255f,67/255f),
        new Color(253/255f,174/255f,97/255f),
        new Color(254/255f,224/255f,144/255f),
        new Color(255/255f,255/255f,191/255f),
        new Color(224/255f,243/255f,248/255f),
        new Color(171/255f,217/255f,233/255f),
        new Color(116/255f,173/255f,209/255f),
        new Color(69/255f,117/255f,180/255f),
        new Color(49/255f,54/255f,149/255f)
    };

    public static Color GetClosestColor(int c)
    {
        int idx = (int)Math.Floor(c / 255f);
        return standardColors[idx];
    }

    public static Color GetClosestColor(float c)
    {
        if (c > 1f)
        {
            Debug.LogWarning(c + " is >1 in Design.GetClosestColor()! Defaulting to 1...");
            return standardColors[standardColors.Count - 1];
        }
        int idx = (int)Math.Floor(c * (standardColors.Count - 1));
        return standardColors[idx];
    }

    public static string TASK_DESCRIPTION_TRIPLE =
        @"NEW TASK: Touching Triples

You will be shown a white curve, where some segments have been coloured red or blue.
Your task is to count the number of double loops in the curve, where the start and the end of the structure are coloured red and blue.
Not every red or blue section will belong to one of these structures.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_LARGER_TAD =
        @"NEW TASK: Larger TAD

You will be shown a curve which is segmented into multiple distinct sections.
One section will start with a blue tip, and one section will start with a red tip.
Of these two sections, your task is to identify the larger one.

Call the instructor before continuing.";

    public static string TASK_DESCRIPTION_TOUCHING_SEGMENTS =
        @"NEW TASK: Touching Segments

You will be shown a white curve, where some segments have been coloured red or blue.
Your task is to count the number of sections where the curve loops, and the start and end of the structure are coloured red and blue.
Not every red or blue section will belong to one of these structures.

Call the instructor before continuing.";

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