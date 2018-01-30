using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Curve : MonoBehaviour
    {
        public GameObject go;
        public static int currentFile = 13;
        private const bool fastDraw = true;
        private Material baseMaterial;
        private string chrtype = "sen";
        private List<GameObject> cylinders = new List<GameObject>();
        private float cylinderWidth;
        private string fileName;
        private string[] files = { "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X" };
        private List<Point> points;
        private int scale = 3;
        private float sphereWidth = 0.01f;

        public Curve(string filen = "", Boolean grayscale = false, Boolean projection = true, int colorID = 0)
        {
            if (filen == "")
                filen = "chr" + files[currentFile] + "_" + chrtype + ".cpoints";
            go = new GameObject();
            points = ReadInFile(filen);
            Color color = Color.black;
            Vector3 shift = new Vector3(0, 0, 0);
            if (!grayscale)
            {
                List<float> colorsIn = new List<float>();
                float maxColor = 0.0f;
                foreach (Point point in points)
                {
                    colorsIn.Add(point.Color);
                    if (point.Color > maxColor)
                        maxColor = point.Color;
                }
                List<Color> colorMap = BuildColorMap(colorsIn);
                int stepsize = colorMap.Count / points.Count;
                for (int i = 0; i < points.Count; i++)
                {
                    int idx = (int)Math.Floor((points[i].Color / maxColor) * (colorMap.Count - 1));
                    points[i].ColorRGB = colorMap[idx];
                }
            }
            else
            {
                switch (colorID)
                {
                    case 0: color = Color.white; break;
                    case 1: color = Color.red; shift = new Vector3(-1.5f, 0, 0); break;
                    case 2: color = Color.blue; shift = new Vector3(1.5f, 0, 0); break;
                }
            }
            //List<Connector> connectors = new List<Connector>();
            //List<LineRenderer> lines = new List<LineRenderer>();
            Debug.Log("Read in file successfully");

            //cylinders = new List<GameObject>();
            //foreach (Point point in points)
            //{
            //    int closest_value = Int32.MaxValue;
            //    Point closest_point = point;
            //    foreach (Point neighbouring_point in points)
            //    {
            //        if (point != neighbouring_point)
            //        {
            //            int v = neighbouring_point.Start - point.End;
            //            if (v > 0 && v < closest_value)
            //            {
            //                closest_value = v;
            //                closest_point = neighbouring_point;
            //            }
            //        }
            //    }
            //    connectors.Add(new Connector(point, closest_point));
            //    if (!fastDraw)
            //        cylinders.Add(BuildConnector(connectors[connectors.Count - 1]));
            //}
            if (fastDraw)
            {
                LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

                lineRenderer.widthMultiplier = 0.01f / scale;
                lineRenderer.positionCount = points.Count;
                var pointarray = new Vector3[points.Count];
                var matarray = new Material[points.Count];
                for (int i = 0; i < points.Count; i++)
                {
                    pointarray[i] = (points[i].Position / scale) + shift + (projection ? new Vector3(0, 0, 2) : new Vector3(0, 0, 0));
                }
                if (!grayscale)
                {
                    float alpha = 1.0f;
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
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
                lineRenderer.SetPositions(pointarray);

                //lineRenderer.materials = matarray;
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

        internal List<Point> Points
        {
            get
            {
                return points;
            }
        }

        public int Scale
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
                new GradientColorKey[] { new GradientColorKey(red ? Color.red : Color.blue, 1.0f), new GradientColorKey(red ? Color.red : Color.blue, 1.0f) },
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

        private List<Color> BuildColorMap(List<float> colorsIn)
        {
            colorsIn.Sort();
            float pointcount = colorsIn.Count;
            int tbucket = 0;
            float first = 0f;
            List<float> buckets = new List<float>();
            for (int i = 0; i < pointcount; i++)
            {
                float value = colorsIn[i];
                if (tbucket == 0)
                {
                    first = value;
                }
                if (tbucket == 1)
                {
                    buckets.Add(value - first);
                    tbucket = 0;
                }
                else
                {
                    tbucket++;
                }
            }

            var colorSpace = new List<Color>();
            int halfcount = (int)Math.Floor(pointcount / 2);
            for (float i = 0; i < (halfcount); i++)
            {
                colorSpace.Add(new Color(i / (pointcount / 2f), 1f, 1f - (i / (pointcount / 2f))));
            }
            int next = 0;
            var outSpace = new List<Color>();
            for (int i = 0; i < halfcount; i++)
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
            Vector3 offset = connector.EndPoint - connector.StartPoint;
            Vector3 scale = new Vector3(cylinderWidth, offset.magnitude / 2f, cylinderWidth);
            cylinder.transform.localScale = scale;
            return cylinder;
        }

        
    }
}