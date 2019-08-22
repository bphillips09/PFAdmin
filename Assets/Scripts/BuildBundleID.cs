using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.MultiplayerModels;

public class BuildBundleID : MonoBehaviour {
    public InputField buildName;
    public Dropdown containerFlavor;
    public InputField containerName;
    public InputField containerTag;
    public InputField serverCountPerVm;
    public Dropdown region;
    public InputField maxServers;
    public InputField standByServers;
    public Dropdown vmSize;
    public InputField portNumber;
    public Dropdown portProtocol;
    public InputField[] allFields;

    void Start() {
        if (containerFlavor) {
            string[] containerFlavors = System.Enum.GetNames (typeof(ContainerFlavor));
            for(int i = 0; i < containerFlavors.Length; i++){
                containerFlavor.options.Add (new Dropdown.OptionData () { text = containerFlavors [i] });
            }
            
            containerFlavor.value = 1;
        }

        if (region) {
            string[] regions = System.Enum.GetNames (typeof(AzureRegion));
            for(int i = 0; i < regions.Length; i++){
                region.options.Add (new Dropdown.OptionData () { text = regions [i] });
            }
            
            region.value = 5;
        }

        if (vmSize) {
            string[] vmSizes = System.Enum.GetNames (typeof(AzureVmSize));
            for(int i = 0; i < vmSizes.Length; i++){
                vmSize.options.Add (new Dropdown.OptionData () { text = vmSizes [i] });
            }
            
            vmSize.value = 7;
        }

        if (portProtocol) {
            string[] portProtocols = System.Enum.GetNames (typeof(ProtocolType));
            for(int i = 0; i < portProtocols.Length; i++){
                portProtocol.options.Add (new Dropdown.OptionData () { text = portProtocols [i] });
            }
            
            portProtocol.value = 0;
        }
    }

    void Update() {
        foreach (var field in allFields) {
            if (!field) {
                return;
            }
            if (string.IsNullOrEmpty(field.text) && field.colors.normalColor != Color.red) {
                ColorBlock fieldBlock = field.colors;
                fieldBlock.normalColor = Color.red;
                field.colors = fieldBlock;
            } else {
                ColorBlock fieldBlock = field.colors;
                fieldBlock.normalColor = Color.white;
                field.colors = fieldBlock;
            }
        }
    }
}
