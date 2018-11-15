using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    private Button myselfButton;

    private void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("ObjectManager").GetComponent<StudyManager>().ParseSubjectIDAndInitialiseStudy()
      );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Start");
            GameObject.Find("ObjectManager").GetComponent<StudyManager>().ParseSubjectIDAndInitialiseStudy();
        }
    }
}