using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetID : MonoBehaviour {
    [HideInInspector] public string fileName;
    [HideInInspector] public Dictionary<string, string> metaData;
    [SerializeField] private Text fileNameText;
    [SerializeField] private Button thisSelectionButton;
    [SerializeField] private GameObject thisDeleteButton;
    [HideInInspector] public string mountPath;

    public void SetText(string filename) {
        fileNameText.text = filename;
    }

    public void SetSelectable(bool selectable) {
        thisDeleteButton.SetActive(!selectable);
        thisSelectionButton.interactable = selectable;
    }
}
