using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    public DialogUI dialog;
    public GameObject optionPanel;
    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenOptionPanel(List<ChoiceData> data, Action<int> onClick)
    {
        optionPanel.GetComponent<Widget>().Fade(1, 0.2f, null);
        AddOptionBtn(data, onClick);
    }
    
    public void AddOptionBtn(List<ChoiceData> data, Action<int> onClick)
    {
        for (int i = 0; i < data.Count; i++)
        {
            var prefab = Resources.Load<AdvancedButtonA>("OptionBtn");
            var btn = Instantiate(prefab, optionPanel.transform);
            btn.Init(data[i].content, i, onClick);
        }
    }

    public void CloseOptionPanel()
    {
        foreach (Transform item in optionPanel.transform)
        {
            Destroy(item.gameObject);
        }
        optionPanel.GetComponent<Widget>().Fade(0, 0.2f, null);
    }
}