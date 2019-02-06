using UnityEngine;
using UnityEngine.UI;

public class FeedbackButton : MonoBehaviour
{
    private Button myselfButton;

    private void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("ObjectManager").GetComponent<StudyManager>().FinishTrial()
      );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Continue");
            GameObject.Find("ObjectManager").GetComponent<StudyManager>().FinishTrial();
        }
    }
}