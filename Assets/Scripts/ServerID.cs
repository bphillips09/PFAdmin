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

    public void SetBuildText(string nameAngRegionAndStatus, string buildID, CurrentServerStats stats) {
        regionAndStateText.text = nameAngRegionAndStatus;
        serverIDText.text = buildID;
        playerCountText.text = string.Format("{0} Active\n{1} Standby\n{2} Propping\n{3} Total",
                                stats.Active, stats.StandingBy, stats.Propping, stats.Total);
    }
}
