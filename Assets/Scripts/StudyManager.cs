﻿using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudyManager : MonoBehaviour
{
    public static GameObject choicePanelClusters;
    public static GameObject choicePanelSegment;
    public static GameObject choicePanelCurve;
    public static GameObject choicePanelDistance;
    public static GameObject choicePanelUnderstanding;
    public static TimeSpan duration;
    public static GameObject feedbackPanel;
    public static string FILE_SEPARATOR = "\\";
    public static Text infoMessage;
    public static GameObject infoPanel;
    public static char LINE_BREAK = '\r';
    public static GameObject panelCanvas;
    public static GameObject viewImageTarget;
    public Trial currentTrial;
    public DateTime dateStart;
    public int numberOfParticipants;
    public IObjectManager objectManager;
    public ObjectManager2D objectManager2D;
    public ObjectManager3D objectManager3D;
    public Formats studyFormat;
    public OrbitScript orbitScript;
    public int totalTrials = 208;
    internal int clickCount;
    internal string cursorTracking = "TIME, C_X, C_Y, C_Z";
    internal string cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";
    internal string mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
    internal bool running = false;
    internal bool shiftDown = false;
    internal int trialNumber = 0;
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

You have completed this condition.
Please, call the instructor.";
            GameObject.Find("ContinueButton").SetActive(false);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileName, true))
            {
                foreach (string line in results.Split(System.Environment.NewLine.ToCharArray()))
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
        }
        trialNumber++;
    }

    public void RecordResults(string rawAnswer)
    {
        string correct = "null";
        if (currentTrial.Task == Tasks.PointDistance)
        {
            print(">>> DISTANCE ANWSWER: " + rawAnswer);
            if (rawAnswer.Contains("AnswerButton_Red"))
                correct = ((PointDistanceTrial)currentTrial.TrialDetails).Correct(true).ToString();
            if (rawAnswer.Contains("AnswerButton_Blue"))
                correct = ((PointDistanceTrial)currentTrial.TrialDetails).Correct(false).ToString();
        }
        else if (currentTrial.Task == Tasks.SegmentDistance)
        {
            print(">>> DISTANCE ANWSWER: " + rawAnswer);
            if (rawAnswer.Contains("AnswerButton_Red"))
                correct = ((SegmentDistanceTrial)currentTrial.TrialDetails).Correct(true).ToString();
            if (rawAnswer.Contains("AnswerButton_Blue"))
                correct = ((SegmentDistanceTrial)currentTrial.TrialDetails).Correct(false).ToString();
        }
        else if (currentTrial.Task == Tasks.CurveComparison)
        {
            print(">>> CURVE ANSWER: " + rawAnswer);
            if (rawAnswer.Contains("AnswerButton_Red"))
                correct = ((CurveComparisonTrial)currentTrial.TrialDetails).Correct(true).ToString();
            if (rawAnswer.Contains("AnswerButton_Blue"))
                correct = ((CurveComparisonTrial)currentTrial.TrialDetails).Correct(false).ToString();
        }
        else if (currentTrial.Task == Tasks.AttributeUnderstanding)
        {
            print(">>> UNDERSTANDING ANSWER: " + rawAnswer);
            if (rawAnswer.Contains("AnswerButton_Red"))
                correct = ((AttributeUnderstandingTrial)currentTrial.TrialDetails).Correct(true).ToString();
            if (rawAnswer.Contains("AnswerButton_Blue"))
                correct = ((AttributeUnderstandingTrial)currentTrial.TrialDetails).Correct(false).ToString();
        }
        else
        {
            throw new Exception("Result test for " + currentTrial.Task.ToString() + " not implemented");
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
        choicePanelDistance.SetActive(false);
        choicePanelSegment.SetActive(false);
        choicePanelCurve.SetActive(false);
        choicePanelUnderstanding.SetActive(false);
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
        else if (currentTrial.Task == Tasks.SegmentDistance)
        {
            panelCanvas.SetActive(true);
            choicePanelSegment.SetActive(true);
        }
        else if (currentTrial.Task == Tasks.CurveComparison)
        {
            panelCanvas.SetActive(true);
            choicePanelCurve.SetActive(true);
        }
        else if (currentTrial.Task == Tasks.AttributeUnderstanding)
        {
            panelCanvas.SetActive(true);
            choicePanelUnderstanding.SetActive(true);
        }
        else
        {
            throw new Exception("Recording for " + currentTrial.Task.ToString() + " not implemented");
        }
    }

    public void ParseSubjectIDAndInitialiseStudy()
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
                return;
            }
            else
            {
                ShowRecordingPanel();
                return;
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
            currentTrial = nextTrial;
            StartTrial();
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
        else if (task == Tasks.SegmentDistance)
        {
            infoMessage.text = Design.TASK_DESCRIPTION_SEGMENT;
        }
        else if (task == Tasks.CurveComparison)
        {
            infoMessage.text = Design.TASK_DESCRIPTION_CURVE;
        }
        else if (task == Tasks.AttributeUnderstanding)
        {
            infoMessage.text = Design.TASK_DESCRIPTION_ATTRIBUTE;
        }
        else
        {
            throw new Exception("Message for " + task + " not implemented");
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
        if (objectManager2D != null)
        {
            objectManager = objectManager2D;
        } else if (objectManager3D != null)
        {
            objectManager = objectManager3D;
        } else
        {
            throw new Exception("Neither object manager has been initialised!");
        }
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
        GameObject.Find("AnswerButton_RedDistance").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_RedDistance").GetComponent<GenericButton>().value = "red";
        GameObject.Find("AnswerButton_BlueDistance").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_BlueDistance").GetComponent<GenericButton>().value = "blue";
        GameObject.Find("AnswerButton_RedSegment").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_RedSegment").GetComponent<GenericButton>().value = "red";
        GameObject.Find("AnswerButton_BlueSegment").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_BlueSegment").GetComponent<GenericButton>().value = "blue";
        GameObject.Find("AnswerButton_RedCurve").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_RedCurve").GetComponent<GenericButton>().value = "red";
        GameObject.Find("AnswerButton_BlueCurve").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_BlueCurve").GetComponent<GenericButton>().value = "blue";
        GameObject.Find("AnswerButton_RedUnderstanding").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_RedUnderstanding").GetComponent<GenericButton>().value = "red";
        GameObject.Find("AnswerButton_BlueUnderstanding").AddComponent<GenericButton>();
        GameObject.Find("AnswerButton_BlueUnderstanding").GetComponent<GenericButton>().value = "blue";
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

        choicePanelSegment = GameObject.Find("ChoicePanelSegment");
        choicePanelSegment.SetActive(false);

        choicePanelCurve = GameObject.Find("ChoicePanelCurve");
        choicePanelCurve.SetActive(false);

        choicePanelUnderstanding = GameObject.Find("ChoicePanelUnderstanding");
        choicePanelUnderstanding.SetActive(false);

        infoPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        PopulateParticipantSelectionDialog(numberOfParticipants);
    }

    // Use this for initialization
    private void StartTask()
    {
        if (currentTrial.Task == Tasks.PointDistance)
        {
            objectManager.SetupPointDistanceTrial(currentTrial.TrialDetails as PointDistanceTrial, ((PointDistanceTrial)currentTrial.TrialDetails).Chromosome.ToString());
        }
        else if (currentTrial.Task == Tasks.SegmentDistance)
        {
            objectManager.SetupSegmentDistanceTrial(currentTrial.TrialDetails as SegmentDistanceTrial, ((SegmentDistanceTrial)currentTrial.TrialDetails).Chromosome.ToString());
        }
        else if (currentTrial.Task == Tasks.CurveComparison)
        {
            objectManager.SetupCurveComparisonTrial(currentTrial.TrialDetails as CurveComparisonTrial);
        }
        else if (currentTrial.Task == Tasks.AttributeUnderstanding)
        {
            objectManager.SetupAttributeUnderstandingTrial(currentTrial.TrialDetails as AttributeUnderstandingTrial);
        }
        else
        {
            throw new Exception("Configuration for " + currentTrial.Task.ToString() + " not implemented");
        }
        if (Formats.Projection == studyFormat)
            orbitScript.menu = false;
        mainTracking = "TIME, VIS_X, VIS_Y, VIS_Z,  VIS_A, VIS_B, VIS_C,  CAM_A, CAM_B, CAM_C";
        cursorTracking = "TIME, C_X, C_Y, C_Z";
        cuttingplaneTracking = "TIME, P_X, P_Y, P_, P_A, P_B, P _C";

        clickCount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Formats.Projection == studyFormat)
                orbitScript.menu = true;
            RecordTimeAndCollectUserAnswer();
        }
    }

    private void WriteToFile(string answer)
    {
        results = results + System.Environment.NewLine + answer;
    }
}