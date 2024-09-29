using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLoader : MonoBehaviour
{
    #region Private variables

    // Image from its GameObject
    private Image image;
    // Button from its gameObject
    private Button button;
    // Name of the character (it is a child)
    private TMP_Text characterName;
    // Canvas group (to show it transparent)
    private CanvasGroup canvasGroup;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        // We obtain all components
        Init();
    }

    #endregion


    #region Public Methods

    public void Init()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        characterName = GetComponentInChildren<TMP_Text>();

        SetClick();
    }

    public void ChangeData(Sprite sprite, string name)
    {
        if (image == null)
            Init();

        image.sprite = sprite;
        characterName.text = name;
        gameObject.name = name;
    }

    public void SetTransparency(bool show)
    {
        if (button == null)
            Init();

        button.interactable = show;
        canvasGroup.alpha = show ? 1f : 100f / 255f;
    }

    #endregion

    #region Private methods

    private void SetClick()
    {
        button.onClick.AddListener(() => { Debug.Log("I pressed " + characterName.text); });
    }

    #endregion

}
