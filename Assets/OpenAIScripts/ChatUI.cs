using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    private static ChatUI _instance;

    public static ChatUI Instance => _instance;
    
    public string userName = "ly";

    public OpenAITextGeneration openAI;
    public Button submitButton;
    public TMP_InputField inputField;
    public Transform chatPanel;
    public GameObject chatPfb;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        submitButton.onClick.AddListener(() =>
        {
            string content = inputField.text;
            if (content != String.Empty)
            {
                content = userName + " : " + content;
                CreateContent(content);
                openAI.Submit(content);
            }
        });
    }

    public void CreateContent(string content)
    {
        var chat = Instantiate(chatPfb, chatPanel).GetComponent<AdvancedText>();
        StartCoroutine(chat.SetText(content, PrintType.Default, () =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatPanel);
            Debug.Log("PrintFinish");
        }));
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatPanel);
    }
}
