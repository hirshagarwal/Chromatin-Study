using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class ObjectManager : MonoBehaviour {
    public TextAsset textfile;
    List<Point> points;
    List<GameObject> spheres = new List<GameObject>();
    string guiText = "";
    Vector2 mousePosition = new Vector2(0, 0);
    List<Point> ReadInFile(string filename)
    {
        //System.IO.StreamReader file = new System.IO.StreamReader(filename);
        TextAsset file = Resources.Load(filename) as TextAsset;
        int counter = 0;
       // string line;
        List<Point> points = new List<Point>();
        //while((line = file.ReadLine()) != null)
        foreach(string line in file.text.Split('\n'))
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
        return points;
    }

    GameObject BuildSphere()
    {
        return BuildSphere(Color.blue, new Vector3(0, 0, 0));
    }

    GameObject BuildSphere(Vector3 position)
    {
        return BuildSphere(Color.blue, position);
    }

    GameObject BuildSphere(Color color, Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<MeshRenderer>().material.color = color;
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        return sphere;
    }

    GameObject BuildConnector(Connector connector)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.GetComponent<Collider>().enabled = false;
        cylinder.GetComponent<MeshRenderer>().material.color = connector.InterpolatedColor;
        Vector3 pos = Vector3.Lerp(connector.StartPoint, connector.EndPoint, 0.5f);
        cylinder.transform.position = pos;
        cylinder.transform.up = connector.EndPoint - connector.StartPoint;
        Vector3 offset = connector.EndPoint - connector.StartPoint;
        Vector3 scale = new Vector3(0.05f, offset.magnitude / 2f, 0.05f);
        cylinder.transform.localScale = scale;
        return cylinder;
    }

    // Use this for initialization
    void Start () {
        points = ReadInFile("chr16_sen");
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
            points[i].ColorRGB = colorMap[(int)(points[i].Color/maxColor)*colorMap.Count];
        }
        List<Connector> connectors = new List<Connector>();
        Debug.Log("Read in file successfully");
        
        List<GameObject> cylinders = new List<GameObject>();
        foreach (Point point in points)
        {
            spheres.Add(BuildSphere(point.ColorRGB, point.Position));
            
            int closest_value = Int32.MaxValue;
            Point closest_point = point;
            foreach (Point neighbouring_point in points)
            {
                if (point != neighbouring_point)
                {
                    int v = neighbouring_point.Start - point.End;
                    if (v > 0 && v < closest_value)
                    {
                        closest_value = v;
                        closest_point = neighbouring_point;
                    }
                }
            }
            connectors.Add(new Connector(point, closest_point));
            cylinders.Add(BuildConnector(connectors[connectors.Count - 1]));
        }
        Debug.Log(string.Format("Built {0} spheres", spheres.Count));
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButton(1))
        {
            Camera c = Camera.main;
            Vector3 mousePos = new Vector3();
            mousePos = Input.mousePosition;
            float closest_dist = Mathf.Infinity;
            int closest_point = 0;
            for (int i = 0; i < points.Count; i++)
            {
                float d = Vector3.Distance(mousePos, c.WorldToScreenPoint(spheres[i].transform.position));
                if (d <= closest_dist)
                {
                    closest_dist = d;
                    closest_point = i;
                }
            }
            //spheres[closest_point].GetComponent<MeshRenderer>().material.color = Color.black;
            guiText = points[closest_point].Name;
            mousePosition = mousePos;
        }
        else
        {
            guiText = "";
            mousePosition = new Vector2(Screen.width + 10, Screen.height + 10);
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(mousePosition.x+15, Screen.height-mousePosition.y+15, 200, 22), guiText);
    }

    List<Color> BuildColorMap(List<float> colorsIn)
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
        for (float i = 0; i < (pointcount / 2); i++)
        {
            colorSpace.Add(new Color(i / (pointcount / 2f), 1f, 1f - (i / (pointcount / 2f))));
        }
        int next = 0;
        var outSpace = new List<Color>();
        for (int i = 0; i < (pointcount/2); i++)
        {
            for (int k = next; k < next + buckets[i] * 10000; k++)
            {
                outSpace.Add(colorSpace[i]);
            }
            next += (int)(buckets[i] * 10000);
        }
        return outSpace;
    }
}
