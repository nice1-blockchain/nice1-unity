using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using static System.Net.WebRequestMethods;

public class WalletManager : Singleton<WalletManager>
{
    #region DLL Imports

    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CheckLicense(string owner, string author, string category, string license_name, string idata_name);

    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CheckNice1GenesisKey(string owner, string author, string category, string license_name, string idata_name, int checkNice1GenesisKey);

    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr CheckLicensePlugin(string owner, string author, string category, string license_name, int checkNice1GenesisKey, int network);

    #endregion

    #region UI Objects

    [Header("Login")]
    public UIPanel loginPanel;
    public GameObject loginPanelGO;
    public GameObject loginButton;
    public GameObject logoutButton;
    public UIPanel userPanel;
    public Text textUserAccount;

    #endregion

    #region License Parameters

    public enum Network
    {
        Jungle4_Testnet, Eos_Mainnet, Proton_Testnet, Proton_Mainnet, Wax_Testnet, Wax_Mainnet, Telos_Testnet, Telos_Mainnet
    }

    [Header("Network")]
    public Network network;
    // Proton testnet: https://proton-testnet.eosio.online/endpoints
    private List<string> networkEndpoints = new List<string>
    {
        "https://jungle4.greymass.com/v1/",
        "https://eos.greymass.com/v1/",
        "https://protontestnet.greymass.com/v1/",
        "https://proton.cryptolions.io/v1/",
        "https://testnet.waxsweden.org/v1/",
        "https://wax.greymass.com/v1/",
        "https://test.telos.eosusa.io/v1/",
        "https://telos.greymass.com/v1/" 
    };

    [Header("License - Mandatory fields")]
    public string AUTHOR = "niceonedemos";
    public string IDATA_NAME = "GAME LICENSE - LegendaryLegends";
    public string CATEGORY = "llegends";

    [Header("Nice1 Genesis Key")]
    public bool checkNice1GenesisKey_bool;

    [Header("FreeToPlay")]
    public bool freeLicense_bool;

    [Header("License - Error message")]
    public string errorLicenseText = "You are not licensed to use this game";

    [Header("Vuplex - Error message")]
    public string errorVuplexText = "It is necessary to have the Vuplex plugin installed. For more information go to https://docs.nice1.dev/";

    [HideInInspector]
    public int checkNice1GenesisKey;

    [HideInInspector]
    public bool hasNice1Key { get; private set; }

    [HideInInspector]
    public string owner { get; private set; }

    #endregion

    public WalletAccount CurrentAccount { get; private set; }

    private bool simpleAssets;
    private WebRequestResults lastRequest;
    private WebRequestResultsDeltas lastRequestDeltas;

    public delegate void WalletDelegate();
    public static event WalletDelegate OnNoLicense;

    private void Awake()
    {
        hasNice1Key = false;
        owner = "";

        CurrentAccount = new WalletAccount();
        checkNice1GenesisKey = checkNice1GenesisKey_bool ? 1 : 0;
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
        string licenseResult = Marshal.PtrToStringAnsi(CheckLicensePlugin(owner, AUTHOR, CATEGORY, IDATA_NAME, checkNice1GenesisKey, (int)network));

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
        HideLogin();
        SetLoggedInMenu();
        SetUserAccount(CurrentAccount.name);
        ShowUser();
        // Start your game
        // - Got to other scene
        // - Deactivate Canvas
    }

    public void NO_License()
    {
        // TO DO: write your code here when License is not OK
        Debug.Log("NO LICENSE");
        if (OnNoLicense != null) OnNoLicense();
        HideLogin();
        SetUserAccount("No License");
        ShowUser();
    }

    #endregion

    #region UI

    public void SetLoggedInMenu()
    {
        HideLogin();
        if (loginButton != null) loginButton.SetActive(false);
    }

    private void SetNotLoggedInMenu()
    {
        if (loginButton != null) loginButton.SetActive(true);
        if (logoutButton != null) logoutButton.SetActive(false);
    }

    public async void LogOut()
    {
#if VUPLEX_STANDALONE
        //await GameObject.FindGameObjectWithTag("canvasWebView").GetComponent<Vuplex.WebView.CanvasWebViewPrefab>().LogOut2();
#endif
        SetNotLoggedInMenu();
    }

    public void ShowLogin()
    {
#if VUPLEX_STANDALONE
        if (loginPanel != null) ShowPanel(loginPanel);
#else
        Debug.Log(errorVuplexText);
#endif
    }

    public void HideLogin()
    {
        if (loginPanel != null) HidePanel(loginPanel);
    }

    public void ShowUser()
    {
        if (userPanel != null) ShowPanel(userPanel);
    }

    public void HideUser()
    {
        if (userPanel != null) HidePanel(userPanel);
    }

    private void ShowPanel(UIPanel panel)
    {
        if (panel != null) panel.ShowPanel();
    }

    private void HidePanel(UIPanel panel)
    {
        if (panel != null) panel.HidePanel();
    }

    public void SetUserAccount(string userAccount)
    {
        if (textUserAccount != null)
            textUserAccount.text = userAccount;
    }

    #endregion

    #region Errors
    public void ShowLoginError(string errorMessage)
    {
        string errorText = "There was a problem logging into your account. Make sure you are logged into Scatter."
                         + "\nError: " + errorMessage;


        Debug.LogError("There was a problem logging into your account. Make sure you are logged into Scatter.");
        Debug.LogError("Error: " + errorMessage);
    }

    public void ShowApiError(string errorMessage)
    {
        string errorText = "There was a problem communicating with the API. Please try again."
                         + "\nError: " + errorMessage;


        Debug.LogError("There was a problem communicating with the API. Please try again.");
        Debug.LogError("Error: " + errorMessage);
    }
    #endregion
}

#region Data Model 

[System.Serializable]
public class WalletAccount
{
    public void Initialize(string name, string authority, string publicKey, string blockChain, bool isHardware, string chainID)
    {
        this.name = name;
        this.authority = authority;
        this.publicKey = publicKey;
        this.blockChain = blockChain;
        this.isHardware = isHardware;
        this.chainID = chainID;
    }

    public string name;
    public string authority;
    public string publicKey;
    public string blockChain;
    public bool isHardware;
    public string chainID;
}

#region SimpleAssets Data Model

[System.Serializable]
public class WebRequestResults
{
    public List<WebResultContainer> results;
}

[System.Serializable]
public class WebResultContainer
{
    public string assetId;
    public string author;
    public string owner;
    public string category;
    public string control;
    public ImmutableData idata;
    public MutableData mdata;
}

[System.Serializable]
public class ImmutableData
{
    public string name;
    public string img;
}

[System.Serializable]
public class MutableData
{
    public string name;
}

#endregion

#region Deltas Data Model

[System.Serializable]
public class WebRequestResultsDeltas
{
    public List<WebResultContainerDeltas> deltas;
}

[System.Serializable]
public class WebResultContainerDeltas
{
    public DataContainerDeltas data;
}

[System.Serializable]
public class DataContainerDeltas
{
    public string author;
    public string category;
    public string owner;
    public string idata;
}

#endregion

#endregion