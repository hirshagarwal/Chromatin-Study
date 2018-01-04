using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudyManager : MonoBehaviour
{
    //public static GameObject answerButton;
    //public static bool answerGiven = false;
    //public static GameObject currentVisualization;
    //private static int MAX_TRIALS = 13;
    //private Color[] colors = { Color.red, Color.yellow };
    //private GameObject currentSelectionMarker;
    //private GameObject cursorPosition;
    //private String dataString;
    //private GameObject hoveredPoint;
    //private Boolean initialized = false;
    //private GameObject MenuDimensions;
    //public static bool participantReadyToAnswer = false;
    //public static bool startNewTrial = false;
    public static GameObject choicePanelClusters;

    public static GameObject choicePanelDistance;
    public static TimeSpan duration;
    public static GameObject feedbackPanel;
    public static string FILE_SEPARATOR = "\\";
    public static Text infoMessage;
    public static GameObject infoPanel;
    public static char LINE_BREAK = '\r';
    public static GameObject panelCanvas;
    public static GameObject viewImageTarget;
    public ObjectManager objectManager;
    public int clickCount;
    public Trial currentTrial;
    public string cursorTracking = "TIME, C_X, C_Y, C_Z";
    public string cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";
    public DateTime dateStart;
    public string mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
    public int numberOfParticipants;
    public bool running = false;
    public bool shiftDown = false;
    public Formats studyFormat;
    public int totalTrials = 208;
    public int trialNumber = 0;
    private List<GameObject> activePoints = new List<GameObject>();
    private Participant currentParticipant;
    private Text feedbackText;
    private int frameCount;
    private GameObject panel;
    private GameObject participantPanel;
    private string results = Design.CSV_HEADER;
    private string saveFileName = "results.csv";
    private List<GameObject> selectedPoints = new List<GameObject>();
    public void FinishTrial()
    {
        feedbackPanel.SetActive(false);
        panelCanvas.SetActive(false);

        trialNumber++;
        if (trialNumber < currentParticipant.TotalTrials)
        {
            LoadTrial(trialNumber);
        }
        else
        {
            panelCanvas.SetActive(true);
            infoPanel.SetActive(true);
            infoMessage.text =
    @"Congratulations!

You have completed this condition. <br><br>
Please, call the instructor.";
        }
    }

    public void RecordResults(object rawAnswer)
    {
        string correct = "null";
        if (currentTrial.Task == Tasks.PointDistance)
        {
            print(">>> DISTANCE ANWSWER: " + rawAnswer);
            if (rawAnswer.Equals("AnswerButton_Red"))
                correct = ((PointDistanceTrial)currentTrial.TrialDetails).Correct(true).ToString();
            if (rawAnswer.Equals("AnswerButton_Blue"))
                correct = ((PointDistanceTrial)currentTrial.TrialDetails).Correct(false).ToString();
        }

        string formatAnswer =
            currentTrial.ToCSV() + ", " +
            studyFormat.ToString() + ", " +
            correct + ", " +
            duration.TotalMilliseconds
        ;

        print(">>>> RESULT LINE= " + formatAnswer);

        WriteToFile(formatAnswer);

        // remove answer screen and load next trial
        choicePanelClusters.SetActive(false);
        choicePanelClusters.SetActive(false);
        infoPanel.SetActive(false);

        feedbackPanel.SetActive(true);
        panelCanvas.SetActive(true);
        if (correct == true.ToString())
            feedbackText.text = "Correct! :)";
        else
            feedbackText.text = "That was wrong :(";
    }

    public void RecordTimeAndCollectUserAnswer()
    {
        feedbackPanel.SetActive(false);
        duration = DateTime.Now - dateStart;
        running = false;

        // show answer screen and wait for user answer.
        // set answer choices
        if (currentTrial.Task == Tasks.PointDistance)
        {
            panelCanvas.SetActive(true);
            choicePanelDistance.SetActive(true);
        }
    }

    public void StartStudy()
    {
        int subjectID = GameObject.Find("Dropdown").GetComponent<Dropdown>().value;
        saveFileName = "results_" + subjectID + "_" + studyFormat.ToString() + ".csv";

        currentParticipant = new Participant(subjectID, studyFormat);

        participantPanel.SetActive(false);
        FinishTrial();
    }

    public void StartTrial()
    {
        panelCanvas.SetActive(false);
        infoPanel.SetActive(false);
        choicePanelDistance.SetActive(false);
        choicePanelClusters.SetActive(false);
        for (int i = 0; i < activePoints.Count; i++)
        {
            Destroy(activePoints[i]);
        }

        selectedPoints.Clear();
        activePoints.Clear();

        // restart this script and load data.
        StartTask();

        GUI.FocusControl(null);

        running = true;

        //start measuring time
        dateStart = DateTime.Now;
    }

    // loads the current trial
    private void LoadTrial(int trial)
    {
        Trial nextTrial = currentParticipant.NextTrial();
        panelCanvas.SetActive(false);
        infoPanel.SetActive(false);
        choicePanelDistance.SetActive(false);
        choicePanelClusters.SetActive(false);
        GUI.FocusControl(null);

        if (trial == 0)
        {//First time introduction
            currentTrial = nextTrial;
            if (currentTrial.Training)
            {
                ShowNewTaskPanel(currentTrial.Task);
            }
            else
            {
                ShowRecordingPanel();
            }
        }

        // Check which conditions changed
        if (currentTrial.Training
            && !nextTrial.Training)
        {
            // stop and show recording-starts screen, which waits for user input
            ShowRecordingPanel();
            currentTrial = nextTrial;
        }
        else
        if (!currentTrial.Training
            && nextTrial.Training)
        {
            // stop and show new condition screen, which waits for user input
            ShowNewTaskPanel(nextTrial.Task);
            currentTrial = nextTrial;
        }
        else
        {
            // start normal trial
            StartTrial();
            currentTrial = nextTrial;
        }
    }

    private void PopulateParticipantSelectionDialog(int participantCount)
    {
        Dropdown dd = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        dd.options.Clear();
        GameObject prefab = GameObject.Find("DropdownLabel");
        for (int i = 0; i < participantCount + 1; i++)
        {
            dd.options.Add(new Dropdown.OptionData() { text = i + "" });
        }
    }

    private void ShowNewTaskPanel(Tasks task)
    {
        panelCanvas.SetActive(true);
        panel.SetActive(true);
        infoPanel.SetActive(true);
        if (task == Tasks.PointDistance)
        {
            infoMessage.text = Design.TASK_DESCRIPTION_DISTANCE;
        }
    }

    private void ShowRecordingPanel()
    {
        panelCanvas.SetActive(true);
        infoPanel.SetActive(true);
        infoMessage.text = Design.TRAINING_END;
    }

    //Step 1
    private void Start()
    {
        if (Application.platform == RuntimePlatform.OSXEditor
        || Application.platform == RuntimePlatform.OSXPlayer)
        {
            FILE_SEPARATOR = "/";
            LINE_BREAK = '\n';
        }
        print("FILE_SEPARATOR = " + FILE_SEPARATOR);

        // SET UP SCENE
        viewImageTarget = GameObject.Find("ViewImageTarget");

        // assign button funcs:
        GameObject.Find("ContinueButton").AddComponent<ContinueButton>();
        GameObject.Find("FeedbackContinueButton").AddComponent<FeedbackButton>();
        GameObject.Find("StartButton").AddComponent<StartButton>();
        GameObject.Find("AnswerButton_Red").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_Blue").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_3").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_4").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_5").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_6").AddComponent<GenericButton>();

        panel = GameObject.Find("Panel");
        panelCanvas = GameObject.Find("PanelCanvas");
        infoPanel = GameObject.Find("InfoPanel");
        participantPanel = GameObject.Find("ParticipantPanel");
        feedbackPanel = GameObject.Find("FeedbackPanel");
        feedbackText = GameObject.Find("FeedbackText").GetComponent<Text>();

        infoMessage = GameObject.Find("InfoMessage").GetComponent<Text>();

        choicePanelDistance = GameObject.Find("ChoicePanelDistance");
        choicePanelDistance.SetActive(false);

        choicePanelClusters = GameObject.Find("ChoicePanelClusters");
        choicePanelClusters.SetActive(false);

        infoPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        PopulateParticipantSelectionDialog(numberOfParticipants);
    }
    // Use this for initialization
    private void StartTask()
    {
        //if (studyFormat == Formats.HoloLens)
        //{
        //    throw new System.Exception("HoloLens handler not implemented!");
        //}
        //else if (studyFormat == Formats.Projection)
        //{
        //    throw new System.Exception("Projection handler not implemented!");
        //}
        //else if (studyFormat == Formats.Heatmap)
        //{
        //    throw new System.Exception("Heatmap handler not implemented!");
        //}

        if (currentTrial.Task == Tasks.PointDistance)
        {
            objectManager.SetupPointDistanceTrial(currentTrial.TrialDetails as PointDistanceTrial, currentTrial.ChromosomeFilename);
        }

        mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
        cursorTracking = "TIME, C_X, C_Y, C_Z";
        cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";

        clickCount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RecordTimeAndCollectUserAnswer();
        }

        // Track interaction
        if (frameCount == Design.TRACKING_FRAME_RATE)
        {
            frameCount = 0;
            Vector3 eulerAngles = GameObject.Find("ViewImageTarget").transform.rotation.eulerAngles;
            Vector3 pos = GameObject.Find("ViewImageTarget").transform.position;
            Vector3 campos = GameObject.Find("HoloLensCamera").transform.position;

            TimeSpan duration = DateTime.Now - dateStart;
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

    private void WriteToFile(string answer)
    {
        results = results + System.Environment.NewLine + answer;
    }
}