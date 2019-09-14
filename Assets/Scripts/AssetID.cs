using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetID : MonoBehaviour {
    [HideInInspector] public string fileName;
    [HideInInspector] public Dictionary<string, string> metaData;
    [SerializeField] private Text fileNameText;

    public void SetText(string filename) {
        fileNameText.text = filename;
    }
}
