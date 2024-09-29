using Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectorPanel : UIPanel
{
    public ElementsConfigurator configurator;
    public UIPanel UserPanel;
    public Text textUserAccount;

    private void Awake()
    {
        _isShowing = true;
        // At the beginning, we hide the panel.
        HidePanel();
    }

    public override void ShowPanel()
    {
        if (textUserAccount.text.Equals("No License"))
        {
            UserPanel.ShowPanel();
            this.HidePanel();
        }
        else if (!_isShowing)
        {
            _isShowing = true;
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            configurator.CheckAllLicenses();
        }
    }

    public override void HidePanel()
    {
        if (_isShowing)
        {
            _isShowing = false;
            transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
        }
    }
}
