using UnityEditor;
using UnityEngine;


namespace Scriptables
{
    [CustomEditor(typeof(ElementsConfigurator))]
    public class ConfiguratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ElementsConfigurator container = (ElementsConfigurator)target;

            GUILayout.Space(20f);

            if (GUILayout.Button("Load data from scriptable data list"))
                container.LoadDataFromScriptableObject();

            GUILayout.Space(2f);
            if (GUILayout.Button("Save data into scriptable data list"))
                container.SaveDataIntoScriptableObject();
            

            GUILayout.Space(2f);
            if (GUILayout.Button("Create elements in the UI"))
                container.CreateElementsInUI();

            GUILayout.Space(2f);
            if (GUILayout.Button("Refresh licenses"))
                container.CheckAllLicenses();


        }
    }

}
