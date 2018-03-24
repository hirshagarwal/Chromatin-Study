using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Curve : MonoBehaviour
    {
        private const int randomness = 5;
        public static int currentFile = 13;
        public static Vector3 displacement = new Vector3(0, 0, 2);
        public static float scale = 4f;
        public List<Color> colorSpace;
        public AnimationCurve colorWidth;
        public GameObject go;
        public float splineRes = 8f;
        private Material baseMaterial;
        private string chrtype = "sen";
        private List<GameObject> cylinders = new List<GameObject>();
        private float cylinderWidth = 0.0015f;
        private string fileName;
        private string[] files = { "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X" };
        private List<Point> points;
        private float sphereWidth = 0.005f;

        public Curve(string filen, int skips, int redCount, bool triple = false)
        {
            go = new GameObject();
            List<Point> allPoints = ReadInFile(filen);
            points = new List<Point>();
            Color color = Design.GetClosestColor(0.5f,false);
            List<float> colorsIn = new List<float>();
            float maxColor = 0.0f;
            System.Random rnd = new System.Random(0);

            for (int i = 0; i < allPoints.Count; i += skips)
            {
                points.Add(allPoints[i]);
            }
            if (triple)
            {
                MutateCurveTriples(redCount, rnd);
            }
            else
            {
                MutateCurveTouchingPoints(redCount, rnd);

                //foreach (Point point in points)
                //{
                //    colorsIn.Add(point.Color);
                //    if (point.Color > maxColor)
                //        maxColor = point.Color;
                //}
                //colorSpace = BuildColorMap(colorsIn);
                //colorWidth = BuildColorCurve(colorsIn);
                //int stepsize = colorSpace.Count / points.Count;
                //for (int i = 0; i < points.Count; i++)
                //{
                //    int idx = (int)Math.Floor((points[i].Color / maxColor) * (colorSpace.Count - 1));
                //    points[i].ColorRGB = colorSpace[idx];
                //}
            }
            cylinders = new List<GameObject>();
            List<Connector> connectors = new List<Connector>();
            Point[] splinePoints = MakeSplines(points.ToArray());
            Point lastPoint = splinePoints[0];
            
            for (int i = 0; i < splinePoints.Length; i++)
            {
                connectors.Add(
                    new Connector(lastPoint.Displaced(displacement) / scale,
                    splinePoints[i].Displaced(displacement) / scale)
                    );
                cylinders.Add(BuildConnector(connectors[connectors.Count - 1]));
                lastPoint = splinePoints[i];
            }
        }

        private void MutateCurveTouchingPoints(int redCount, System.Random rnd)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].ColorRGB = Design.GetClosestColor(0.5f,false);
            }
            int currentReds = 0;
            splineRes *= 2;
            while (currentReds < redCount)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    int r = rnd.Next(25);
                    if (r == 2)
                    {
                        if (
                            !points[i].HasColor() &&
                            i > 0 &&
                            i < points.Count - 2 &&
                            !points[i - 1].HasColor() &&
                            !points[i + 1].HasColor()
                            )
                        {
                            if (currentReds < redCount)
                            {
                                points[i] = points[i].Displaced(new Vector3(rnd.Next(randomness) / 10f, rnd.Next(randomness) / 10f, rnd.Next(randomness) / 10f));
                            }
                            points[i - 1].MakeRed();
                            points[i - 1].ColorRGB = Design.GetClosestColor(0f,true);
                            points[i + 1].MakeBlue();
                            points[i + 1].ColorRGB = Design.GetClosestColor(1f,true);
                            currentReds++;
                        }
                    }
                }
            }
        }

        private void MutateCurveTriples(int redCount, System.Random rnd)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].ColorRGB = Design.GetClosestColor(0.5f,false);
            }
            int currentReds = 0;
            splineRes *= 2;
            while (currentReds < redCount)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    int r = rnd.Next(25);
                    if (r == 2)
                    {
                        if (
                            !points[i].HasColor() &&
                            i > 0 &&
                            i < points.Count - 2 &&
                            !points[i - 1].HasColor() &&
                            !points[i + 1].HasColor()
                            )
                        {
                            Point startPoint = points[i];
                            Point leftPoint = points[i - 1];
                            Point rightPoint = points[i + 1];
                            Point displacePoint = startPoint.Displaced(new Vector3(rnd.Next(randomness) / 10f, rnd.Next(randomness) / 10f, rnd.Next(randomness) / 10f));
                            Point leftInterpolated = leftPoint.Interpolate(displacePoint);
                            Point rightInterpolated = rightPoint.Interpolate(displacePoint);
                            Point leftBegin = startPoint.Displaced(new Vector3(rnd.Next(randomness) / 100f, rnd.Next(randomness) / 100f, rnd.Next(randomness) / 100f));
                            Point rightBegin = startPoint.Displaced(new Vector3(rnd.Next(randomness) / 100f, rnd.Next(randomness) / 100f, rnd.Next(randomness) / 100f));

                            if (currentReds < redCount)
                            {
                                points.Insert(i, leftInterpolated);
                                points.Insert(i + 2, rightInterpolated);
                                points.Insert(i - 1, leftBegin);
                                points.Insert(i + 5, rightBegin);

                                points[i - 1].MakeBlue();
                                points[i - 1].ColorRGB = Design.GetClosestColor(0f,true);
                                points[i + 2].MakeYellow();
                                points[i + 5].MakeRed();
                                points[i + 5].ColorRGB = Design.GetClosestColor(1f,true);
                            }
                            else
                            {
                                points[i - 1].MakeBlue();
                                points[i - 1].ColorRGB = Design.GetClosestColor(0f,true);
                                points[i].MakeYellow();
                                points[i + 1].MakeRed();
                                points[i + 1].ColorRGB = Design.GetClosestColor(1f,true);
                            }
                            currentReds++;
                        }
                    }
                }
            }
        }

        public Curve(string filen = "", Boolean grayscale = false, Boolean projection = true, int colorID = 0, bool fast = false, bool tad = false)
        {
            fast = false; //remove if we want to pay attention to the fast flag again
            projection = false;
            if (filen == "")
                filen = "chr" + files[currentFile] + "_" + chrtype + ".cpoints";
            go = new GameObject();
            points = ReadInFile(filen);
            Debug.Log("Loaded " + filen);
            Color color = Design.GetClosestColor(0.5f,false);
            Vector3 shift = new Vector3(0, 0, 0);
            if (!grayscale && !tad)
            {
                List<float> colorsIn = new List<float>();
                float maxColor = 0.0f;
                foreach (Point point in points)
                {
                    colorsIn.Add(point.Color);
                    if (point.Color > maxColor)
                        maxColor = point.Color;
                }
                colorSpace = BuildColorMap(colorsIn);
                colorWidth = BuildColorCurve(colorsIn);

                int stepsize = colorSpace.Count / points.Count;
                for (int i = 0; i < points.Count; i++)
                {
                    int idx = (int)Math.Floor((points[i].Color / maxColor) * (colorSpace.Count - 1));
                    points[i].ColorRGB = colorSpace[idx];
                }
            }
            else if (tad)
            {
                for (int i = 1; i < points.Count - 1; i++)
                {
                    points[i].ColorRGB = Design.GetClosestColor(0.5f,false);
                }
                points[0].ColorRGB = Design.GetClosestColor(0f,true) ;
                points[points.Count - 1].ColorRGB = Design.GetClosestColor(1f,true);
            }
            else
            {
                switch (colorID)
                {
                    case 0: color = Design.GetClosestColor(0.5f,false); break;
                    case 1: color = Design.GetClosestColor(0f,true); shift = new Vector3(-1.5f, 0, 0); break;
                    case 2: color = Design.GetClosestColor(1f,true); shift = new Vector3(1.5f, 0, 0); break;
                }
            }
            List<Connector> connectors = new List<Connector>();
            List<LineRenderer> lines = new List<LineRenderer>();

            if (fast)
            {
                LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

                lineRenderer.widthMultiplier = 0.01f / scale;
                lineRenderer.positionCount = points.Count * (int)splineRes;
                var pointarray = new Vector3[points.Count];
                //var matarray = new Material[points.Count];
                for (int i = 0; i < points.Count; i++)
                {
                    pointarray[i] = (points[i].Position / scale) + shift + (projection ? new Vector3(0, 0, 2) : new Vector3(0, 0, 0));
                }
                Vector3[] splinePoints = MakeSplines(pointarray);
                if (tad)
                {
                    float alpha = 1.0f;
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Design.GetClosestColor(0f,true), 0.0f), new GradientColorKey(Design.GetClosestColor(1f,true), 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 1.0f) }
                        );
                    lineRenderer.colorGradient = gradient;
                }
                else if (!grayscale)
                {
                    float alpha = 1.0f;
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Design.GetClosestColor(0f,false), 0.0f), new GradientColorKey(Design.GetClosestColor(1f,true), 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 1.0f) }
                        );
                    lineRenderer.colorGradient = gradient;
                }
                else
                {
                    float alpha = 1.0f;
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(color, 1.0f), new GradientColorKey(color, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 1.0f) }
                        );
                    lineRenderer.colorGradient = gradient;
                }
                lineRenderer.SetPositions(splinePoints);
                lineRenderer.Simplify(0.001f);
                //lineRenderer.materials = matarray;
            }
            else
            {
                cylinders = new List<GameObject>();
                Point[] splinePoints = MakeSplines(points.ToArray());
                if (grayscale)
                {
                    for (int i = 0; i < splinePoints.Length; i++)
                    {
                        splinePoints[i].ColorRGB = color;
                    }
                }
                Point lastPoint = splinePoints[0];
                for (int i = 0; i < splinePoints.Length; i++)
                {
                    connectors.Add(
                        new Connector(lastPoint.Displaced(displacement) / scale,
                        splinePoints[i].Displaced(displacement) / scale)
                        );
                    cylinders.Add(BuildConnector(connectors[connectors.Count - 1]));
                    lastPoint = splinePoints[i];
                }
            }
        }

        ~Curve()
        {
            foreach (GameObject cyl in cylinders)
            {
                UnityEngine.Object.Destroy(cyl);
            }
            cylinders.Clear();
        }

        public float Scale
        {
            get
            {
                return scale;
            }
        }

        public float SphereWidth
        {
            get
            {
                return sphereWidth;
            }
        }

        internal List<Point> Points
        {
            get
            {
                return points;
            }
        }

        internal void DestroyEverything()
        {
            foreach (GameObject cyl in cylinders)
            {
                Destroy(cyl);
            }
            cylinders.Clear();
            Destroy(go.GetComponent<LineRenderer>());
        }

        internal GameObject GenerateLineSegment(SegmentDistanceTrial sdt, bool red)
        {
            GameObject gO = new GameObject();
            LineRenderer renderedLine = gO.AddComponent<LineRenderer>();
            bool insideLine = false;
            bool foundStart = false;
            bool foundEnd = false;
            List<Vector3> pointsInLine = new List<Vector3>();
            string start = null;
            string end = null;
            if (red)
            {
                start = sdt.RedA;
                end = sdt.RedB;
            }
            else
            {
                start = sdt.BlueA;
                end = sdt.BlueB;
            }
            foreach (Point point in points)
            {
                if (!insideLine)
                {
                    if (point.Name == start)
                    {
                        insideLine = true;
                        foundStart = true;
                        pointsInLine.Add((point.Position / scale) + new Vector3(0, 0, 2));
                    }
                }
                else
                {
                    if (point.Name == end)
                    {
                        foundEnd = true;
                        insideLine = false;
                        pointsInLine.Add((point.Position / scale) + new Vector3(0, 0, 2));
                        break;
                    }
                    else
                    {
                        pointsInLine.Add((point.Position / scale) + new Vector3(0, 0, 2));
                    }
                }
            }
            if (!foundStart)
            {
                throw new Exception("Could not find " + start);
            }
            if (!foundEnd)
            {
                throw new Exception("Could not find " + end);
            }
            Vector3[] pointArray = pointsInLine.ToArray();
            renderedLine.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            renderedLine.widthMultiplier = 0.01f / scale;
            renderedLine.positionCount = pointArray.Length;
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Design.GetClosestColor(red ? 0f : 1f,true), 1.0f), new GradientColorKey(Design.GetClosestColor(red ? 0f : 1f,true), 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 1.0f) }
                );
            renderedLine.colorGradient = gradient;
            renderedLine.SetPositions(pointArray);
            return gO;
        }

        internal List<Point> ReadInFile(string filename)
        {
            TextAsset file = Resources.Load(filename + ".cpoints") as TextAsset;
            int counter = 0;
            List<Point> points = new List<Point>();
            foreach (string line in file.text.Split('\n'))
            {
                if (line != "")
                {
                    points.Add(new Point(line));
                    counter++;
                }
            }
            for (int i = 0; i < counter; i++)
            {
                points[i].InitialiseRGBValue(i, counter);
            }
            points.Sort();
            return points;
        }

        private AnimationCurve BuildColorCurve(List<float> colorsIn)
        {
            AnimationCurve curve = new AnimationCurve();
            float min = colorsIn.Min();
            float max = colorsIn.Max();
            float step = (max - min) / 8;
            int[] widths = new int[8];
            float i_last = 0f;
            int idx = 0;
            for (float i = min; i < max; i += step)
            {
                foreach (float c in colorsIn)
                {
                    if (c <= i && c > i_last)
                    {
                        widths[idx]++;
                    }
                }
                curve.AddKey(idx / 8f, (widths[idx] + 1f) / colorsIn.Count);
                i_last = i;
                idx++;
            }

            return curve;
        }

        private List<Color> BuildColorMap(List<float> colorsIn)
        {
            colorsIn.Sort();
            float pointcount = colorsIn.Count;
            int tbucket = 0;
            float last = 0f;
            List<float> buckets = new List<float>();
            for (int i = 0; i < pointcount; i++)
            {
                float value = colorsIn[i];
                buckets.Add(value - last);
                last = value;
            }

            var colorSpace = new List<Color>();
            for (float i = 0; i < (buckets.Max()); i++)
            {
                colorSpace.Add(Design.GetClosestColor(((float)i / buckets.Max()),false));
            }
            int next = 0;
            var outSpace = new List<Color>();
            for (int i = 0; i < buckets.Count; i++)
            {
                for (int k = next; k < next + buckets[i] * 10000; k++)
                {
                    outSpace.Add(colorSpace[i]);
                }
                next += (int)(buckets[i] * 10000);
            }

            return outSpace;
        }

        private GameObject BuildConnector(Connector connector)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.GetComponent<Collider>().enabled = false;
            cylinder.GetComponent<MeshRenderer>().material.color = connector.InterpolatedColor;
            Vector3 pos = Vector3.Lerp(connector.StartPoint, connector.EndPoint, 0.5f);
            //if (fastdraw)
            //  pos = pos + new Vector3 (0, 0, 7);
            cylinder.transform.position = pos;
            cylinder.transform.up = connector.EndPoint - connector.StartPoint;
            cylinder.transform.parent = GameObject.Find("ObjectManager").transform;
            Vector3 offset = connector.EndPoint - connector.StartPoint;
            Vector3 scale = new Vector3(cylinderWidth, offset.magnitude / 2f, cylinderWidth);
            cylinder.transform.localScale = scale;
            return cylinder;
        }

        private int ClampPos(int i, int l)
        {
            if (i < 0)
                return l - 1;
            if (i > l)
                return 1;
            if (i > l - 1)
                return 0;
            return i;
        }

        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            p0 *= 1000;
            p1 *= 1000;
            p2 *= 1000;
            p3 *= 1000;
            //The coefficients of the cubic polynomial (except the 0.5f * which is added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos / 1000;
        }

        //Based on http://www.habrador.com/tutorials/interpolation/1-catmull-rom-splines//
        private Vector3[] MakeSplines(Vector3[] pointarray)
        {
            int loops = (int)splineRes;//Mathf.FloorToInt(1f / res);
            float res = 1f / splineRes;
            int l = pointarray.Length;
            Vector3[] output = new Vector3[l * loops];
            int nextPoint = 0;
            for (int i = 0; i < l; i++)
            {
                Vector3 p0 = pointarray[ClampPos(i - 1, l)];
                Vector3 p1 = pointarray[i];
                Vector3 p2 = pointarray[ClampPos(i + 1, l)];
                Vector3 p3 = pointarray[ClampPos(i + 2, l)];

                for (int j = 0; j < loops; j++)
                {
                    float t = j * res;
                    Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                    output[nextPoint] = newPos;
                    nextPoint++;
                }
            }
            return output;
        }

        private Point[] MakeSplines(Point[] pointArray)
        {
            int loops = (int)splineRes;//Mathf.FloorToInt(1f / res);
            float res = 1f / splineRes;
            int l = pointArray.Length;
            Point[] output = new Point[l * loops];
            int nextPoint = 0;
            for (int i = 0; i < l; i++)
            {
                Vector3 p0 = pointArray[ClampPos(i - 1, l)].Position;
                Vector3 p1 = pointArray[i].Position;
                Vector3 p2 = pointArray[ClampPos(i + 1, l)].Position;
                Vector3 p3 = pointArray[ClampPos(i + 2, l)].Position;

                for (int j = 0; j < loops; j++)
                {
                    float t = j * res;
                    Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                    output[nextPoint] = new Point(
                        newPos.x,
                        newPos.y,
                        newPos.z,
                        pointArray[i].Name,
                        pointArray[i].Color,
                        pointArray[i].ColorRGB
                        );
                    nextPoint++;
                }
            }
            return output;
        }
    }
}