using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudioID : MonoBehaviour {
    [HideInInspector] public string Id;
    [HideInInspector] public string Name;
    [HideInInspector] public PFTitle[] Titles;
    [SerializeField] private Text studioText;

    public void SetText(string studioName) {
        studioText.text = studioName;
    }
}
