using System.Collections.Generic;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "ScriptableDataList", menuName = "Scriptables/List")]
    public class ScriptableDataList : ScriptableObject
    {
        public List<ScriptableData> elements;

        public ScriptableData GetScriptableElement(string label)
        {
            ScriptableData data = null;

            for (int i = 0; i < elements.Count && data == null; i++)
            {
                if (elements[i].label.Equals(label))
                    data = elements[i];
            }


            return data;
        }

        public void AddElement(string label, string idata, Sprite sprite, bool free)
        {
            ScriptableData data = new ScriptableData(label, idata, sprite, free);
            elements.Add(data);
        }

        public void RemoveElement(string label)
        {
            elements.Remove(GetScriptableElement(label));
        }
    }

}
