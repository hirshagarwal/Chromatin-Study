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
        //   new Color(255/255f,255/255f,229/255f),
        //   new Color(255/255f,247/255f,188/255f),
        //   new Color(254/255f,227/255f,145/255f),
        //   new Color(254/255f,196/255f,79/255f),
        //   new Color(254/255f,153/255f,41/255f),
        //   new Color(236/255f,112/255f,20/255f),
        //   new Color(204/255f,76/255f,2/255f),
        //   new Color(153/255f,52/255f,4/255f),
        //   new Color(102/255f,37/255f,6/255f),
        //new Color(250/255f,233/255f,205/255f),
        //new Color(1f,1f,1f),
        //new Color(2405f,191/255f,111/255f),
        //new Color(225/255f,142/255f,60/255f),
        //new Color(209/255f,87/255f,49/255f),
        //new Color(194/255f,45/255f,40/255f),
        //new Color(153/255f,30/255f,30/255f),
        //new Color(87/255f,13/255f,16/255f),
        //new Color(0f,0f,0f),
new Color(1f,1f,1f),
new Color(252/255f, 228/255f, 189/255f),
new Color(251/255f, 224/255f, 178/255f),
new Color(250/255f, 214/255f, 151/255f),
new Color(249/255f, 202/255f, 122/255f),
new Color(248/255f, 187/255f, 87/255f),
new Color(246/255f, 176/255f, 62/255f),
new Color(244/255f, 162/255f, 36/255f),
new Color(239/255f, 141/255f, 34/255f),
new Color(234/255f, 112/255f, 31/255f),
new Color(227/255f, 85/255f, 31/255f),
new Color(221/255f, 56/255f, 31/255f),
new Color(215/255f, 34/255f, 30/255f),
new Color(209/255f, 5/255f, 28/255f),
new Color(186/255f, 2/255f, 24/255f),
new Color(152/255f, 0/255f, 20/255f),
new Color(120/255f, 1/255f, 16/255f),
new Color(83/255f, 1/255f, 10/255f),
new Color(64/255f, 2/255f, 8/255f),
new Color(37/255f, 0/255f, 5/255f),
new Color(17/255f, 0/255f, 2/255f),
    };

    public static List<Color> taskColors = new List<Color>
    {
        new Color(1f,0f,0f),
        new Color(.2f, .2f, 1f)
    };

    public static Color GetClosestColor(float c, bool task)
    {
        if (task)
        {
            int index = (int)Math.Floor(c * (taskColors.Count - 1));
            return taskColors[index];
        }
        if (c > 1f)
        {
            Debug.LogWarning(c + " is >1 in Design.GetClosestColor()! Defaulting to 1...");
            return standardColors[standardColors.Count - 1];
        }
        int idx = (int)Math.Floor(c * (standardColors.Count - 1));
        return standardColors[idx];
        // return new Color(0f, 0f, 0f, c);
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