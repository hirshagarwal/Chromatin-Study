using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    private Button myselfButton;
    public string value;

    private void Start()
    {
        Debug.Log("Created Generic Button");
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("ObjectManager").GetComponent<StudyManager>().RecordResults(gameObject.ToString())
      );
    }

    private void Update()
    {
        Debug.Log("Listening");
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Continue");
            GameObject.Find("ObjectManager").GetComponent<StudyManager>().RecordResults("AnswerButton_Red");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Continue");
            GameObject.Find("ObjectManager").GetComponent<StudyManager>().RecordResults("AnswerButton_Blue");
        }
    }
}