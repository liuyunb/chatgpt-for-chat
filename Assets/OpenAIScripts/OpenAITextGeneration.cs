using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAITextGeneration : MonoBehaviour
{
    string apiKey = "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";
    string modelID = "LY";


    private Coroutine a;
    private string m_ApiUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField]public List<SendData> m_DataList = new List<SendData>();
    [Serializable]public class PostData
    {
        public string model;
        public List<SendData> messages;
    }

    [Serializable]
    public class SendData
    {
        public string role;
        public string content;

        public SendData()
        {
        }

        public SendData(string _role, string _content)
        {
            role = _role;
            content = _content;
        }
    }
    
    [Serializable]
    public class MessageBack
    {
        public string id;
        public string created;
        public string model;
        public List<MessageBody> choices;
    }
    [Serializable]
    public class MessageBody
    {
        public Message message;
        public string finish_reason;
        public string index;
    }
    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    private void Start()
    {

    }
    
    private void Update()
    {
    }

    public void Submit(string content)
    {
        StartCoroutine(GenerateNewsTitle(content));
    }

    IEnumerator GenerateNewsTitle(string prompt = "河海大学怎么样")
    {

        // Set the number of tokens to generate
        int length = 20;
        
        m_DataList.Add(new SendData(ChatUI.Instance.userName, prompt));
        
        PostData _postData = new PostData
        {
            model = "gpt-3.5-turbo",
            messages = m_DataList
        };

        
        var www = new UnityWebRequest (m_ApiUrl, "POST");

        // Create the JSON request
        string jsonRequest = "{\"model\": \"" + modelID + "\", \"prompt\": \"" + prompt + "\", \"length\": " + length + "}";

        string _jsonText = JsonUtility.ToJson (_postData);
        byte[] data = System.Text.Encoding.UTF8.GetBytes (_jsonText);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw (data);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer ();
        
        
        // Create the web request
        // UnityWebRequest www = UnityWebRequest.Post("https://api.openai.com/v1/completions", _jsonText);

        // Set the headers for the web request
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the web request and wait for the response
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error generating text: " + www.error);
        }
        else
        {
            // Get the response text
            string response = www.downloadHandler.text;
            
            // Parse the response JSON
            JObject json = JObject.Parse(response);
            
            // Get the generated text
            string generatedText = json["choices"][0]["message"]["content"].ToString();
            
            // string _msg = www.downloadHandler.text;
            // TextCallback _textback = JsonUtility.FromJson<TextCallback> (_msg);
            // if (_textback!=null && _textback.choices.Count > 0) {
            //     Debug.Log(_textback.choices [0].text);
            // }
            Debug.Log("Finish");

            var jsData = json["choices"][0]["message"];
            m_DataList.Add(new SendData(jsData["role"].ToString(), jsData["content"].ToString()));
            
            var content = jsData["role"].ToString() + " : " + jsData["content"].ToString();
            ChatUI.Instance.CreateContent(content);
            //
            // // Print the generated text to the console
            // Debug.Log(prompt + "1" + generatedText.Trim());
        }
    }
}
