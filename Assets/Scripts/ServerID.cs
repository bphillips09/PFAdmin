using System.Collections;
using System.Collections.Generic;
using PlayFab.MultiplayerModels;
using UnityEngine;
using UnityEngine.UI;

public class ServerID : MonoBehaviour {
    public Button endSessionButton;
    [HideInInspector] public AzureRegion Region;
    [HideInInspector] public string ServerIdentifier;
    [HideInInspector] public string SessionIdentifier;
    [HideInInspector] public string State;
    [HideInInspector] public List<ConnectedPlayer> connectedPlayers;
    [SerializeField] private Text regionAndStateText;
    [SerializeField] private Text serverIDText;
    [SerializeField] private Text playerCountText;

    public void SetText(string regionAndState, string serverID, int players) {
        regionAndStateText.text = regionAndState;
        serverIDText.text = serverID;
        playerCountText.text = "Players\n" + players.ToString();
    }
}
