using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public abstract class IntroRunner : MonoBehaviour {

    protected Queue<Panel> _panels = null;
    public Panel currentPanel;
    public float panelTimeLeft;

    public GameObject introspectionContainer;
    public Text introspectionText;

    [Serializable]
    public class Panel
    {
        public float duration;
        public string text;
        public Action doOnce;
        public Action doEveryFrame;
    }

    public abstract void InitPanels();

	// Use this for initialization
	void Start () {
        _panels = new Queue<Panel>();
        InitPanels();
        OnNextPanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPanel == null)
        {
            OnNextPanel();
        }

        if (currentPanel != null)
        {
            float deltaTime = Time.unscaledDeltaTime;
            if (panelTimeLeft < deltaTime)
            {
                OnNextPanel();
            }
            else
            {
                panelTimeLeft -= deltaTime;
            }
        }
        
        if (currentPanel != null)
        {
            if (currentPanel.doEveryFrame != null)
            {
                currentPanel.doEveryFrame();
            }
        }
    }

    private void OnNextPanel()
    {
        if (_panels != null && _panels.Count > 0)
        {
            OnStartPanel(_panels.Dequeue());
        }
        else
        {
            OnStartPanel(null);
        }
    }

    private void OnStartPanel(Panel panel)
    {
        currentPanel = panel;

        bool showText = false;
        string text = "";
        
        if (panel != null)
        {
            panelTimeLeft = currentPanel.duration;

            if (panel.doOnce != null)
            {
                panel.doOnce();
            }

            if (!string.IsNullOrEmpty(panel.text))
            {
                text = panel.text;
                showText = true;
            }
        }
        else
        {
            panelTimeLeft = 0;
        }
        
        introspectionContainer.gameObject.SetActive(showText);
        introspectionText.gameObject.SetActive(showText);
        introspectionText.text = text;
    }
}
