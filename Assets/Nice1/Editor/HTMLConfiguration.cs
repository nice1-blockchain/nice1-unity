using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WalletManager))]
public class HTMLConfiguration : Editor
{
    private string baseHtmlPath = Application.streamingAssetsPath + "/webview/base.html";
    private string indexHtmlPath = Application.streamingAssetsPath + "/webview/index.html";
    private char replaceSymbol = '&';

    private List<string> configurations = new List<string>
    {
		//https://support.greymass.com/support/solutions/articles/72000449566-list-of-blockchains-supported-by-anchor
        //EOS Testnet
		"chainId: '73e4385a2708e6d7048834fbc1079f2fabb17b3c125b146af438971e90716c4d'," +
        "nodeUrl: 'https://jungle4.greymass.com/'",
        //EOS Mainnet
        "chainId: 'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906'," +
        "nodeUrl: 'https://eos.greymass.com/'",
		// Telos Testnet
		"chainId: '1eaa0824707c8c16bd25145493bf062aecddfeb56c736f6ba6397f3195f33c9f'," +
        "nodeUrl: 'https://testnet.eos.miami/'",
		// Telos Mainnet
		"chainId: '4667b205c6838ef70ff7988f6e8257e8be0e1284a2f59699054a018f743b1d11'," +
        "nodeUrl: 'https://telos.greymass.com/'",
		// Proton Testnet
		"chainId: '71ee83bcf52142d61019d95f9cc5427ba6a0d7ff8accd9e2088ae2abeaf3d3dd'," +
        "nodeUrl: 'https://protontestnet.greymass.com/'",
		// Proton Mainnet
		"chainId: '384da888112027f0321850a169f737c33e53b388aad48b5adace4bab97f437e0'," +
        "nodeUrl: 'https://proton.greymass.com/'",
        // WAX Testnet
        "chainId: 'f16b1833c747c43682f4386fca9cbb327929334a762755ebec17f6f23c9b8a12'," +
        "nodeUrl: '	https://waxtestnet.greymass.com/'",
        // WAX Mainnet
        "chainId: '1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4'," +
        "nodeUrl: 'https://wax.greymass.com/'"
    };

    private int previousNetwork;

    private void Awake()
    {
        previousNetwork = -1;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int currentNetwork = (int)WalletManager.Instance.network;
        if (previousNetwork != -1 && previousNetwork != currentNetwork)
        {
            CreateHtml(currentNetwork);
        }
        previousNetwork = currentNetwork;
    }

    public void CreateHtml(int network)
    {
        if (File.Exists(baseHtmlPath))
        {
            string content = File.ReadAllText(baseHtmlPath);
            string configuration = configurations[network];

            int pos = content.IndexOf(replaceSymbol);
            if (pos != -1)
            {
                content = content.Replace(replaceSymbol.ToString(), configuration);

                if (!File.Exists(indexHtmlPath))
                {
                    File.Create(indexHtmlPath);
                }

                File.WriteAllText(indexHtmlPath, content);
            }
            else
            {
                Debug.Log("Create HTML failed. Base HTML wrong format.");
            }
        }
        else
        {
            Debug.Log("Create HTML failed. Base HTML file doesn't exist");
        }
    }
}
