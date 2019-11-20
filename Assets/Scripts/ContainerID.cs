using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerID : MonoBehaviour {
    public string containerName;
    public Text text;
    public Dropdown tagsList;

    public void EnumerateTags(List<string> tagResults) {
        if (tagsList != null) {
            tagsList.options.Clear();
            foreach (var tag in tagResults) {
                tagsList.options.Add (new Dropdown.OptionData () { text = tag });
            }
            tagsList.value = 0;
            tagsList.RefreshShownValue();
        } else {
            Debug.LogWarning(this.name);
        }
    }
}
