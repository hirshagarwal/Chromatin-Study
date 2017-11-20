using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class ObjectManager : MonoBehaviour {

    List<Point> ReadInFile(string filename)
    {
        System.IO.StreamReader file = new System.IO.StreamReader(filename);
        int counter = 0;
        string line;
        List<Point> points = new List<Point>();
        while((line = file.ReadLine()) != null)
        {
            points.Add(new Point(line));
            counter++;
        }
        for (int i = 0; i < counter; i++)
        {
            points[i].InitialiseRGB(i, counter);
        }
        return points;
    }

    GameObject BuildGameObject()
    {
        return BuildGameObject(Color.blue, new Vector3(0, 0, 0));
    }

    GameObject BuildGameObject(Vector3 position)
    {
        return BuildGameObject(Color.blue, position);
    }

    GameObject BuildGameObject(Color color, Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<MeshRenderer>().material.color = color;
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return sphere;
    }

    // Use this for initialization
    void Start () {
        List<Point> points = ReadInFile(@"C:\Users\Alex\Documents\visualisation\Assets\chr16_sen.cpoints");
        Debug.Log("Read in file successfully");
        List<GameObject> spheres = new List<GameObject>();
        foreach (Point point in points)
        {
            spheres.Add(BuildGameObject(point.ColorRGB, point.Position()));
        }
        Debug.Log(string.Format("Built {0} spheres", spheres.Count));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
