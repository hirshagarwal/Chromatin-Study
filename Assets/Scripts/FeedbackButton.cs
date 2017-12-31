using UnityEngine;
using UnityEngine.UI;

public class FeedbackButton : MonoBehaviour
{
    private Button myselfButton;

    private void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("StudyObject").GetComponent<StudyManager>().FinishTrial()
      );
    }
}