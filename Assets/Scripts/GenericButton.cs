using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    private Button myselfButton;
    public string value;

    private void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("ObjectManager").GetComponent<StudyManager>().RecordResults(gameObject.ToString())
      );
    }
}