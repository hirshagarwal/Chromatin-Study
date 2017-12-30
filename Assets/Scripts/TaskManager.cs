using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public int clickCount;
    public string cursorTracking = "TIME, C_X, C_Y, C_Z";
    public string cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";
    public string mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
    public bool shiftDown = false;
    public StudyManager studyManager;

    private static int MAX_TRIALS = 13;
    private List<GameObject> activePoints = new List<GameObject>();
    private Color[] colors = { Color.red, Color.yellow };
    private GameObject currentSelectionMarker;
    private Tasks currentTask;
    private int currentTrial;
    private GameObject cursorPosition;
    private String dataString;
    private int frameCount;
    private GameObject hoveredPoint;
    private Boolean initialized = false;

    private GameObject MenuDimensions;
    private List<GameObject> selectedPoints = new List<GameObject>();

    // loads the data passed in the text string (CSV);
    public void LoadData()
    {
        throw new Exception("Data Loading Not Implemented");
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

        // restart this script and load data.
        Start();
    }

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
        }
        else if (Design.format == Formats.Heatmap)
        {
            throw new System.Exception("Heatmap handler not implemented!");
        }

        LoadData();

        mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
        cursorTracking = "TIME, C_X, C_Y, C_Z";
        cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";

        clickCount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            studyManager.RecordTimeAndCollectUserAnswer();
        }

        // Track interaction
        if (frameCount == Design.TRACKING_FRAME_RATE)
        {
            frameCount = 0;
            Vector3 eulerAngles = GameObject.Find("ViewImageTarget").transform.rotation.eulerAngles;
            Vector3 pos = GameObject.Find("ViewImageTarget").transform.position;
            Vector3 campos = GameObject.Find("HoloLensCamera").transform.position;

            TimeSpan duration = DateTime.Now - studyManager.dateStart;
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
        }
        frameCount++;
    }
}