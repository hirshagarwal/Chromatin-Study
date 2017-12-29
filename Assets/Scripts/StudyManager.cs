using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudyManager : MonoBehaviour
{
    public TaskManager taskManager;
    private List<string> trials = new List<string>();
    private String designtext = Design.text;
    public static string answer;
    public int participantID = 0;
    private string training = "R";
    public Formats technique = Formats.HoloLens;
    private int currentTrial = -1;
    public Tasks currentTask;
    public int currentDataSet;
    public int totalTrials = 208;
    private string saveFileName = "results.csv";
    public static bool participantReadyToAnswer = false;
    public static bool startNewTrial = false;
    public static bool answerGiven = false;

    // UI panel element showing answer text and answer choices
    public static GameObject panelCanvas;
    // subpanel of panel contining only the answers
    public static GameObject choicePanelDistance;
    public static GameObject choicePanelClusters;
    public static GameObject feedbackPanel;
    // subpanel of panel contining only the answers
    public static GameObject answerButton;
    public static GameObject infoPanel;
    public static Text infoMessage;
    public static GameObject viewImageTarget;
    public static GameObject currentVisualization;
    public static TimeSpan duration;
    public bool running = false;
    public string FILE_SEPARATOR = "\\";
    public char LINE_BREAK = '\r';
    public DateTime dateStart;
    // Use this for initialization

    // READ ME: USAGE
    // ATTACH THIS SCRIPT TO AN OBJECT IN THE SCENE
    // PROVIDE THE CURRENT PARTICIPANT ID : int Participant;
    // PROVIDE THE TOTAL NUMBER OF TRIALS THAT THE PARTICIPANTS WILL PERFORM:  int totalTrials;

    private GameObject panel;
    private GameObject participantPanel;
    private Text feedbackText;

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

        // load design file
        designtext = Design.text;
        // populate input dropdown

        String[] csv = designtext.Split(LINE_BREAK);

        int participantCount = int.Parse(csv[csv.Length - 1].Split(',')[0]);
        Dropdown dd = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        dd.options.Clear();
        GameObject prefab = GameObject.Find("DropdownLabel");
        for (int i = 0; i < participantCount + 1; i++)
        {
            dd.options.Add(new Dropdown.OptionData() { text = i + "" });
        }
    }

    //loads the "participant" ID conditions from the condition file
    // and stores them in theTrials list
    private void LoadParticipantBlocks(string text, int participant)
    {
        string[] blocks = text.Split(LINE_BREAK);
        string[] line;
        int participantCount = int.Parse(blocks[blocks.Length - 1].Split(',')[0]);

        for (int i = 1; i < blocks.Length; i++)
        {
            line = blocks[i].Split(',');
            if (line.Length == 0)
                continue;
            if (int.Parse(line[0]) == participant
                && line[2] == Design.format.ToString()
            )
                trials.Add(blocks[i]);
        }
    }

    // loads the current trial
    private void LoadTrial(int trial)
    {
        String[] trialConditions = trials[trial].Split(',');

        // here do data parsing on the trial condition
        // and set the data to your visualistion class

        // e.g.
        currentTask = trialConditions[3];
        int trialNumber = int.Parse(trialConditions[4]);
        string newTraining = trialConditions[5];
        currentDataSet = int.Parse(trialConditions[6]);

        panelCanvas.SetActive(false);
        infoPanel.SetActive(false);
        choicePanelDistance.SetActive(false);
        choicePanelClusters.SetActive(false);
        GUI.FocusControl(null);

        // Check which conditions changed
        if (training == "T"
            && newTraining == "R")
        {
            // stop and show recording-starts screen, which waits for user input
            ShowRecordingPanel();

            training = newTraining;
        }
        else
        if (training == "R"
            && newTraining == "T")
        {
            // stop and show new condition screen, which waits for user input
            ShowNewTaskPanel(currentTask);
            training = newTraining;
        }
        else
        {
            // start normal trial
            StartTrial();
        }
    }

    public void StartTrial()
    {
        panelCanvas.SetActive(false);
        infoPanel.SetActive(false);
        choicePanelDistance.SetActive(false);
        choicePanelClusters.SetActive(false);

        // 1) load data
        taskManager.Restart(currentTask, currentTrial);

        GUI.FocusControl(null);

        running = true;

        //start measuring time
        dateStart = DateTime.Now;
    }

    public void RecordTimeAndCollectUserAnswer(object parameters = null)
    {
        feedbackPanel.SetActive(false);
        // record and stop time
        duration = DateTime.Now - dateStart;

        // disable running
        running = false;

        // show answer screen and wait for user answer.
        // set answer choices
        if (currentTask == Tasks.PointDistance)
        {
            panelCanvas.SetActive(true);
            choicePanelDistance.SetActive(true);
        }
        //else
        //if (currentTask == "cluster")
        //{
        //    panelCanvas.SetActive(true);
        //    choicePanelClusters.SetActive(true);
        //}
        //else
        //if (currentTask == "cuttingplane")
        //{
        //    RecordResults(parameters); // distance is float number
        //}
        //else
        //if (currentTask == "selection")
        //{
        //    RecordResults(parameters); // error is number of wrong clicks
        //}
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

    private void ShowFinishedPanel()
    {
        panelCanvas.SetActive(true);
        infoPanel.SetActive(true);
        infoMessage.text =
@"Congratulations!

You have completed this condition. <br><br>
Please, call the instructor.";
    }

    private void ShowRecordingPanel()
    {
        panelCanvas.SetActive(true);
        infoPanel.SetActive(true);
        infoMessage.text = Design.TRAINING_END;
    }

    public void RecordResults(object rawAnswer)
    {
        // Records data and starts next trial.

        //print("RAW ANSWER = " + rawAnswer);

        // set user answer
        String answer = "null";
        float error = -1;
        if (currentTask == Tasks.PointDistance)
        {
            print(">>> DISTANCE ANWSWER: " + rawAnswer);
            if (rawAnswer.Equals("AnswerButton_Red"))
                answer = "1";
            if (rawAnswer.Equals("AnswerButton_Blue"))
                answer = "2";

            if (Distance.answers[currentDataSet] == answer)
                error = 0; // no error
            else
                error = 1; // error
        }
        

        string formatAnswer =
            participantID + ", " +
            currentTrial + ", " +
            Design.format + ", " +
            currentTask + ", " +
            training + "," +
            currentDataSet + ", " +
            answer + ", " +
            error + ", " +
            duration.TotalMilliseconds
        ;

        print(">>>> RESULT LINE= " + formatAnswer);

        //this writes to the answer file
        WriteToFile(formatAnswer);

        // remove answer screen and load next trial
        choicePanelClusters.SetActive(false);
        choicePanelClusters.SetActive(false);
        infoPanel.SetActive(false);

        //        LoadNextTrial();

        feedbackPanel.SetActive(true);
        panelCanvas.SetActive(true);
        //if (currentTask == "selection" || currentTask == "cuttingplane")
        //{
        //    feedbackText.text = "Good! Click 'Continue' for next trial.";
        //}
        //else
        {
            if (error == 0)
            {
                feedbackText.text = "Correct! :)";
            }
            else
                feedbackText.text = "That was wrong :(";
        }
    }

    //  I/O file answers
    private string results = Design.CSV_HEADER;

    private void WriteToFile(string answer)
    {
        results = results + System.Environment.NewLine + answer;
    }

    public void LoadNextTrial()
    {
        feedbackPanel.SetActive(false);
        panelCanvas.SetActive(false);

        currentTrial++;
        if (currentTrial < trials.Count)
        {
            LoadTrial(currentTrial);
        }
        else
        {
            ShowFinishedPanel();
        }
    }

    public void StartStudy()
    {
        participantID = GameObject.Find("Dropdown").GetComponent<Dropdown>().value;
        saveFileName = "results_" + participantID + "_" + Design.format.ToString() + ".csv";

        LoadParticipantBlocks(designtext, participantID);

        participantPanel.SetActive(false);
        LoadNextTrial();
    }
}