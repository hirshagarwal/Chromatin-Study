using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    private Button myselfButton;

    private void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("StudyObject").GetComponent<StudyManager>().RecordResults(gameObject.name)
      );
    }
}