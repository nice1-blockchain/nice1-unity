using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AnchorLinkSharp;
using AnchorLinkTransportSharp.Src;
using AnchorLinkTransportSharp.Src.StorageProviders;
using AnchorLinkTransportSharp.Src.Transports.Canvas;
using EosioSigningRequest;
using EosSharp.Core.Api.v1;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniversalAuthenticatorLibrary;

public class Nice1LoginCanvas : MonoBehaviour
{

    #region DLL Imports

#if UNITY_ANDROID
    [DllImport("cppLib")]
    private static extern int CheckLicense(string owner, string author, string category, string license_name, string idata_name);

    [DllImport("cppLib")]
    private static extern int CheckNice1GenesisKey(string owner, string author, string category, string license_name, string idata_name, int checkNice1GenesisKey);

    [DllImport("cppLib")]
    private static extern string CheckLicensePlugin(string owner, string author, string category, string license_name, int checkNice1GenesisKey, int network);

#else
    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CheckLicense(string owner, string author, string category, string license_name, string idata_name);

    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CheckNice1GenesisKey(string owner, string author, string category, string license_name, string idata_name, int checkNice1GenesisKey);

    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr CheckLicensePlugin(string owner, string author, string category, string license_name, int checkNice1GenesisKey, int network);

#endif

    #endregion

    #region UI Objects
    /// <summary>
    /// Assign UnityTransport through the Editor
    /// </summary>
    [SerializeField] private UnityCanvasTransport Transport;

    [Header("Panels")]
    [SerializeField] private GameObject CustomActionsPanel;

    [Header("Buttons")]
    [SerializeField] private Button LoginButton;



    private Coroutine waitCoroutine;
    /// app identifier, should be set to the eosio contract account if applicable
    private const string Identifier = "example";

    /// initialize the link
    private AnchorLink _link;

    /// the session instance, either restored using link.restoreSession() or created with link.login()
    private LinkSession _session;

    [HideInInspector]
    public bool hasNice1Key { get; private set; }

    [HideInInspector]
    public string owner { get; private set; }
    #endregion

    #region License Parameters
    public enum Network
    {
        Jungle4, Eos, Proton_testnet, Proton, Wax_testnet, Wax, Telos_testnet, Telos
    }
    [Header("Network")]
    public Network network;
    // Proton testnet: https://proton-testnet.eosio.online/endpoints
    private List<string> networkEndpoints = new List<string>
    {
        "https://jungle4.api.eosnation.io",
        "https://eos.greymass.com/v1/",
        "https://protontestnet.greymass.com/v1/",
        "https://proton.cryptolions.io/v1/",
        "https://testnet.waxsweden.org/v1/",
        "https://wax.greymass.com/v1/",
        "https://test.telos.eosusa.io/v1/",
        "https://telos.greymass.com/v1/"
    };
    private List<string> networkChainIds = new List<string>
    {
        "73e4385a2708e6d7048834fbc1079f2fabb17b3c125b146af438971e90716c4d", //Jungle4
        "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906", //Eos
        "e0a0743522e90db0a1802632b90fc48272957f9c32e4d35639d769546c10b763", //Proton_testnet
        "384da888112027f0321850a169f737c33e53b388aad48b5adace4bab97f437e0", //Proton"
        "f16b1833c747c43682f4386fca9cbb327929334a762755ebec17f6f23c9b8a12", //Wax_testNet
        "1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4", //Wax
        "1eaa0824707c8c16bd25145493bf062aecddfeb56c736f6ba6397f3195f33c9f", //Telos_testnet
        "4667b205c6838ef70ff7988f6e8257e8be0e1284a2f59699054a018f743b1d11", //Telos
    };
    [Header("License - Mandatory fields")]
    public string AUTHOR = "niceonedemos";
    public string IDATA_NAME = "GAME LICENSE - LegendaryLegends";
    public string CATEGORY = "llegends";

    [Header("Nice1 Genesis Key")]
    public bool checkNice1GenesisKey_bool;

    [Header("FreeToPlay")]
    public bool freeLicense_bool;

    #endregion

    private EventSystem _canvasEventSystem;

    public WalletAccount CurrentAccount { get; private set; }

    private void Start()
    {
        print(((int)network));
        int netWorkInt = (int)network;
        if (netWorkInt % 2 == 0)
        {
            SigningRequestConstants.ChainIdLookup.Add(ChainName.Unknown, networkChainIds[(int)network]);
            print("added");
        }
        _canvasEventSystem = EventSystem.current;
        CurrentAccount = new WalletAccount();
    }
    /// Initialize a new session
    public async void StartSession()
    {
        _link = new AnchorLink(new LinkOptions()
        {
            Transport = this.Transport,
            // Uncomment this for and EOS session
            //ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
            //          "EOS8MnhYsRe6AUqCNSP1pQe3YSr2aHRWbqgZKX42Ti4h3SXXyUQBL"
            //Rpc = "https://eos.greymass.com",

            // WAX session
            //ChainId = "1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4",
            //Rpc = "https://api.wax.liquidstudios.io",

            //ChainId = "",
            ChainId = networkChainIds[(int)network],
            Rpc = "",
            ZlibProvider = new NetZlibProvider(),
            Storage = new PlayerPrefsStorage()
        });


        await Login();
    }

    /// Login and store session if sucessful
    private async Task Login()
    {
        var loginResult = await _link.Login(Identifier);
        print(loginResult.Session + " / " + Identifier);
        _session = loginResult.Session;
        DidLogin();
    }

    /// <summary>
    /// Call from UI button
    /// </summary>
    public async void RestoreASession()
    {
        await RestoreSession();
    }

    /// <summary>
    /// Tries to restore session, called when document is loaded
    /// </summary>
    /// <returns></returns>
    private async Task RestoreSession()
    {
        var restoreSessionResult = await _link.RestoreSession(Identifier);
        _session = restoreSessionResult;
        if (_session != null)
            DidLogin();
    }


    /// Call from UI button
    public async void DoLogout()
    {
        await Logout();
    }

    /// Logout and remove session from storage
    private async Task Logout()
    {
        await _session.Remove();
    }

    /// Called when session was restored or created
    private void DidLogin()
    {
        Debug.Log($"{_session.Auth.actor} logged-in");
        SetAccount(_session.Auth.actor);

    }

    /// Use this to toggle on a new rect (or a gameobject) in the canvas
    public void ShowTargetPanel(GameObject targetPanel)
    {
        Transport.SwitchToNewPanel(targetPanel);
    }

    // Switches from one panel and activates another one. Use the float parameter to delay the next panel from showing immediately
    private IEnumerator SwitchPanels(GameObject fromPanel, GameObject toPanel, float SecondsToWait = 0.1f)
    {
        Debug.Log("Start counter");
        yield return new WaitForSeconds(SecondsToWait);

        Transport.DisableTargetPanel(fromPanel, toPanel);
    }

    /// <summary>Called when ctrl + v is pressed in browser (webgl)</summary>
    public void OnBrowserClipboardPaste(string pastedText)
    {
        if (_canvasEventSystem.currentSelectedGameObject?.GetComponent<TMP_InputField>() != null)
            _canvasEventSystem.currentSelectedGameObject.GetComponent<TMP_InputField>().text = pastedText;
    }

    #region License 

    public void SetAccount(string name)
    {
        CurrentAccount.Initialize(name, null, null, null, false, null);
        if (freeLicense_bool)
        {
            Debug.Log("FREE LICENSE");
            LicenseOK();
        }
        else
            StartCoroutine(SearchAssetsByOwner(name));
    }

    private IEnumerator SearchAssetsByOwner(string owner)
    {
#if UNITY_ANDROID
        string licenseResult = CheckLicensePlugin(owner, AUTHOR, CATEGORY, IDATA_NAME, Convert.ToInt32(checkNice1GenesisKey_bool), (int)network);
#else
        string licenseResult = Marshal.PtrToStringAnsi(CheckLicensePlugin(owner, AUTHOR, CATEGORY, IDATA_NAME, Convert.ToInt32(checkNice1GenesisKey_bool), (int)network));
#endif

        Debug.Log(licenseResult);
        if (licenseResult == "LICENSE" || licenseResult == "NICE1KEY")
        {
            if (licenseResult.Equals("NICE1KEY"))
                hasNice1Key = true;
            this.owner = owner;

            LicenseOK();
        }
        else
        {
            NO_License();
        }

        yield return new WaitForSeconds(0);
    }

    public void LicenseOK()
    {
        // TO DO: write your code here when License is OK
        // EXAMPLE
        Debug.Log("LICENSE");
        // TODO
        //HideLogin();
        //SetLoggedInMenu();
        //SetUserAccount(CurrentAccount.name);
        //ShowUser();
        // Start your game
        // - Got to other scene
        // - Deactivate Canvas
    }

    public void NO_License()
    {
        // TO DO: write your code here when License is not OK
        Debug.Log("NO LICENSE");
       /* if (OnNoLicense != null) OnNoLicense();
        HideLogin();
        SetUserAccount("No License");
        ShowUser();*/
    }

#endregion
}

