using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public StudyManager studyManager;

    // STATIC CONFIGS
    // Names of pictures in the tracker data base.
    // Database name: "Atelier".
    private string POINT_TRACKER_NAME = "cursor";

    private string PLANE_TRACKER_NAME = "stones";
    private string T_0_TRACKER_NAME = "T_0";
    private string T_3_TRACKER_NAME = "T_3";
    private string C_RED_TRACKER_NAME = "C_red";
    private string C_BLUE_TRACKER_NAME = "C_blue";
    private string EXTRACT_TRACKER_NAME = "extract";

    private static float DEFAULT_UNSELECTED_TRANSPARENCY = .1f;
    private static Vector4 DEFAULT_SELECTED_COLOR = new Vector4(1f, 1f, 1f, 1f);
    private static float DEFAULT_POINT_SIZE = 0.003f; // 3 mm
    private static float DEFAULT_SCALE = 0.1f;   // 1 cm
    private static float DEFAULT_OPERATION_RANGE = DEFAULT_POINT_SIZE;

    public Material linesGraphMaterial;

    public int[] dim = new int[4] { 0, 1, 2, 3 };

    private GameObject MenuDimensions;
    private GameObject mainViewGO;
    public Material mainViewPointCloudMaterial;
    private GameObject secondViewGO;
    public Material secondaryViewPointCloudMaterial;
    private Shader cubeShader;
    public int clickCount;

    private static GameObject desktopCuttingplane;

    private static GameObject arCuttingplane;
    private GameObject cursorPosition;
    private GameObject line1;
    private GameObject line2;


    private GameObject currentSelectionMarker;

    private String dataString;

    private Tasks currentTask;
    private int currentTrial;

    private List<GameObject> selectedPoints = new List<GameObject>();
    private List<GameObject> activePoints = new List<GameObject>();

    private static int MAX_TRIALS = 13;

    public bool shiftDown = false;

    private Color[] colors = { Color.red, Color.yellow };

    public string mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
    public string cursorTracking = "TIME, C_X, C_Y, C_Z";
    public string cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";

    private GameObject boundingbox;
    private Boolean initialized = false;

    // Use this for initialization
    private void Start()
    {
        if (Design.format == Formats.HoloLens)
        {
            throw new System.Exception("HoloLens handler not implemented!");
        }
        else if (Design.format == Formats.Projection)
        {
            throw new System.Exception("Projection handler not implemented!");
        } else if (Design.format == Formats.Heatmap)
        {
            throw new System.Exception("Heatmap handler not implemented!");
        }

        LoadData();

        mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
        cursorTracking = "TIME, C_X, C_Y, C_Z";
        cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";

        clickCount = 0;
    }

    // restarts this script and askes to load the passed data path.
    public void Restart(Tasks currentTask, int currentTrial)
    {
        this.currentTask = currentTask;
        this.currentTrial = currentTrial;

        for (int i = 0; i < activePoints.Count; i++)
        {
            Destroy(activePoints[i]);
        }

        selectedPoints.Clear();
        activePoints.Clear();

        // destroy current view if existing
        if (mainViewGO != null)
            Destroy(mainViewGO);

        // restart this script and load data.
        Start();
    }

    // loads the data passed in the text string (CSV);
    public void LoadData()
    {
        throw new Exception("Data Loading Not Implemented");
    }

    // Update is called once per frame
    private GameObject hoveredPoint;

    private int frameCount;
    private Vector3 v0 = new Vector3(0f, 0f, 0f);
    private Vector3 v1 = new Vector3(0f, 0f, 0f);
    private Vector3 v2 = new Vector3(0f, 0f, 0f);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            studyManager.RecordTimeAndCollectUserAnswer(null);
        }

        // Track interaction
        if (frameCount == Design.TRACKING_FRAME_RATE)
        {
            frameCount = 0;
            Vector3 eulerAngles = GameObject.Find("ViewImageTarget").transform.rotation.eulerAngles;
            Vector3 pos = GameObject.Find("ViewImageTarget").transform.position;
            Vector3 campos = GameObject.Find("HoloLensCamera").transform.position;

            TimeSpan duration = DateTime.Now - GameObject.Find("StudyObject").GetComponent<TaskManager>().dateStart;
            mainTracking +=
                Design.LINE_BREAK +
                    duration.TotalMilliseconds.ToString()
                    + ", " + pos.x
                    + ", " + pos.y
                    + ", " + pos.z
                    + ", " + eulerAngles.x
                    + ", " + eulerAngles.y
                    + ", " + eulerAngles.z
                    + ", " + campos.x
                    + ", " + campos.y
                    + ", " + campos.z
                    ;

            //if (currentTask == "selection")
            //{
            //    pos = GameObject.Find("CursorImageTarget").transform.position;
            //    cursorTracking +=
            //        Designfile.LINE_BREAK +
            //        duration.TotalMilliseconds.ToString()
            //        + ", " + pos.x
            //        + ", " + pos.y
            //        + ", " + pos.z
            //        ;
            //}

            //if (currentTask == "cuttingplane")
            //{
            //    eulerAngles = GameObject.Find("ARCuttingplane").transform.rotation.eulerAngles;
            //    pos = GameObject.Find("ARCuttingplane").transform.position;

            //    cuttingplaneTracking +=
            //        Designfile.LINE_BREAK +
            //        duration.TotalMilliseconds.ToString()
            //        + ", " + pos.x
            //       + ", " + pos.y
            //       + ", " + pos.z
            //       + ", " + eulerAngles.x
            //       + ", " + eulerAngles.y
            //       + ", " + eulerAngles.z
            //       ;
            //}
        }
        frameCount++;

        if (mainViewPointCloudMaterial == null)
            return;

        mainViewPointCloudMaterial.SetVector("selectionColor", DEFAULT_SELECTED_COLOR);
        mainViewPointCloudMaterial.SetFloat("nonSelectedOpacity", DEFAULT_UNSELECTED_TRANSPARENCY);
        mainViewPointCloudMaterial.SetFloat("operationRange", DEFAULT_OPERATION_RANGE);

        /////////////////////
        /// HOLOLENS CODE ///
        /////////////////////

        int dimensionality = 3;

        if (currentTask == "selection")
        {
            cursorPosition = GameObject.Find("AR_Point");
            v0 = cursorPosition.transform.position;
            Vector3 pos;
            for (int i = 0; i < activePoints.Count; i++)
            {
                if (selectedPoints.Contains(activePoints[i]))
                    continue;

                pos = activePoints[i].transform.position;
                pos = pos - v0;

                if (pos.magnitude < DEFAULT_OPERATION_RANGE * 4)
                {
                    activePoints[i].GetComponent<Renderer>().material.color = Color.yellow;
                    hoveredPoint = activePoints[i];
                    break;
                }
                else
                {
                    activePoints[i].GetComponent<Renderer>().material.color = Color.red;
                    hoveredPoint = null;
                }
            }
        }
        else
        if (currentTask == "cuttingplane")
        {
            activeTrackables = sm.GetActiveTrackableBehaviours();
            
            dimensionality = 2;

            v0 = GameObject.Find("CuttingplaneCorner1").transform.position;
            v1 = GameObject.Find("CuttingplaneCorner2").transform.position;
            v2 = GameObject.Find("CuttingplaneCorner3").transform.position;
            dimensionality = 2;

            // Calculate cuttingplane to determine which red cubes become green.
            Plane cuttingPlane = new Plane(v0, v1, v2);
            float distance;
            for (var i = 0; i < activePoints.Count; i++)
            {
                distance = (float)Math.Abs(cuttingPlane.GetDistanceToPoint(activePoints[i].transform.position));
                if (distance < DEFAULT_POINT_SIZE)
                {
                    activePoints[i].GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    activePoints[i].GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }

        mainViewPointCloudMaterial.SetFloat("dimensionality", dimensionality);
        if (currentTask == "selection")
        {
            mainViewPointCloudMaterial.SetFloat("operationRange", DEFAULT_OPERATION_RANGE * 3);
            mainViewPointCloudMaterial.SetVector("p0Temp", new Vector4(v0.x, v0.y, v0.z, 0f));
        }
        else
        if (currentTask == "cuttingplane")
        {
            mainViewPointCloudMaterial.SetVector("p0Temp", new Vector4(v0.x, v0.y, v0.z, 0f));
            mainViewPointCloudMaterial.SetVector("p1Temp", new Vector4(v1.x, v1.y, v1.z, 0f));
            mainViewPointCloudMaterial.SetVector("p2Temp", new Vector4(v2.x, v2.y, v2.z, 0f));
        }
    }

    // checks if cursor is hovering a point
    public void EvaluatePointSelection()
    {
        if (hoveredPoint != null)
        {
            hoveredPoint.GetComponent<Renderer>().material.color = Color.green;
            if (selectedPoints.Contains(hoveredPoint))
                return;

            selectedPoints.Add(hoveredPoint);

            if (selectedPoints.Count == activePoints.Count)
            {
                studyScript.RecordTimeAndCollectUserAnswer(null);
            }
        }
    }

    public void SetActivePoint(Vector3 pos, Color color)
    {
        GameObject go;
        float size = DEFAULT_POINT_SIZE * 1.1f;
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3((pos.x * DEFAULT_SCALE) - 0.05f, (pos.y * DEFAULT_SCALE), (pos.z * DEFAULT_SCALE) - 0.05f);
        go.transform.localScale = new Vector3(size, size, size);
        go.GetComponent<Renderer>().material.color = color;
        activePoints.Add(go);

        if (Designfile.technique == "holo")
        {
            go.transform.parent = GameObject.Find("ViewImageTarget").transform;
        }
    }

    //////////////////////////
    // VIEW GENERATION CODE //
    //////////////////////////

    private GameObject CreateLabel(string label, GameObject parent, Vector3 position)
    {
        GameObject TextObject = new GameObject(label);
        TextObject.AddComponent<TextMesh>();
        TextMesh tm = TextObject.GetComponent<TextMesh>();
        tm.text = label;
        TextObject.transform.localPosition = position;
        TextObject.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        tm.fontSize = 108;
        tm.color = Color.black;
        TextObject.AddComponent<BoxCollider>();

        TextObject.transform.parent = parent.transform;

        return TextObject;
    }

    /// <summary>
    /// sets the position of the view
    /// </summary>
    /// <param name="view"></param>
    /// <param name="pos"></param>
    private void SetViewPosition(ref GameObject view, Vector3 pos)
    {
        view.transform.position = pos;
    }

    /// <summary>
    /// sets the scale of the view
    /// </summary>
    /// <param name="view"></param>
    /// <param name="scale"></param>
    private void SetViewScale(ref GameObject view, Vector3 scale)
    {
        view.transform.localScale = scale;
    }

    /// <summary>
    /// sets the orientation of the view
    /// </summary>
    /// <param name="view"></param>
    /// <param name="quat"></param>
    private void SetViewRotation(ref GameObject view, Quaternion quat)
    {
        view.transform.rotation = quat;
    }

    /// <summary>
    /// creates a histogram view
    /// </summary>
    /// <param name="dobjs"></param>
    /// <param name="Dimension"></param>
    /// <param name="binSize"></param>
    /// <param name="smooth"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    private GameObject CreateSingle1DView(DataObject dobjs, int Dimension, int binSize, bool smooth, float scale)
    {
        GameObject Snax = new GameObject();
        List<Vector3> l = new List<Vector3>();

        //get the array of dimension
        float[] values = dobjs.GetCol(dobjs.DataArray, Dimension);
        float[] bins = new float[binSize + 1];

        //bin the values
        for (int i = 0; i < values.Length; i++)
        {
            int indexBin = Mathf.RoundToInt(values[i] * binSize);
            bins[indexBin] += 0.05f;
        }

        float minBin = values.Min();
        float maxBin = values.Max();

        //create the data points height (~ histo)
        for (int i = 0; i < bins.Length; i++)
        {
            l.Add(new Vector3(i, bins[i], 0));
        }

        Vector3[] pointCurved;
        if (smooth) pointCurved = Curver.MakeSmoothCurve(l.ToArray(), binSize);
        else pointCurved = l.ToArray();

        LineRenderer lineRenderer = Snax.gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = Color.red;
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.SetWidth(0.025f, 0.025f);
        lineRenderer.SetVertexCount(pointCurved.Length);
        lineRenderer.SetPositions(pointCurved);
        lineRenderer.useWorldSpace = false;

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(pointCurved);
        int[] indices = tr.Triangulate();

        Vector2[] UVs = new Vector2[4];
        UVs[0] = new Vector2(0, 1);
        UVs[1] = new Vector2(1, 1);
        UVs[2] = new Vector2(0, 0);
        UVs[3] = new Vector2(1, 0);

        //Create the indices
        for (int i = 0; i < pointCurved.Length; i++)
        {
        }

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[pointCurved.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(pointCurved[i].x, pointCurved[i].y, 0);
        }

        Snax.transform.localScale *= scale;

        return Snax;
    }

    /// <summary>
    /// Creates a single view with dimensions x, y, z
    /// </summary>
    /// <param name="dobjs"></param>
    /// <param name="DimensionX"></param>
    /// <param name="DimensionY"></param>
    /// <param name="DimensionZ"></param>
    /// <param name="topology"></param>
    /// <param name="LinkIndex"> the linking field to create a graph; pass a negative value to ignore</param>
    /// <returns></returns>
    private GameObject CreateSingle2DView(
        DataObject dobjs,
        int DimensionX,
        int DimensionY,
        int DimensionZ,
        int DimensionSize,
        int LinkIndex,
        MeshTopology topology,
        Material material,
        out View v)
    {
        v = new View(topology);
        string viewName = "";

        if (DimensionX > -1) viewName += dobjs.indexToDimension(DimensionX) + " - ";
        if (DimensionY > -1) viewName += dobjs.indexToDimension(DimensionY) + " - ";
        if (DimensionZ > -1) viewName += dobjs.indexToDimension(DimensionZ) + " - ";
        if (DimensionSize > -1) viewName += dobjs.indexToDimension(DimensionSize);

        GameObject view = new GameObject(viewName);
        view.transform.parent = transform;

        v.initialiseDataView(dobjs.DataPoints, view);
        if (DimensionX >= 0)
        {
            v.setDataDimension(dobjs.getDimension(DimensionX), View.VIEW_DIMENSION.X);
        }

        if (DimensionY >= 0)
        {
            v.setDataDimension(dobjs.getDimension(DimensionY), View.VIEW_DIMENSION.Y);
        }

        if (DimensionZ >= 0)
        {
            v.setDataDimension(dobjs.getDimension(DimensionZ), View.VIEW_DIMENSION.Z);
        }

        if (LinkIndex < 0)
            v.updateView(null);
        else
            v.updateView(dobjs.getDimension(LinkIndex));

        view.AddComponent<MeshFilter>();
        view.AddComponent<MeshRenderer>();

        view.GetComponent<MeshFilter>().mesh = v.MyMesh;
        view.GetComponent<Renderer>().material = material;

        return view;
    }

    private void PrintVector(String name, Vector3 v)
    {
        print(name + "=" + v.x + ", " + v.y + ", " + v.z);
    }
}