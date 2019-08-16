using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.MultiplayerModels;

public class RegionID : MonoBehaviour {
    public AzureRegion region;
    public int maxServers;
    public int standByServers;
    public BuildRegionParams regionIDParams;
    public Text buttonText;

    public void UpdateRegion(AzureRegion newRegion, int newMax, int newStandby) {
        region = newRegion;
        maxServers = newMax;
        standByServers = newStandby;

        if (regionIDParams == null) {
            regionIDParams = new BuildRegionParams();
        }

        regionIDParams.Region = newRegion;
        regionIDParams.MaxServers = newMax;
        regionIDParams.StandbyServers = newStandby;

        buttonText.text = $"<b>{newRegion}</b>\nMX:<b>{newMax}</b> - SB:<b>{newStandby}</b>";
    }
}
