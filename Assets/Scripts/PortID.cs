using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.MultiplayerModels;

public class PortID : MonoBehaviour {
    public string portName;
    public int number;
    public ProtocolType protocol;
    public Port portIDParams;
    public Text buttonText;

    public void UpdatePort(string newName, int portNumber, ProtocolType newProtocol) {
        portName = newName;
        number = portNumber;
        protocol = newProtocol;

        if (portIDParams == null) {
            portIDParams = new Port();
        }

        portIDParams.Name = newName;
        portIDParams.Num = portNumber;
        portIDParams.Protocol = newProtocol;

        buttonText.text = $"{newName}\n<b>{newProtocol}</b> - {portNumber}";
    }
}
