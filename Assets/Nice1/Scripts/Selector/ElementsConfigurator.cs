using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Scriptables
{
    public class ElementsConfigurator : MonoBehaviour
    {
        #region DLL Imports

        [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CheckLicense(string owner, string author, string category, string license_name, string idata_name);

        [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CheckNice1GenesisKey(string owner, string author, string category, string license_name, string idata_name, int checkNice1GenesisKey);

        [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr CheckLicensePlugin(string owner, string author, string category, string license_name, int checkNice1GenesisKey, int network);

        #endregion

        #region Public variables

        [Header("Data & elements")]
        [Tooltip("Scriptable object list where the elements will be stored")]
        public ScriptableDataList scriptableDataList;
        [Tooltip("List of items to be created or edited")]
        public List<ScriptableData> elementsList;
        [Tooltip("WalletManager to get some information")]
        public WalletManager walletManager;

        [Header("Settings")]
        [Tooltip("Display mode")]
        public DisplayMode displayMode;

        [Tooltip("Owner name (for testing in editor)")]
        public string testOwner;

        [Tooltip("The \"parent\" of each element in the UI")]
        public GameObject target;
        [Tooltip("Prefab of the element which will be shown")]
        public GameObject prefab;

        #endregion

        #region Enum

        public enum DisplayMode
        {
            Show, // If you do not have the license, it will not be shown
            Transparent // If you do not the license, it will be shown transparently
        }

        #endregion


        #region Private variables

        // Indicates if we are running or not the scene
        private bool isRunning = false;

        #endregion

        #region Unity methods

        private void Awake()
        {
            // We are now running the scene
            isRunning = true;
        }

        #endregion


        #region Public methods

        #region Elements data

        public void LoadDataFromScriptableObject()
        {
            if (scriptableDataList == null)
                return;

            elementsList.Clear();

            foreach (ScriptableData data in scriptableDataList.elements)
                elementsList.Add(data);

        }

        public void SaveDataIntoScriptableObject()
        {
            if (scriptableDataList == null)
                return;
            // We force save data
            EditorUtility.SetDirty(scriptableDataList);
            scriptableDataList.elements.Clear();

            foreach (ScriptableData data in elementsList)
                scriptableDataList.elements.Add(data);
        }

        #endregion

        #region Elements displaying

        public void CreateElementsInUI()
        {
            // First, we destroy all elements contained by the UI
            for (int i = target.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(target.transform.GetChild(i).gameObject);


            // Second, we get each element of the list
            foreach (ScriptableData data in elementsList)
            {
                // We put it in the UI
                GameObject obj = Instantiate(prefab, target.transform);
                CharacterLoader load = obj.AddComponent<CharacterLoader>();
                load.Init();
                load.ChangeData(data.sprite, data.label);
            }
        }

        #endregion

        private void DisplayElement(GameObject obj, bool show)
        {
            CharacterLoader load = obj.GetComponent<CharacterLoader>();

            switch (displayMode)
            {
                case DisplayMode.Show:
                    obj.SetActive(show);
                    load.SetTransparency(true);
                    break;
                case DisplayMode.Transparent:
                    obj.SetActive(true);
                    load.SetTransparency(show);
                    break;
            }
        }

        #region Checking Licenses

        private void CheckLicense(ScriptableData data, GameObject obj)
        {
            DisplayElement(obj, data.freeElement || CheckElementLicense(data));
        }

        public void CheckAllLicenses()
        {
            for (int i = 0; i < target.transform.childCount; i++)
            {
                ScriptableData data = elementsList[i];

                GameObject obj = target.transform.GetChild(i).gameObject;

                CheckLicense(data, obj);
            }
        }

        private bool CheckElementLicense(ScriptableData data)
        {
            if (data.nice1Key && walletManager.hasNice1Key)
                return true;

            string owner;

            if (!isRunning)
                owner = testOwner;
            else
                owner = walletManager.owner;

            int genesisKey = 0;
            string license = Marshal.PtrToStringAnsi(CheckLicensePlugin(owner, walletManager.AUTHOR, walletManager.CATEGORY, data.idata, genesisKey, (int)walletManager.network));


            return license.Equals("LICENSE");
        }

        #endregion


        #endregion

    }
}
