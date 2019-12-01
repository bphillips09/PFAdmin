using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleID : MonoBehaviour {
    [HideInInspector] public string Id;
    [HideInInspector] public string Name;
    [HideInInspector] public string SecretKey;
    [HideInInspector] public string GameManagerUrl;
    [SerializeField] private Text titleText;

    public void SetText(string titleName) {
        titleText.text = titleName;
    }
}
