﻿using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    private Button myselfButton;

    private void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(
            () => GameObject.Find("ObjectManager").GetComponent<StudyManager>().StartTrial()
      );
    }
}