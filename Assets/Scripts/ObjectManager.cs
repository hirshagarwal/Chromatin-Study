using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class ObjectManager : MonoBehaviour {
	public string filename;
	public float cylinderwidth;
	public float spherewidth;
    public TextAsset textfile;
    string[] files = {"10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X" };
    string chrtype = "sen";
    int current_file = 10;
    List<Point> points;
    List<GameObject> spheres = new List<GameObject>();
    List<GameObject> cylinders = new List<GameObject>();
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
		sphere.transform.localScale = new Vector3(spherewidth, spherewidth, spherewidth);
        return sphere;
    }

    GameObject BuildConnector(Connector connector)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.GetComponent<Collider>().enabled = false;
        cylinder.GetComponent<MeshRenderer>().material.color = connector.InterpolatedColor;
        Vector3 pos = Vector3.Lerp(connector.StartPoint, connector.EndPoint, 0.5f);
		pos = pos + new Vector3 (0, 0, 7);
        cylinder.transform.position = pos;
        cylinder.transform.up = connector.EndPoint - connector.StartPoint;
        Vector3 offset = connector.EndPoint - connector.StartPoint;
        Vector3 scale = new Vector3(cylinderwidth, offset.magnitude / 2f, cylinderwidth);
        cylinder.transform.localScale = scale;
        return cylinder;
    }

    // Use this for initialization
    void Start ()
    {
        NextFile();
    }

    private void NextFile()
    {
        foreach (GameObject cyl in cylinders) {
            Destroy(cyl);
        }
        cylinders.Clear();
        foreach (GameObject sph in spheres)
        {
            Destroy(sph);
        }
        spheres.Clear();
        var filen = "chr" + files[current_file] + "_" + chrtype + ".cpoints";
        points = ReadInFile(filen);
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
        List<Connector> connectors = new List<Connector>();
        Debug.Log("Read in file successfully");

        cylinders = new List<GameObject>();
        foreach (Point point in points)
        {
			//spheres.Add(BuildSphere(point.ColorRGB, point.Position + new Vector3(0,0,0)));

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
            if (chrtype == "pro")
            {
                guiText += "\nProliferating";
            }
            else
            {
                guiText += "\nSenescent";
            }
            mousePosition = mousePos;
        }
        else
        {
            guiText = "";
            mousePosition = new Vector2(Screen.width + 10, Screen.height + 10);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            current_file--;
            current_file = ((current_file %= files.Length) < 0) ? current_file + files.Length : current_file;
            NextFile();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            current_file = (current_file + 1) % files.Length;
            NextFile();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (chrtype == "sen")
            {
                chrtype = "pro";
            }
            else
            {
                chrtype = "sen";
            }
            NextFile();
        }
    }
		

    private void OnGUI()
    {
        GUI.Box(new Rect(mousePosition.x+15, Screen.height-mousePosition.y+15, 200, 40), guiText);
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
        int halfcount = (int) Math.Floor(pointcount / 2);
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
}
