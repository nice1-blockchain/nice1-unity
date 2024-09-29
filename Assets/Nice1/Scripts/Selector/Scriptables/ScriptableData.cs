using System;
using UnityEngine;

namespace Scriptables
{
    [Serializable]
    public class ScriptableData
    {
        [Tooltip("Name of the element")]
        public string label;
        [Tooltip("Idata name")]
        public string idata;
        [Tooltip("Sprite of the element")]
        public Sprite sprite;
        [Tooltip("True if license is not necessary")]
        public bool freeElement;
        [Tooltip("Check previously if has nice1Key")]
        public bool nice1Key;

        public ScriptableData(string label, string idata, Sprite sprite, bool free)
        {
            this.label = label;
            this.idata = idata;
            this.sprite = sprite;
            this.freeElement = free;
        }


    }
}