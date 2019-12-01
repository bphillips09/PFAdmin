using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core.Util;
using System.Threading.Tasks;
using System.Threading;
using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.Json;
using PlayFab.AdminModels;
using PlayFab.MultiplayerModels;
using PlayFab.AuthenticationModels;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using SFB;
using UnityEngine.Networking;
using System.Linq;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Button createContainerButton;
    [SerializeField] private BuildBundleID buildID;
    [SerializeField] private GameObject loader;
    [SerializeField] private Text loaderMessage;
    [SerializeField] private GameObject segmentButton;
    [SerializeField] private GameObject playerButton;
    [SerializeField] private GameObject containerButton;
    [SerializeField] private GameObject segmentWindow; 
    [SerializeField] private GameObject playerWindow; 
    [SerializeField] private GameObject containerWindow; 
    [SerializeField] private GameObject playerInfoWindow; 
    [SerializeField] private GameObject deleteSelectedButton;
    [SerializeField] private GameObject informModal;
    [SerializeField] private GameObject informExecutableModal;
    [SerializeField] private Text informExecutableText;
    [SerializeField] private GameObject informTagsModal;
    [SerializeField] private Button informButton;
    [SerializeField] private GameObject informOkButton;
    [SerializeField] private GameObject informUrlButton;
    [SerializeField] private Text informText;
    [SerializeField] private Text informTagsText;
    [SerializeField] private InputField informField;
    [SerializeField] private GameObject informFieldModal;
    [SerializeField] private InputField informPolicyText;
    [SerializeField] private GameObject informPolicyModal;
    [SerializeField] private ContainerID informContainerDeletionID;
    [SerializeField] private GameObject informContainerDeletionModal;
    [SerializeField] private Text informContainerDeletionText;
    private PlayerID lastPlayerIdentifier;
    private bool selectAll = false;
    private int frames = 0;
    private string lastURL = "";
    [Header("Player InfoWindow")]
    [SerializeField] private Text displayName;
    [SerializeField] private Text lastLogin;
    [SerializeField] private Text creationDate;
    [SerializeField] private Text bannedUntil;
    [SerializeField] private Text playerID;
    [SerializeField] private Text location;
    [SerializeField] private Text originPlatform;
    [SerializeField] private Text currencies;
    [SerializeField] private Text deletionWarningText;
    [SerializeField] private GameObject deletionWarningModal;
    [SerializeField] private Image mapImage;
    [Header("Ban Window")]
    [SerializeField] private GameObject banModal;    
    [SerializeField] private Text banText;
    [SerializeField] private InputField banReason;
    [SerializeField] private InputField banTimeInHours;
    [Header("Currency Window")]
    [SerializeField] private GameObject currencyModal;    
    [SerializeField] private Text currencyText;
    [SerializeField] private InputField currencyType;
    [SerializeField] private InputField currencyNewValue;
    [Header("Username Window")]
    [SerializeField] private GameObject usernameModal;    
    [SerializeField] private Text usernameText;
    [SerializeField] private InputField usernameNewValue;
    [Header("Login Window")]
    [SerializeField] private GameObject loginWindow;
    [SerializeField] private GameObject twoFactorLoginWindow;
    [SerializeField] private InputField titleIDField;
    [SerializeField] private InputField secretKeyField;
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField passwordField;
    [Header("Build Select Window")]
    [SerializeField] private Text buildTitle;
    [SerializeField] private GameObject buildButton;
    [SerializeField] private GameObject buildView;
    [Header("Server Select Window")]
    [SerializeField] private GameObject serverButton;
    [SerializeField] private GameObject serverView;
    [SerializeField] private GameObject serverBuildSelection;
    [SerializeField] private Dropdown serverTagsDropdown;
    [SerializeField] private InputField serverStartCommandField;
    [SerializeField] private InputField serverLogCommandField;
    [Header("Server View Window")]
    private string lastServerBuildId = "";
    [SerializeField] private Text serverViewRegion;
    [SerializeField] private Text serverViewServerIdentifier;
    [SerializeField] private Text serverViewSessionIdentifier;
    [SerializeField] private Text serverViewState;
    [SerializeField] private Text serverViewConnectedPlayers;
    [SerializeField] private GameObject endSessionModal;
    [SerializeField] private ServerID serverViewID;
    private RegionID editRegionID;
    [Header("Region ID Window")]
    [SerializeField] private GameObject regionButton;
    [SerializeField] private GameObject editRegionWindow;
    [SerializeField] private Dropdown regionDropdown;
    [SerializeField] private InputField maxServersField;
    [SerializeField] private InputField standByServersField;
    private PortID editPortID;
    [Header("Port ID Window")]
    [SerializeField] private GameObject portButton;
    [SerializeField] private GameObject editPortWindow;
    [SerializeField] private Dropdown portDropdown;
    [SerializeField] private InputField portNameField;
    [SerializeField] private InputField portNumberField;
    private PolicyID editPolicyID;
    [Header("Policy ID Window")]
    [SerializeField] private GameObject policyButton;
    [SerializeField] private GameObject policyWindow;
    [SerializeField] private GameObject editPolicyWindow;
    [SerializeField] private Dropdown actionDropdown;
    [SerializeField] private Dropdown effectDropdown;
    [SerializeField] private InputField commentField;
    [SerializeField] private InputField resourceField;
    [SerializeField] private InputField principalField;
    private List<string> policyChanges;
    [Header("Asset Upload Window")]
    [SerializeField] private GameObject assetButton;
    [SerializeField] private GameObject assetView;
    [SerializeField] private GameObject uploadConfirmationWindow;
    [SerializeField] private Text uploadConfirmationMessageText;
    [SerializeField] private GameObject buildAssetButton;
    [SerializeField] private GameObject editAssetWindow;
    [SerializeField] private Text editAssetWindowTitle;
    [SerializeField] private InputField assetMountPathField;
    private AssetID editAssetID;
    private string lastFilePath;
    [Header("Studios Window")]
    [SerializeField] private GameObject studiosWindow;
    [SerializeField] private GameObject studioButton;
    [Header("Titles Window")]
    [SerializeField] private GameObject titlesWindow;
    [SerializeField] private GameObject titleButton;

    void Awake() {
        var args = System.Environment.GetCommandLineArgs();

        if (!args[0].Contains("-batchmode")) {
            return;
        }

        for (int i = 0; i < args.Length; i++) {
            Debug.Log (args[i]);
            if (args[i].Equals("-createcontainer")) {
                //GetContainerCredentialsWithToken();
            }
        }
    }

    void Start() {
        Application.targetFrameRate = 60;
        
        createContainerButton.interactable = !Application.isMobilePlatform;

        float height = Screen.currentResolution.height*0.75f;
        Screen.SetResolution((int)(height/1.4f), (int)height, false);

        string titleID = PlayerPrefs.GetString("titleID", null);
        string secretKey = PlayerPrefs.GetString("secretKey", null);

        if (string.IsNullOrEmpty(secretKey) ||
            string.IsNullOrEmpty(titleID)) {
            loginWindow.SetActive(true);
        } else {
            PlayFabSettings.TitleId = titleID;
            PlayFabSettings.DeveloperSecretKey = secretKey;
            Authenticate();
        }

        if (actionDropdown) {
            string[] actions = System.Enum.GetNames (typeof(PolicyAction));
            for(int i = 0; i < actions.Length; i++){
                actionDropdown.options.Add (new Dropdown.OptionData () { text = actions [i] });
            }
        }

        if (effectDropdown) {
            string[] effects = System.Enum.GetNames (typeof(EffectType));
            for(int i = 0; i < effects.Length; i++){
                effectDropdown.options.Add (new Dropdown.OptionData () { text = effects [i] });
            }
        }

        if (!PowerShellExists()) {
            //Inform("PowerShell not found!");
        }
    }

    void ShowLoader() {
        loaderMessage.text = "Loading...";
        loader.SetActive(true);
    }

    void ShowLoader(string message) {
        loaderMessage.text = message;
        loader.SetActive(true);
    }

    void HideLoader() {
        loader.SetActive(false);
    }

    string PowerShellDirectory() {
        string psDir = "";
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        psDir = "/Applications/PowerShell.app/Contents/MacOS/";
        #elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        psDir = "/usr/bin/";
        #else
        psDir = "C:/WINDOWS/system32/WindowsPowerShell/v1.0/";
        #endif
        
        return psDir;
    }

    string PowerShellPath() {
        string psDir = "";
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        psDir = "PowerShell.sh";
        #elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        psDir = "PowerShell.sh";
        #else
        psDir = "powershell.exe";
        #endif
        return PowerShellDirectory() + psDir;
    }

    bool PowerShellExists() {
        return Directory.Exists(PowerShellDirectory());        
    }

    public void Login() {
        if (!string.IsNullOrEmpty(titleIDField.text) && !string.IsNullOrEmpty(secretKeyField.text)) {
            PlayFabSettings.TitleId = titleIDField.text;
            PlayFabSettings.DeveloperSecretKey = secretKeyField.text;
            PlayerPrefs.SetString("titleID", PlayFabSettings.TitleId);
            PlayerPrefs.SetString("secretKey", PlayFabSettings.DeveloperSecretKey);
            Authenticate();
        } else if (!string.IsNullOrEmpty(usernameField.text) && !string.IsNullOrEmpty(passwordField.text)) {
            StartCoroutine(PFLogin("https://editor.playfabapi.com", 
                                   "/DeveloperTools/User/Login", 
                                   RequestType.LoginRequest,
                new LoginRequest{

                Email = usernameField.text,
                Password = passwordField.text,
                DeveloperToolProductName = "PFAdmin",
                DeveloperToolProductVersion = Application.version,
                TwoFactorAuth = ""
            }));
        } else {
            Inform ("Please use valid credentials to login.");
        }
    }

    public void Logout() {
        PlayFabSettings.TitleId = "";
        PlayFabSettings.DeveloperSecretKey = "";
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void Exit() {
        Application.Quit();
    }

    public void Authenticate() {
        ShowLoader();
        var request = new GetEntityTokenRequest();
        PlayFabAuthenticationAPI.GetEntityToken(request, 
        result => {
            Debug.Log("GOT TOKEN OK: " + result.ToJson().ToString());
            Inform ("Authentication Success!\n\nExpires " + result.TokenExpiration.Value.ToLocalTime().ToString());
            HideLoader();
            loginWindow.SetActive(false);
            DestroyStudioWindow();
            DestroyTitlesWindow();
        }, error => {
            HideLoader();
            loginWindow.SetActive(true);
            Inform("GET TOKEN FAILED: " + error.ErrorMessage);
            Debug.LogError("GET TOKEN FAILED: " + error.ToString());
        });
    }

    public void UploadAsset() {
        ShowLoader("Waiting for file...");
        string desktopDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", desktopDir, "zip", false, (string[] paths) => {
           if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0])) {
                lastFilePath = paths[0];
                ShowUploadConfirmation(Path.GetFileName(lastFilePath));
                HideLoader();
            } else {
                HideLoader();
            } 
        });
    }

    public void ShowAssets(bool selection) {
        ShowLoader("Loading Assets...");
        PlayFabMultiplayerAPI.ListAssetSummaries(new ListAssetSummariesRequest{
            PageSize = 50
        },
        result => {
            HideLoader();
            foreach (var asset in result.AssetSummaries) {
                GameObject newAssetButton = Instantiate(assetButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             assetButton.transform.parent) as GameObject;

                AssetID identity = newAssetButton.GetComponent<AssetID>();
                
                identity.fileName = asset.FileName;
                identity.metaData = asset.Metadata;
                identity.SetText($"{identity.fileName}");
                identity.SetSelectable(selection);
            
                newAssetButton.SetActive(true);
            }
            assetView.SetActive(true);
        },
        error => {
            Debug.LogError ("AssetSummaries Error: " + error.GenerateErrorReport());
        });
    }

    public void DeleteAsset(AssetID identity) {
        ShowLoader ("Deleting Asset...");
        PlayFabMultiplayerAPI.DeleteAsset(new DeleteAssetRequest{
            FileName = identity.fileName
        },
        result => {
            HideLoader();
            Destroy (identity.gameObject);
        },
        error => {
            Debug.LogError ("Delete Error: " + error.GenerateErrorReport());
            Inform ("Error: " + error.ErrorMessage);
        });
    }

    void ShowUploadConfirmation(string fileName) {
        uploadConfirmationMessageText.text = $"Are you sure you want to upload {fileName}?";
        uploadConfirmationWindow.SetActive(true);
    }

    public void ConfirmUpload() {
        ShowLoader("Getting Upload URL...");
        string fileName = Path.GetFileName(lastFilePath);
        PlayFabMultiplayerAPI.GetAssetUploadUrl(new GetAssetUploadUrlRequest{
            FileName = fileName
        },
        result => {
            Debug.Log ("Got Upload URL: " + result.AssetUploadUrl);

            string SASToken = result.AssetUploadUrl;
            SASToken = SASToken.Remove(SASToken.LastIndexOf("&api"));
            string StorageAccountName = SASToken.Substring(8,SASToken.IndexOf("blob")-9);
            SASToken = SASToken.Substring(SASToken.IndexOf("sv"));

            StorageCredentials creds = new StorageCredentials(SASToken);
            CloudStorageAccount storageAccount = new CloudStorageAccount(creds, StorageAccountName, "core.windows.net", true);
    		CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
    		CloudBlobContainer container = blobClient.GetContainerReference("gameassets");
    		CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            ShowLoader("Reading File...");

            UploadToAzure(blockBlob);
        },
        error => {
            Debug.LogError("UploadURL Error: " + error.GenerateErrorReport());
            Inform ("Error: " + error.ErrorMessage);
        });
    }

    async void UploadToAzure(CloudBlockBlob blockBlob) {
        long fileSize = new FileInfo(lastFilePath).Length;

        ShowLoader("Uploading...\nPlease Wait.");

        try {
            var progressHandler = new Progress<StorageProgress>(
            value => {
                ShowLoader(string.Format("Uploading... {0}%", (100 * value.BytesTransferred / fileSize).ToString()));
            });

            var progress = progressHandler as IProgress<StorageProgress>;

            CancellationToken caToken = new CancellationToken();

            await Task.Run(() => {
                Thread.Sleep(500);
            });

    	    await blockBlob.UploadFromFileAsync(
                lastFilePath, 
                AccessCondition.GenerateEmptyCondition(), 
                new BlobRequestOptions{
                    ParallelOperationThreadCount = 1
                }, 
                null, 
                progress, 
                caToken
            );
        } catch (System.Exception e) {
            Debug.LogError (e);
            Inform ("Upload Error:\n\n" + e.Message);
        }

        Inform ("Upload Complete!");
    }

    public void ListContainerImages() {
        ShowLoader();
        PlayFabMultiplayerAPI.ListContainerImages(new ListContainerImagesRequest{
            PageSize = 50
        },
        result => {
            HideLoader();
            Debug.Log ("GOT IMAGES OK: " + result.ToJson().ToString());
            foreach (var container in result.Images) {
                GameObject newContainerButton = Instantiate(containerButton, 
                                                Vector3.zero, Quaternion.identity, 
                                                containerButton.transform.parent) as GameObject;

                ContainerID identity = newContainerButton.GetComponent<ContainerID>();
                identity.containerName = container;
                identity.text.text = container;
                newContainerButton.SetActive(true);
            }
            containerWindow.SetActive(true);
        },
        error => {
            HideLoader();
            Inform("GET IMAGES FAILED: " + error.ErrorMessage);
            Debug.LogError ("GET IMAGES FAILED: " + error.ToString());
        });
    }

    public void DeleteContainerImage(ContainerID identity) {
        informContainerDeletionID.containerName = identity.containerName;
        informContainerDeletionText.text = $"Are you sure you want to delete container image \"{identity.containerName}\"?";
        informContainerDeletionModal.SetActive(true);
    }

    public void ConfirmDeleteContainerImage(ContainerID identity) {
        ShowLoader("Getting Credentials...");
        PlayFabMultiplayerAPI.GetContainerRegistryCredentials(new GetContainerRegistryCredentialsRequest(),
        result => {
            Debug.Log("GOT CREDENTIALS OK: " + result.ToJson());
            StartCoroutine(DeleteContainerImageRequest(identity, result.Username, result.Password, result.DnsName));
        },
        error => {
            Debug.LogError(error.GenerateErrorReport());
            Inform("Error:\n\n" + error.ErrorMessage);
        });
    }

    IEnumerator DeleteContainerImageRequest(ContainerID identity, string user, string pass, string repo) {
        ShowLoader("Deleting...");

        string url = $"https://{repo}/acr/v1/{identity.containerName}";

        Debug.Log("Deletion URL: " + url);

        using (UnityWebRequest www = UnityWebRequest.Delete(url)) {
            string auth = BasicAuthenticate(user, pass);
            www.SetRequestHeader("AUTHORIZATION", auth);

            Debug.Log("AUTHORIZATION: " + auth);

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
    
            while (!www.isDone) {
                ShowLoader("Deleting Container...");
                yield return null;
            }

            if (www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
                Inform ("Delete Error:\n\n" + www.error);
                foreach (var header in www.GetResponseHeaders()) {
                    Debug.LogWarning(header);
                }
            } else {
                HideLoader();
                Debug.Log ("Deleted Image: " + identity.containerName);
                Transform containerParent = containerButton.transform.parent;
                for (int i = 0; i < containerParent.childCount; i++) {
                    if (containerParent.GetChild(i).GetComponent<ContainerID>().containerName.Equals(identity.containerName)) {
                        Destroy(containerParent.GetChild(i).gameObject);
                    }
                }
            }
        }
    }

    private string BasicAuthenticate(string username, string password) {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    public void GetContainerTags(ContainerID identity) {
        ShowLoader();
        PlayFabMultiplayerAPI.ListContainerImageTags(new ListContainerImageTagsRequest{
            ImageName = identity.containerName
        },
        result => {
            Debug.Log ("GOT TAGS OK: " + result.ToJson());
            HideLoader();
            identity.EnumerateTags(result.Tags);
            InformContainer(string.Format("Select Tag for '<b>{0}</b>'", identity.containerName), identity);
        },
        error => {
            HideLoader();
            Debug.LogError("GET TAGS FAILED: " + error.ToString());
            Inform("Failed to retrieve information for the container!\n\n" + error.ErrorMessage);
        });
    }

    private T GetEnumValue<T>(string str) where T : struct {   
        try  {   
            T res = (T)System.Enum.Parse(typeof(T), str);   
            if (!System.Enum.IsDefined(typeof(T), res)) return default(T);   
            return res;   
        } catch {   
            return default(T);   
        }   
    }   

    public void IgnoreExectuableAsset(BuildBundleID identity) {
        identity.overrideChmod = true;
        CreateBuildWithCustomContainer(identity);
    }

    public void FixExecutableAsset(BuildBundleID identity) {
        string startCommand = serverStartCommandField.text;
        string logCommand = serverLogCommandField.text;
        serverStartCommandField.text = $"chmod +x {startCommand};{startCommand}" + 
            (string.IsNullOrEmpty(logCommand) ? "" : $" {logCommand}");

        CreateBuildWithCustomContainer(identity);
    }

    public void CreateBuildWithCustomContainer(BuildBundleID identity) {
        ShowLoader();
        try {
            PortID[] configuredPorts = portButton.transform.parent.GetComponentsInChildren<PortID>(false);
            RegionID[] configuredRegions = regionButton.transform.parent.GetComponentsInChildren<RegionID>(false);
            AssetID[] configuredAssets = buildAssetButton.transform.parent.GetComponentsInChildren<AssetID>(false);
            
            if (configuredPorts.Length == 0) {
                Inform ("Error: Ports cannot be empty! Please add a port.");
                return;
            }

            if (configuredRegions.Length == 0) {
                Inform ("Error: Regions cannot be empty! Please add a region.");
                return;
            }

            if (configuredAssets.Length == 0) {
                Inform ("Error: Assets cannot be empty! Please add an asset.");
                return;
            }

            if (string.IsNullOrEmpty(serverStartCommandField.text)) {
                Inform ("Error: Start Game Command cannot be empty! Please add a command. (Example: /data/Assets/GameServer.x86_64");
                return;
            }

            List<Port> portList = new List<Port>();
            List<BuildRegionParams> regionList = new List<BuildRegionParams>();
            List<AssetReferenceParams> assetList = new List<AssetReferenceParams>();

            foreach (var port in configuredPorts) {
                portList.Add(port.portIDParams);
            }

            foreach (var region in configuredRegions) {
                regionList.Add(region.regionIDParams);
            }

            foreach (var asset in configuredAssets) {
                assetList.Add(new AssetReferenceParams{ 
                    FileName = asset.fileName,
                    MountPath = asset.mountPath
                });
            }

            if (!serverStartCommandField.text.Contains("chmod") && !identity.overrideChmod) {
                InformExecutable("Your server asset is not marked as executable!");
                return;
            }

            PlayFabMultiplayerAPI.CreateBuildWithCustomContainer(new CreateBuildWithCustomContainerRequest{
                BuildName = identity.buildName.text,
                ContainerFlavor = GetEnumValue<ContainerFlavor>(identity.containerFlavor.options[identity.containerFlavor.value].text),
                ContainerImageReference = new ContainerImageReference{
                    ImageName = identity.containerName.text,
                    Tag = identity.containerTag.text
                },
                ContainerRunCommand = serverStartCommandField.text,
                MultiplayerServerCountPerVm = int.Parse(identity.serverCountPerVm.text),
                VmSize = GetEnumValue<AzureVmSize>(identity.vmSize.options[identity.vmSize.value].text),
                GameAssetReferences = assetList,
                Ports = portList,
                RegionConfigurations = regionList
            }, 
            result => {
                Debug.Log ("CREATE BUILD OK: " + result.ToJson());
                buildID.gameObject.SetActive(false);
                InformURL("Build Created Successfully!\n\nBuild ID:\n" + result.BuildId, 
                string.Format("https://developer.playfab.com/en-US/{0}/multiplayer/server/builds", PlayFabSettings.TitleId));
            },
            error => {
                Debug.LogError ("CREATE BUILD FAILURE: " + error.ToString());
                Inform("Build Creation Failure!\n\n" + error.ErrorMessage);
            });
        } catch (System.Exception e) {
            Inform (e.Message);
        }
    }

    public void AddAsset() {
        ShowAssets(true);
    }

    public void OnSelectAsset(AssetID identity) {
        DestroyAssetWindow();
        GameObject newAssetButton = Instantiate(buildAssetButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             buildAssetButton.transform.parent) as GameObject;

        AssetID newAsset = newAssetButton.GetComponent<AssetID>();
        newAsset.fileName = identity.fileName;
        newAsset.mountPath = identity.mountPath;
        identity.SetText($"{identity.fileName}");
        EditAsset(newAsset);
        newAssetButton.SetActive(true);
    }

    public void DeleteAssetFromBuild(AssetID identity) {
        Destroy(identity.gameObject);
    }

    public void EditAsset(AssetID identity) {
        editAssetID = identity;
        editAssetWindowTitle.text = identity.fileName;
        if (string.IsNullOrEmpty(identity.mountPath)) {
            identity.mountPath = "/data/Assets/";
        }
        assetMountPathField.text = identity.mountPath;
        editAssetWindow.SetActive(true);
    }

    public void SaveAssetConfirmation() {
        try {
            editAssetID.mountPath = assetMountPathField.text;
            editAssetID.SetText(editAssetID.fileName);
            editAssetWindow.SetActive(false);
        } catch (System.Exception e) {
            Inform ("Error: " + e.Message);
        }
    }

    public void AddRegion() {
        GameObject newRegionButton = Instantiate(regionButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             regionButton.transform.parent) as GameObject;

        RegionID identity = newRegionButton.GetComponent<RegionID>();
        identity.UpdateRegion(AzureRegion.EastUs, 4, 4);
        EditRegion(identity);
        newRegionButton.SetActive(true);
    }   

    public void AddPort() {
        GameObject newPortButton = Instantiate(portButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             portButton.transform.parent) as GameObject;

        PortID identity = newPortButton.GetComponent<PortID>();
        identity.UpdatePort("game", 8080, ProtocolType.TCP);
        EditPort(identity);
        newPortButton.SetActive(true);
    }   

    public void DeleteRegion(RegionID identifier) {
        Destroy(identifier.gameObject);
    }

    public void DeletePort(PortID identifier) {
        Destroy(identifier.gameObject);
    }

    public void EditRegion(RegionID identifier) {
        editRegionID = identifier;
        for (int i = 0; i < regionDropdown.options.Count; i++) {
            if (regionDropdown.options[i].text.Equals(identifier.region.ToString())) {
                regionDropdown.value = i;
                regionDropdown.RefreshShownValue();
            }
        }
        maxServersField.text = identifier.maxServers.ToString();
        standByServersField.text = identifier.standByServers.ToString();
        editRegionWindow.SetActive(true);
    }

    public void EditPort(PortID identifier) {
        editPortID = identifier;
        for (int i = 0; i < portDropdown.options.Count; i++) {
            if (portDropdown.options[i].text.Equals(identifier.protocol.ToString())) {
                portDropdown.value = i;
                portDropdown.RefreshShownValue();
            }
        }

        portNameField.text = identifier.portName.ToString();
        portNumberField.text = identifier.number.ToString();
        editPortWindow.SetActive(true);
    }

    public void SaveRegion(BuildBundleID identity) {
        AzureRegion newRegion = GetEnumValue<AzureRegion>(identity.region.options[identity.region.value].text);
        
        try {
            editRegionID.UpdateRegion(newRegion, int.Parse(maxServersField.text), int.Parse(standByServersField.text));
            editRegionWindow.SetActive(false);
        } catch (System.Exception e) {
            Inform ("Error: " + e.Message);
        }
    }

    public void SavePort(BuildBundleID identity) {
        ProtocolType newPortType = GetEnumValue<ProtocolType>(identity.portProtocol.options[identity.portProtocol.value].text);
        
        try {
            editPortID.UpdatePort(portNameField.text, int.Parse(portNumberField.text), newPortType);
            editPortWindow.SetActive(false);
        } catch (System.Exception e) {
            Inform ("Error: " + e.Message);
        }
    }

    void RunPowershellSetup(string dockerPath) {
        if (!PowerShellExists()) {
            Inform ("Unable to launch: PowerShell not detected!");
            return;
        }

        var ps1File = dockerPath + "DockerCommands.ps1";
        if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer) {
            var startInfo = new ProcessStartInfo() {
                FileName = PowerShellPath(),
                Arguments = $"-NoProfile -NoExit -ExecutionPolicy unrestricted -File \"{ps1File}\"",
                UseShellExecute = false
            };

            Process.Start(startInfo);
        } else {
            StartCoroutine(WaitForPSMac(ps1File));
        }
    }

    public void GetContainerCredentialsWithToken() {
        ShowLoader();
        PlayFabMultiplayerAPI.GetContainerRegistryCredentials(new GetContainerRegistryCredentialsRequest(),
        result => {
            Debug.Log ("GOT CREDS OK: " + result.ToJson().ToString());
            HideLoader();

            Inform ("Ready to build container!");
            string dockerPath = Application.dataPath + 
            (Application.platform == RuntimePlatform.OSXPlayer ? "/.." : "") + "/../Docker/"; 
            string path = dockerPath + "Credentials";
            string credentials = string.Format("Repo={0}\n" + 
                                               "User={1}\n" +
                                               "Pass={2}", 
                                               result.DnsName,
                                               result.Username,
                                               result.Password);
            try {
                File.WriteAllText(path, credentials);
                RunPowershellSetup(dockerPath);
            } catch (System.Exception e) {
                Inform (e.Message);
            }
        },
        error => {
            Debug.LogError ("GET CREDS FAILED: " + error.ToString());
            HideLoader();
            Inform("GET CREDENTIALS FAILED: " + error.ErrorMessage);
        });
    }

    public void GetAPIPolicy() {
        ShowLoader();
        PlayFabAdminAPI.GetPolicy(new GetPolicyRequest{
            PolicyName = "ApiPolicy"
        },
        result => {
            HideLoader();
            Debug.Log ("Got Policy OK: " + result.ToJson());
            foreach (var policy in result.Statements) {
                GameObject newPolicyButton = Instantiate(policyButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             policyButton.transform.parent) as GameObject;

                PolicyID identity = newPolicyButton.GetComponent<PolicyID>();
                identity.SetPolicy(policy.Action, policy.ApiConditions,
                policy.Comment, policy.Effect, policy.Principal, policy.Resource);
                newPolicyButton.SetActive(true);
            }
            if (result.Statements.Count > 0) {
                policyChanges = new List<string>();
                policyWindow.SetActive(true);
            }
        },
        error => {
            Debug.LogError ("Error Getting Policy: " + error.GenerateErrorReport());
            Inform ("Error Getting Policy!\n\n" + error.ErrorMessage);
        });
    }

    public void DeletePolicy() {
        if (editPolicyID) {
            TrackPolicyModifier($"Modified: DELETED {editPolicyID.Resource}");
            Destroy (editPolicyID.gameObject);
            editPolicyWindow.SetActive(false);
        }
    }

    public void AddPolicy() {
        GameObject newPolicyButton = Instantiate(policyButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             policyButton.transform.parent) as GameObject;

        PolicyID identity = newPolicyButton.GetComponent<PolicyID>();
        identity.SetPolicy("*", null, "", EffectType.Deny, "*", "pfrn:api--*");
        EditPolicy(identity);
    }  

    public void SavePolicy(PolicyID identity) {
        PolicyAction newActionType = GetEnumValue<PolicyAction>(actionDropdown.options[actionDropdown.value].text);
        EffectType newEffectType = GetEnumValue<EffectType>(effectDropdown.options[effectDropdown.value].text);
        
        try {
            editPolicyID.SetPolicy(GetPolicyActionString(newActionType), null,
                commentField.text, newEffectType, principalField.text, resourceField.text);

            if (editPolicyID) {
                editPolicyID.gameObject.SetActive(true);
            }

            TrackPolicyModifier($"Modified: {identity.Effect.ToString()}: {identity.Resource}");

            editPolicyWindow.SetActive(false);
        } catch (System.Exception e) {
            Inform ("Error: " + e.Message);
        }
    }

    public void MovePolicyUp(PolicyID identity) {
        Transform transformIdentity = identity.transform;
        int index = transformIdentity.GetSiblingIndex();
        int childCount = transformIdentity.parent.childCount;
        if (index != 1) {
            transformIdentity.SetSiblingIndex(index-1);
            index = transformIdentity.GetSiblingIndex();
        }
        identity.downButton.interactable = !(index == childCount-1);
        identity.upButton.interactable = !(index == 1);
        TrackPolicyModifier($"Modified: Moved Up: {identity.Resource}");
    }

    public void MovePolicyDown(PolicyID identity) {
        Transform transformIdentity = identity.transform;
        int index = transformIdentity.GetSiblingIndex();
        int childCount = transformIdentity.parent.childCount;
        if (index != childCount-1) {
            transformIdentity.SetSiblingIndex(index+1);
            index = transformIdentity.GetSiblingIndex();
        }
        identity.downButton.interactable = !(index == childCount-1);
        identity.upButton.interactable = !(index == 1);
        TrackPolicyModifier($"Modified: Moved Down: {identity.Resource}");
    }

    private void TrackPolicyModifier(string modification) {
        if (policyChanges == null) {
            policyChanges = new List<string>();
        }

        policyChanges.Add(modification);
    }

    public void FinishEditingPolicies(bool update) {
        ShowLoader();

        List<PermissionStatement> policyList = new List<PermissionStatement>();
        Transform policyParent = policyButton.transform.parent;
        for (int i = 0; i < policyParent.childCount; i++) {
            if (policyParent.GetChild(i).gameObject.activeSelf) {
                PolicyID policyIdentity = policyParent.GetChild(i).GetComponent<PolicyID>();
                policyList.Add(policyIdentity.GetPermissionStatement());
            }
        }

        if (policyChanges != null && policyChanges.Count > 0) {
            if (!update) {
                string changes = "";
                foreach (string change in policyChanges) {
                    changes += change + "\n";
                }
                InformPolicyChanges (changes);
                return;
            }
        } else {
            Debug.Log ("No changes to API Policies!");
            HideLoader();
            DestroyPolicyWindow();
            return;
        }

        PlayFabAdminAPI.UpdatePolicy(new UpdatePolicyRequest{
            OverwritePolicy = true,
            PolicyName = "ApiPolicy",
            Statements = policyList
        },
        result => {
            Debug.Log ("Updated Policy OK: " + result.ToJson());
            HideLoader();
            DestroyPolicyWindow();
            Inform ("Successfully Updated API Policies!");
        },
        error => {
            Debug.LogError("Error: " + error.GenerateErrorReport());
            Inform ("ERROR: " + error.ErrorMessage);
        });
    }

    public static string GetPolicyActionString(PolicyAction policy) {
        string action = "*";
        switch (policy) {
            case PolicyAction.ALL:
            action = "*";
            break;
            case PolicyAction.Read:
            action = "Read";
            break;
            case PolicyAction.Write:
            action = "Write";
            break;
            case PolicyAction.Accept:
            action = "Accept";
            break;
            case PolicyAction.POST:
            action = "POST";
            break;
            case PolicyAction.GET:
            action = "GET";
            break;
            default:
            action = "*";
            break;
        }
        return action;
    }

    public static PolicyAction GetPolicyAction(string policy) {
        PolicyAction action = PolicyAction.ALL;
        switch (policy) {
            case "*":
            action = PolicyAction.ALL;
            break;
            case "Read":
            action = PolicyAction.Read;
            break;
            case "Write":
            action = PolicyAction.Write;
            break;
            case "Accept":
            action = PolicyAction.Accept;
            break;
            case "POST":
            action = PolicyAction.POST;
            break;
            case "GET":
            action = PolicyAction.GET;
            break;
            default:
            action = PolicyAction.ALL;
            break;
        }
        return action;
    }

    public void EditPolicy(PolicyID identity) {
        editPolicyID = identity;
        for (int i = 0; i < effectDropdown.options.Count; i++) {
            if (effectDropdown.options[i].text.Equals(identity.Effect.ToString())) {
                effectDropdown.value = i;
                effectDropdown.RefreshShownValue();
            }
        }
        for (int i = 0; i < actionDropdown.options.Count; i++) {
            if (actionDropdown.options[i].text.Equals(identity.Action.ToString())) {
                actionDropdown.value = i;
                actionDropdown.RefreshShownValue();
            }
        }

        resourceField.text = identity.Resource;
        commentField.text = identity.Comment;
        principalField.text = identity.Principal;
        editPolicyWindow.SetActive(true);
    }

    public void GetSegments() {
        ShowLoader();
        PlayFabAdminAPI.GetAllSegments(new GetAllSegmentsRequest(), 
        result => {
            Debug.Log ("GOT ALL SEGMENTS OK: " + result.ToJson().ToString());
            foreach (var segment in result.Segments) {
                GameObject newSegmentButton = Instantiate(segmentButton, Vector3.zero, Quaternion.identity, segmentButton.transform.parent) as GameObject;
                SegmentID identity = newSegmentButton.GetComponent<SegmentID>();
                identity.segmentID = segment.Id;
                identity.nameText.text = segment.Name;
                newSegmentButton.SetActive(true);
            }
            segmentWindow.SetActive(true);
            HideLoader();
        }, 
        error => {
            HideLoader();
            Debug.LogError("ERROR GETTING SEGMENTS: " + error.GenerateErrorReport());
            Inform(error.ErrorMessage);
        });
    }

    public void EnumerateServers(ListMultiplayerServersResponse result) {
        lastServerBuildId = JsonUtility.FromJson<ListMultiplayerServersRequest>(result.Request.ToJson()).BuildId;
        
        Debug.Log ("GOT SERVERS OK: " + result.ToJson().ToString());
        HideLoader();
        if (result.MultiplayerServerSummaries.Count > 0) {
            foreach (var server in result.MultiplayerServerSummaries) {
                GameObject newServerButton = Instantiate(serverButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             serverButton.transform.parent) as GameObject;

                ServerID identity = newServerButton.GetComponent<ServerID>();
                
                identity.Region = (AzureRegion)server.Region;
                identity.ServerIdentifier = server.ServerId;
                identity.SessionIdentifier = server.SessionId;
                identity.State = server.State;
                identity.connectedPlayers = server.ConnectedPlayers;
                identity.SetText($"{identity.Region.ToString()}\n<color={StateColor(identity.State)}><b>{identity.State}</b></color>",
                                    identity.ServerIdentifier, identity.connectedPlayers.Count);
            
                newServerButton.SetActive(true);
            }

            serverBuildSelection.SetActive(false);
            serverView.SetActive(true);
        } else {
            Inform ("No servers were found.");
        }
    }

    public void EnumerateBuilds(ListBuildSummariesResponse result) {
        lastServerBuildId = JsonUtility.FromJson<ListMultiplayerServersRequest>(result.Request.ToJson()).BuildId;
        
        Debug.Log ("GOT SERVERS OK: " + result.ToJson().ToString());
        HideLoader();

        foreach (var build in result.BuildSummaries) {
            foreach (var region in build.RegionConfigurations) {
                GameObject newBuildButton = Instantiate(buildButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             buildButton.transform.parent) as GameObject;

                ServerID identity = newBuildButton.GetComponent<ServerID>();

                identity.Region = (AzureRegion)region.Region;
                identity.ServerIdentifier = build.BuildId;
                identity.SetBuildText(string.Format("<b>{0}</b>\n{1}\n<i>{2}</i>", 
                                      build.BuildName, region.Region,
                                      region.Status), build.BuildId,
                                      region.CurrentServerStats);

                newBuildButton.SetActive(true);
            }
        }

        buildView.SetActive(true);
    }

    public void ViewServer(ServerID identity) {
        serverViewRegion.text = "Region: " + identity.Region.ToString();
        serverViewState.text = "State: " + identity.State;
        serverViewServerIdentifier.text = "Server: " + identity.ServerIdentifier;
        serverViewSessionIdentifier.text = "Session: " + identity.SessionIdentifier;

        string players = "";
        if (identity.connectedPlayers.Count > 0) {
            foreach (var player in identity.connectedPlayers) {
                players += player.PlayerId + "\n";
            }
        } else {
            players = "None Yet";
        }
        
        serverViewConnectedPlayers.text = "Players:\n" + players;

        if (identity.endSessionButton) {
            identity.endSessionButton.interactable = !string.IsNullOrEmpty(identity.SessionIdentifier);
        }

        serverViewID.Region = identity.Region;
        serverViewID.SessionIdentifier = identity.SessionIdentifier;
    }

    public void ConfirmEndServerSession(ServerID identity) {
        ShowLoader();
        Debug.Log ("End Session: " + identity.SessionIdentifier + "\nfor build: " + lastServerBuildId);
        PlayFabMultiplayerAPI.ShutdownMultiplayerServer(new ShutdownMultiplayerServerRequest{
            Region = identity.Region,
            BuildId = lastServerBuildId,
            SessionId = identity.SessionIdentifier
        },
        result => {
            Debug.Log ("SHUTDOWN OK: " + result.ToJson().ToString());
            Inform ("Server was shutdown successfully.");
        },
        error => {
            Debug.LogError ("SHUTDOWN ERROR: " + error.GenerateErrorReport());
            Inform ("Unable to shutdown server! " + error.ErrorMessage);
        });
    }

    private string StateColor(string state) {
        string colorString = "";
        switch (state) {
            case "Active":
            colorString = "green";
            break;

            case "StandingBy":
            colorString = "yellow";            
            break;

            default:
            colorString = "red";            
            break;
        }
        return colorString;
    }

    public void GetServers(ServerID identity) {
        ShowLoader();
        
        PlayFabMultiplayerAPI.ListMultiplayerServers(new ListMultiplayerServersRequest {
            PageSize = 50,
            Region = identity.Region,
            BuildId = identity.ServerIdentifier
        },
        result => {
            EnumerateServers(result);
        },
        error => {
            Debug.LogError ("ERROR GETTING BUILDS: " + error.GenerateErrorReport());
            HideLoader();
            Inform(error.ErrorMessage);
        });
    }

    public void ListBuilds() {
        ShowLoader();
        PlayFabMultiplayerAPI.ListBuildSummaries(new ListBuildSummariesRequest{
            PageSize = 50
        },
        result => {
            HideLoader();
            if (result.BuildSummaries.Count > 0) {
                EnumerateBuilds(result);
            } else {
                Inform ("No servers were found.");
            }
            Debug.Log ("Build summaries result: " + result.ToJson());
        },
        error => {
            Inform ("Error: " + error.ErrorMessage);
            Debug.LogError ("Build Summaries ERROR: " + error.GenerateErrorReport());
        });
    }

    public void SelectSegment(SegmentID id) {
        ShowLoader();
        PlayFabAdminAPI.GetPlayersInSegment(new GetPlayersInSegmentRequest{
            SegmentId = id.segmentID
        }, 
        result => {
            Debug.Log ("GOT SEGMENT PLAYERS OK: " + result.ToJson().ToString());
            foreach (var playerProfile in result.PlayerProfiles) {
                GameObject newPlayerButton = Instantiate(playerButton, 
                                             Vector3.zero, Quaternion.identity, 
                                             playerButton.transform.parent) as GameObject;

                PlayerID identity = newPlayerButton.GetComponent<PlayerID>();
                identity.avatarURL = playerProfile.AvatarUrl;
                if (playerProfile.BannedUntil.HasValue) {
                    identity.bannedUntil = playerProfile.BannedUntil.Value.ToLocalTime().ToString();
                } else {
                    identity.bannedUntil = "N/A";
                }
                identity.creationDate = playerProfile.Created.Value.ToLocalTime().ToString();
                identity.lastLogin = playerProfile.LastLogin.Value.ToLocalTime().ToString();
                identity.displayName = playerProfile.DisplayName;
                foreach (var location in playerProfile.Locations) {
                    identity.location = location.Value.City + ", " + 
                                        location.Value.CountryCode;
                    identity.latLong = location.Value.Latitude + "," +
                                        location.Value.Longitude;
                }
                string currencies = "";
                foreach (var currency in playerProfile.VirtualCurrencyBalances) {
                    currencies += "\n" + currency.Key + ": " + currency.Value;
                }
                identity.currencies = currencies;
                identity.originPlatform = playerProfile.Origination.Value.ToString();
                identity.playerID = playerProfile.PlayerId;
                identity.nameText.text = playerProfile.DisplayName;
                newPlayerButton.SetActive(true);
            }
            DestroySegmentWindow();
            playerWindow.SetActive(true);
            HideLoader();
        }, 
        error => {
            HideLoader();
            Debug.LogError("ERROR GETTING SEGMENT PLAYERS: " + error.GenerateErrorReport());
            Inform(error.ErrorMessage);
        });
    } 

    public void DestroySegmentWindow() {
        for (int i = 0; i < segmentButton.transform.parent.childCount; i++) {
            Transform child = segmentButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        segmentWindow.SetActive(false);
    }

    public void DestroyPolicyWindow() {
        for (int i = 0; i < policyButton.transform.parent.childCount; i++) {
            Transform child = policyButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        policyWindow.SetActive(false);
    }

    public void DestroyPlayerWindow() {
        for (int i = 0; i < playerButton.transform.parent.childCount; i++) {
            Transform child = playerButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        playerWindow.SetActive(false);
    }

    public void DestroyContainerWindow() {
        for (int i = 0; i < containerButton.transform.parent.childCount; i++) {
            Transform child = containerButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        containerWindow.SetActive(false);
    }

    public void DestroyStudioWindow() {
        for (int i = 0; i < studioButton.transform.parent.childCount; i++) {
            Transform child = studioButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        studiosWindow.SetActive(false);
    }

    public void DestroyTitlesWindow() {
        for (int i = 0; i < titleButton.transform.parent.childCount; i++) {
            Transform child = titleButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        titlesWindow.SetActive(false);
    }

    public void DestroyServerWindow() {
        for (int i = 0; i < serverButton.transform.parent.childCount; i++) {
            Transform child = serverButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        serverView.SetActive(false);
    }

    public void DestroyBuildWindow() {
        for (int i = 0; i < buildButton.transform.parent.childCount; i++) {
            Transform child = buildButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        buildView.SetActive(false);
    }

    public void DestroyAssetWindow() {
        for (int i = 0; i < assetButton.transform.parent.childCount; i++) {
            Transform child = assetButton.transform.parent.GetChild(i);
            if (child.gameObject.activeSelf) {
                Destroy(child.gameObject);
            }
        }
        assetView.SetActive(false);
    }

    IEnumerator WaitForPSMac(string ps1Location) {
        ProcessStartInfo startInfo = new ProcessStartInfo("osascript", 
        "-e 'tell application \"Terminal\"' -e 'do script \"pwsh -f " + ps1Location + "\"' -e 'end tell'");

        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = true;
 
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
       
        process.WaitForExit();
        yield return null;
    }

    void Update() {
        frames++;
        
        if (frames % 30 == 0) {
            Transform parent = playerButton.transform.parent;
            int numDisabled = 0;
            for (int i = 0; i < parent.childCount; i++) {
                if (parent.GetChild(i).GetComponentInChildren<Toggle>().isOn) {
                    if (!deleteSelectedButton.activeSelf) {
                        deleteSelectedButton.SetActive(true);
                    }
                } else {
                    numDisabled++;
                }
            }
            if (deleteSelectedButton.activeSelf && numDisabled == parent.childCount) {
                deleteSelectedButton.SetActive(false);
            }
            frames = 0;
        }
    }

    public void DeleteSelection() {
        Transform parent = playerButton.transform.parent;
        int numToggled = 0;
        for (int i = 0; i < parent.childCount; i++) {
            if (parent.GetChild(i).GetComponentInChildren<Toggle>().isOn) {
                numToggled++;
            }
        }
        deletionWarningText.text = "Are you sure you want to <i>PERMANENTLY</i> delete " + numToggled + " players?";
        deletionWarningModal.SetActive(true);
    }

    public void ConfirmDeleteSelected() {
        ShowLoader();
        Transform parent = playerButton.transform.parent;
        for (int i = 0; i < parent.childCount; i++) {
            if (parent.GetChild(i).GetComponentInChildren<Toggle>().isOn) {
                DeletePlayerImmediate(parent.GetChild(i).GetComponent<PlayerID>());
            }
        }
        playerInfoWindow.SetActive(false);
        deletionWarningModal.SetActive(false);
        HideLoader();
    }

    public void ToggleSelectAll() {
        selectAll = !selectAll;
        Transform parent = playerButton.transform.parent;
        for (int i = 0; i < parent.childCount; i++) {
            parent.GetChild(i).GetComponentInChildren<Toggle>().isOn = selectAll;
        }
    }

    public void DeletePlayerImmediate(PlayerID playerID) {
        Debug.LogFormat ("Call to delete player: {0} with ID {1}", playerID.nameText.text, playerID.playerID);
        PlayFabAdminAPI.DeleteMasterPlayerAccount(new DeleteMasterPlayerAccountRequest{
            PlayFabId = playerID.playerID
        },
        result => {
            Debug.Log ("DELETE PLAYER OK: " + result.ToJson().ToString());
            Destroy (playerID.gameObject);
        },
        error => {
            Debug.LogError ("DELETE PLAYER ERROR: " + error.GenerateErrorReport());
            Inform(string.Format("Unable to delete {0}! {1}", playerID.displayName, error.ErrorMessage));
        });
    }

    public void DeleteUser() {
        ShowLoader();
        Transform parent = playerButton.transform.parent;
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            if (child.GetComponent<PlayerID>().playerID.Equals(lastPlayerIdentifier.playerID)) {
                child.GetComponentInChildren<Toggle>().isOn = true;
            } else {
                child.GetComponentInChildren<Toggle>().isOn = false;
            }
        }
        HideLoader();
        DeleteSelection();
    }

    public void BanUser() {
        banText.text = "Add Ban for User: \"" + lastPlayerIdentifier.displayName + "\"";
        banModal.SetActive(true);
    }

    public void ConfirmBanUser() {
        var request = new BanRequest();
        try {
            if (!string.IsNullOrEmpty(banTimeInHours.text)) {
                request.DurationInHours = uint.Parse(banTimeInHours.text);
            }
        } catch (System.Exception) {
            request.DurationInHours = 0;
        }
        request.Reason = banReason.text;
        request.PlayFabId = lastPlayerIdentifier.playerID;
        var banList = new List<BanRequest>();
        banList.Add(request);
        PlayFabAdminAPI.BanUsers(new BanUsersRequest{
            Bans = banList
        }, 
        result => {
            banModal.SetActive(false);
            Debug.Log ("BAN USER OK: " + result.ToJson().ToString());
            Inform(string.Format("{0} was successfully banned for {1} hours for \"{2}\"", 
                                lastPlayerIdentifier.displayName, banTimeInHours.text, banReason.text));
        },
        error => {
            Debug.LogError("BAN PLAYER FAILED: " + error.ToString());
            Inform("Unable to ban user! " + error.ErrorMessage);
            banModal.SetActive(false);
        });
    }

    public void ModifyUserTitleName() {
        usernameText.text = string.Format("Change Display Name for {0}", lastPlayerIdentifier.displayName);
        usernameModal.SetActive(true);
    }

    public void ConfirmModifyUserTitleName() {
        if (usernameNewValue.text.ToCharArray().Length < 4) {
            Inform ("Username too short!");
            return;
        } else {
            PlayFabAdminAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest{
                PlayFabId = lastPlayerIdentifier.playerID,
                DisplayName = usernameNewValue.text
            },
            result => {
                Debug.Log ("UPDATE DISPLAY NAME OK: " + result.ToJson().ToString());
                Inform (string.Format("Successfully renamed {0} to {1}", 
                                    lastPlayerIdentifier.displayName, 
                                    usernameNewValue.text));
            },
            error => {
                Debug.LogError("UPDATE NAME FAILURE: " + error.ToString());
                Inform("Unable to update name! " + error.ErrorMessage);
            });
        }
    }

    public void ModifyCurrency() {
        currencyText.text = string.Format("Modify Currency for {0}\n\nCurrent Values:{1}", 
                            lastPlayerIdentifier.displayName, 
                            lastPlayerIdentifier.currencies);

        currencyModal.SetActive(true);
    }

    public void ConfirmModifyCurrency(bool add) {
        if (!string.IsNullOrEmpty(currencyType.text) && !string.IsNullOrEmpty(currencyNewValue.text)) {
            int newCurrencyValue = int.Parse(currencyNewValue.text);

            if (add) {
                PlayFabAdminAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest{
                    PlayFabId = lastPlayerIdentifier.playerID,
                    VirtualCurrency = currencyType.text,
                    Amount = newCurrencyValue
                },
                result => {
                    Debug.Log ("ADD CURRENCY OK: " + result.ToJson().ToString());
                    Inform (string.Format("Modified Currency for {0}\n\nLast Value: {1}\n\nNew Value: {2}",
                                            lastPlayerIdentifier.displayName, lastPlayerIdentifier.currencies, 
                                            result.Balance + " " + currencyType.text));
                },
                error => {
                    Debug.LogError("ADD CURRENCY FAILURE: " + error.ToString());
                    Inform ("Add Currency Failed! " + error.ErrorMessage);
                });
            } else {
                PlayFabAdminAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest{
                    PlayFabId = lastPlayerIdentifier.playerID,
                    VirtualCurrency = currencyType.text,
                    Amount = newCurrencyValue
                },
                result => {
                    Debug.Log ("REMOVE CURRENCY OK: " + result.ToJson().ToString());
                    Inform (string.Format("Modified Currency for {0}\n\nLast Value: {1}\n\nNew Value: {2}",
                                            lastPlayerIdentifier.displayName, lastPlayerIdentifier.currencies, 
                                            result.Balance + " " + currencyType.text));
                },
                error => {
                    Debug.LogError("REMOVE CURRENCY FAILURE: " + error.ToString());
                    Inform ("REMOVE Currency Failed! " + error.ErrorMessage);
                });
            }
        } else {
            Inform ("Fields cannot be empty!");
        }
    }

    public void GetUserInventory() {
        ShowLoader();
        PlayerID playerIdentifier = lastPlayerIdentifier;
        PlayFabAdminAPI.GetUserInventory(new GetUserInventoryRequest{
            PlayFabId = playerIdentifier.playerID
        },
        result => {
            HideLoader();
            Debug.Log ("GOT INVENTORY OK: " + result.ToJson());
            string items = "";
            int itemCount = 0;
            foreach (var item in result.Inventory) {
                itemCount++;
                items += string.Format("\n{0}. {1}", itemCount, item.DisplayName);
            }
            Inform(string.Format("\"{0}'s\" inventory:\n{1}", lastPlayerIdentifier.displayName, items));
        },
        error => {
            HideLoader();
            Debug.LogError ("GET INVENTORY FAILED: " + error.ToString());
            Inform ("Unable to get inventory for \"" + playerIdentifier.displayName + "\"!\n\n" + error.ErrorMessage);
        });
    }

    public IEnumerator PFLogin<TRequest>(string apiEndpoint, string api, RequestType requestType, TRequest request) {
        ShowLoader("Loading...");
        var url = apiEndpoint + api;
        var req = PlayFabSimpleJson.SerializeObject(request);

        var headers = new Dictionary<string, string> {
            {"Content-Type", "application/json"},
            {"X-ReportErrorAsSuccess", "true"}
        };

        var payload = System.Text.Encoding.UTF8.GetBytes(req.Trim());
        var www = new UnityWebRequest(url) {
            uploadHandler = new UploadHandlerRaw(payload),
            downloadHandler = new DownloadHandlerBuffer(),
            method = "POST"
        };

        foreach (var header in headers) {
            if (!string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value)) {
                www.SetRequestHeader(header.Key, header.Value);
            } else {
                UnityEngine.Debug.LogWarning("Null header");
            }
        }

        if (www != null) {
            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error)) {
                Debug.LogError(www.error);
                Inform("Error: " + www.error);
            } else {
                Debug.Log("Got Response: " + www.downloadHandler.text);
                var httpResult = PlayFabSimpleJson.DeserializeObject<HTTPResponse>(www.downloadHandler.text);

                if (httpResult.code != 200) {
                    var error = GeneratePlayFabError(www.downloadHandler.text);
                    Debug.Log (error.GenerateErrorReport());

                    if ((int)error.Error == 1246 || error.ErrorMessage.Contains("TwoFactor")) {
                        HideLoader();
                        twoFactorLoginWindow.SetActive(true);
                    } else {
                        Inform("Login Error:\n\n" + error.ErrorMessage);
                    }
                } else {
                    HideLoader();
                    
                    if (requestType == RequestType.LoginRequest) {
                        var dataJson = PlayFabSimpleJson.SerializeObject(httpResult.data);
                        var loginResult = PlayFabSimpleJson.DeserializeObject<PFLoginResult>(dataJson);
                        StartCoroutine(PFLogin("https://editor.playfabapi.com", 
                            "/DeveloperTools/User/GetStudios", RequestType.StudiosRequest,
                            new GetPFStudiosRequest{
                                DeveloperClientToken = loginResult.DeveloperClientToken
                            }
                        ));
                    } else {
                        var dataJson = PlayFabSimpleJson.SerializeObject(httpResult.data);
                        var studiosResult = PlayFabSimpleJson.DeserializeObject<GetPFStudiosResult>(dataJson);
                        foreach (var studio in studiosResult.Studios) {
                            GameObject newStudioButton = Instantiate(studioButton, 
                                                         Vector3.zero, Quaternion.identity, 
                                                         studioButton.transform.parent) as GameObject;

                            StudioID identity = newStudioButton.GetComponent<StudioID>();

                            identity.Id = studio.Id;
                            identity.Name = studio.Name;
                            identity.Titles = studio.Titles;
                            identity.SetText(studio.Name);
                            newStudioButton.SetActive(true);
                        }

                        studiosWindow.SetActive(true);
                    }
                }
            }
        } else {
            Debug.LogError("WebRequest was null");
            Inform("WebRequest was null");
        }
    }

    public void TwoFactorLogin(InputField twoFactorField) {
        StartCoroutine(PFLogin("https://editor.playfabapi.com", 
                               "/DeveloperTools/User/Login", 
                               RequestType.LoginRequest,
            new LoginRequest{

            Email = usernameField.text,
            Password = passwordField.text,
            DeveloperToolProductName = "PFAdmin",
            DeveloperToolProductVersion = Application.version,
            TwoFactorAuth = twoFactorField.text
        }));
    }

    public void SelectStudio(StudioID identity) {
        foreach (var title in identity.Titles) {
            GameObject newTitleButton = Instantiate(titleButton, 
                                         Vector3.zero, Quaternion.identity, 
                                         titleButton.transform.parent) as GameObject;
            TitleID titleIdentity = newTitleButton.GetComponent<TitleID>();
            titleIdentity.Id = title.Id;
            titleIdentity.Name = title.Name;
            titleIdentity.SecretKey = title.SecretKey;
            titleIdentity.GameManagerUrl = title.GameManagerUrl;
            titleIdentity.SetText(title.Name);
            newTitleButton.SetActive(true);
        }

        titlesWindow.SetActive(true);
    }

    public void SelectTitle(TitleID identity) {
        PlayFabSettings.TitleId = identity.Id;
        PlayFabSettings.DeveloperSecretKey = identity.SecretKey;

        PlayerPrefs.SetString("titleID", PlayFabSettings.TitleId);
        PlayerPrefs.SetString("secretKey", PlayFabSettings.DeveloperSecretKey);

        Authenticate();
    }

    public void ViewPlayer(PlayerID playerIdentifier) {
        lastPlayerIdentifier = playerIdentifier;
        displayName.text = "<b>\"" + playerIdentifier.displayName + "\"</b>";
        creationDate.text = "<b>Creation Date</b>: " + playerIdentifier.creationDate;
        lastLogin.text = "<b>Last Login</b>: " + playerIdentifier.lastLogin;
        location.text = "<b>Location</b>: " + playerIdentifier.location;
        originPlatform.text = "<b>Origin Platform</b>: " + playerIdentifier.originPlatform;
        currencies.text = "<b>Currencies</b>: " + playerIdentifier.currencies;
        bannedUntil.text = "<b>Banned Until</b>: <color=red>" + playerIdentifier.bannedUntil + "</color>";
        playerID.text = "<b>PlayFab ID</b>: " + playerIdentifier.playerID;
        StartCoroutine(GetMapImage(playerIdentifier.latLong));
        playerInfoWindow.SetActive(true);
    }

    IEnumerator GetMapImage(string latLong) {
        string apiKey = "UPdUOwnBZdPmID280PvZiwyQWkALryyR";
        string defaultSize = "425,500";
        string url = string.Format("https://open.mapquestapi.com/staticmap/v4/getmap?key={0}&size={1}&zoom=10&center={2}&mcenter={2}", apiKey, defaultSize, latLong);
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone) {
            mapImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            mapImage.preserveAspect = true;
        }
    }

    public void InformContainer(string message, ContainerID identity) {
        HideLoader();
        informTagsText.text = message;
        informTagsModal.GetComponent<ContainerID>().containerName = identity.containerName;
        informTagsModal.SetActive(true);
    }

    public void CreateNewBuild(ContainerID identity) {
        string containerName = identity.containerName;
        string containerTag = identity.tagsList.options[identity.tagsList.value].text;
        buildTitle.text = "Build Image: " + containerName;
        buildID.containerName.text = containerName;
        buildID.containerTag.text = containerTag;
        buildID.gameObject.SetActive(true);
    }

    public void GoUrl() {
        if (!string.IsNullOrEmpty(lastURL)) {
            Application.OpenURL(lastURL);
        }
    }

    public void InformURL(string message, string URL) {
        HideLoader();
        lastURL = URL;
        informText.text = message;
        informUrlButton.SetActive(true);
        informOkButton.SetActive(false);
        informModal.SetActive(true);
    }

    public void InformExecutable(string message) {
        HideLoader();
        informExecutableText.text = message;
        informExecutableModal.SetActive(true);
    }

    public void Inform(string message) {
        HideLoader();
        Debug.LogWarning(message);
        informText.text = message;
        informUrlButton.SetActive(false);
        informOkButton.SetActive(true);
        informModal.SetActive(true);
    }

    public void InformField(string message) {
        HideLoader();
        informField.text = message;
        informFieldModal.SetActive(true);
    }

    public void InformPolicyChanges(string changes) {
        HideLoader();
        informPolicyText.text = changes;
        informPolicyModal.SetActive(true);
    }

    public PlayFabError GeneratePlayFabError(string json, object customData = null) {
        JsonObject errorDict = null;
        Dictionary<string, List<string>> errorDetails = null;
        
        try {
            //deserialize the error
            errorDict = PlayFabSimpleJson.DeserializeObject<JsonObject>(json);
            if (errorDict.ContainsKey("errorDetails")) {
                var ed = PlayFabSimpleJson.DeserializeObject<Dictionary<string, List<string>>>(errorDict["errorDetails"].ToString());
                errorDetails = ed;
            }
        } catch (Exception e) {
            return new PlayFabError() {
                ErrorMessage = e.Message
            };
        }

        //create new error object
        return new PlayFabError {
            HttpCode = errorDict.ContainsKey("code") ? Convert.ToInt32(errorDict["code"]) : 400,
            HttpStatus = errorDict.ContainsKey("status")
                ? (string)errorDict["status"]
                : "BadRequest",
            Error = errorDict.ContainsKey("errorCode")
                ? (PlayFabErrorCode)Convert.ToInt32(errorDict["errorCode"])
                : PlayFabErrorCode.ServiceUnavailable,
            ErrorMessage = errorDict.ContainsKey("errorMessage")
                ? (string)errorDict["errorMessage"]
                : string.Empty,
            ErrorDetails = errorDetails,
            CustomData = customData ?? new object()
        };
    }
}

public class LoginRequest {
    public string Email;
    public string Password;
    public string TwoFactorAuth;
    public string DeveloperToolProductName;
    public string DeveloperToolProductVersion;
}

public class HTTPResponse {
    public int code;
    public string status;
    public object data;
}

public class PFLoginResult {
    public string DeveloperClientToken;
}

public class GetPFStudiosRequest {
    public string DeveloperClientToken;
}

public class GetPFStudiosResult {
    public PFStudio[] Studios;
}

public class PFTitle {
    public string Id;
    public string Name;
    public string SecretKey;
    public string GameManagerUrl;
}

public class PFStudio {
    public static PFStudio OVERRIDE = new PFStudio { Id = "", Name = "_OVERRIDE_", Titles = null };

    public string Id;
    public string Name;

    public PFTitle[] Titles;

    public PFTitle GetTitle(string titleId) {
        if (Titles == null)
            return null;
        for (var i = 0; i < Titles.Length; i++)
            if (Titles[i].Id == titleId)
                return Titles[i];
        return null;
    }

    public string GetTitleSecretKey(string titleId) {
        var title = GetTitle(titleId);
        return title == null ? "" : title.SecretKey;
    }
}

public enum RequestType {LoginRequest, StudiosRequest}