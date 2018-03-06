using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager3D : MonoBehaviour, IObjectManager
{
    private Formats studyFormat;
    private Material baseMaterial;
    private TextAsset textFile;
    private Curve mainCurve;
    private Curve redCurve;
    private Curve blueCurve;
    private GameObject blueObject;
    private GameObject redObject;
    private bool showUnderstanding = false;
    private string understandingString = "";
    private string guiText = "";
    private string[] files = { "1", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "X" };
    private string chrtype = "sen";
    private int current_file = 7;
    private Vector2 mousePosition = new Vector2(0, 0);
    private List<GameObject> spheres = new List<GameObject>();

    public Formats StudyFormat
    {
        get
        {
            return studyFormat;
        }
    }

    public Material BaseMaterial
    {
        get
        {
            return baseMaterial;
        }
    }

    public TextAsset TextFile
    {
        get
        {
            return textFile;
        }
    }

    public Curve MainCurve
    {
        get
        {
            return mainCurve;
        }
    }

    public Curve RedCurve
    {
        get
        {
            return redCurve;
        }
    }

    public Curve BlueCurve
    {
        get
        {
            return blueCurve;
        }
    }

    public GameObject BlueObject
    {
        get
        {
            return blueObject;
        }
    }

    public GameObject RedObject
    {
        get
        {
            return redObject;
        }
    }

    public bool ShowUnderstanding
    {
        get
        {
            return showUnderstanding;
        }
    }

    public string UnderstandingString
    {
        get
        {
            return understandingString;
        }
    }

    public void SetupCurveComparisonTrial(CurveComparisonTrial curveComparisonTrial)
    {
        bool fast = (studyFormat == Formats.HoloLens);
        mainCurve = new Curve(curveComparisonTrial.ReferenceChromosome, true, true, 0, fast);
        redCurve = new Curve(curveComparisonTrial.RedChromosome, true, true, 1, fast);
        blueCurve = new Curve(curveComparisonTrial.BlueChromosome, true, true, 2, fast);
    }

    internal GameObject BuildSphere(Color color, Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<MeshRenderer>().material.color = color;
        sphere.transform.position = (position / mainCurve.Scale) + new Vector3(0, 0, 2);
        sphere.transform.localScale = new Vector3(mainCurve.SphereWidth, mainCurve.SphereWidth, mainCurve.SphereWidth);
        return sphere;
    }

    private GameObject BuildSphere()
    {
        return BuildSphere(Color.blue, new Vector3(0, 0, 0));
    }

    private GameObject BuildSphere(Vector3 position)
    {
        return BuildSphere(Color.blue, position);
    }

    public void SetupPointDistanceTrial(PointDistanceTrial pdt, string chrfn)
    {
        bool isFast = (studyFormat == Formats.HoloLens);

        mainCurve = new Curve(chrfn, true, fast:isFast);
        foreach (Point point in mainCurve.Points)
        {
            if (point.Name == pdt.BlueA || point.Name == pdt.BlueB)
            {
                spheres.Add(BuildSphere(Color.blue, point.Position));
            }
            else if (point.Name == pdt.RedA || point.Name == pdt.RedB)
            {
                spheres.Add(BuildSphere(Color.red, point.Position));
            }
        }
    }

    public void SetupAttributeUnderstandingTrial(AttributeUnderstandingTrial adt)
    {
        bool isFast = (studyFormat == Formats.HoloLens);

        mainCurve = new Curve(adt.Chromosome, false, true, fast:isFast);
        understandingString = adt.Question;
        showUnderstanding = true;
    }

    public void SetupSegmentDistanceTrial(SegmentDistanceTrial sdt, string chrfn)
    {
        bool isFast = (studyFormat == Formats.HoloLens);

        mainCurve = new Curve(chrfn, true, true, fast:isFast);
        redObject = mainCurve.GenerateLineSegment(sdt, true);
        blueObject = mainCurve.GenerateLineSegment(sdt, false);
    }

    public void LoadNextFile(string filename = "", bool isFast = false)
    {
        if (isFast == true)
        {
            studyFormat = Formats.HoloLens;
        }
        if ("" == filename)
            filename = "chr" + files[current_file] + "_" + chrtype;
        Destroy(mainCurve);
        mainCurve = new Curve(filename, fast: studyFormat == Formats.HoloLens);
    }

    private LineRenderer BuildLR(Connector connector)
    {
        LineRenderer lr = new LineRenderer();
        lr.material.color = connector.InterpolatedColor;
        lr.positionCount = 2;
        lr.widthMultiplier = 0.2f;
        lr.SetPosition(0, connector.StartPoint);
        lr.SetPosition(1, connector.EndPoint);
        return lr;
    }

    private void OnGUI()
    {
        if (showUnderstanding)
            GUI.Box(new Rect(0, 0, 400, 40), understandingString);
        GUI.Box(new Rect(mousePosition.x + 15, Screen.height - mousePosition.y + 15, 200, 40), guiText);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(gameObject.GetComponent<LineRenderer>());
            Destroy(redObject);
            Destroy(blueObject);
            mainCurve.DestroyEverything();
            try
            {
                redCurve.DestroyEverything();
                blueCurve.DestroyEverything();
            }
            catch
            {
            }
            foreach (var sph in spheres)
            {
                Destroy(sph);
            }
            showUnderstanding = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            current_file--;
            current_file = ((current_file %= files.Length) < 0) ? current_file + files.Length : current_file;
            Destroy(gameObject.GetComponent<LineRenderer>());
            Destroy(redObject);
            Destroy(blueObject);
            mainCurve.DestroyEverything();
            try
            {
                redCurve.DestroyEverything();
                blueCurve.DestroyEverything();
            }
            catch
            {
            }
            foreach (var sph in spheres)
            {
                Destroy(sph);
            }
            LoadNextFile();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            current_file = (current_file + 1) % files.Length;
            Destroy(gameObject.GetComponent<LineRenderer>());
            Destroy(redObject);
            Destroy(blueObject);
            mainCurve.DestroyEverything();
            try
            {
                redCurve.DestroyEverything();
                blueCurve.DestroyEverything();
            }
            catch
            {
            }
            foreach (var sph in spheres)
            {
                Destroy(sph);
            }
            LoadNextFile();
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
            Destroy(gameObject.GetComponent<LineRenderer>());
            Destroy(redObject);
            Destroy(blueObject);
            mainCurve.DestroyEverything();
            try
            {
                redCurve.DestroyEverything();
                blueCurve.DestroyEverything();
            }
            catch
            {
            }
            foreach (var sph in spheres)
            {
                Destroy(sph);
            }
            LoadNextFile();
        }
    }
}