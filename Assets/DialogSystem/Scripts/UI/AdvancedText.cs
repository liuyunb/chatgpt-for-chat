using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class RubyData
{
    public int StartIndex { get; }
    public int EndIndex { get; set; }
    public string Content { get; }

    public RubyData(int startIndex, string content)
    {
        StartIndex = startIndex;
        Content = content;
        EndIndex = startIndex;
    }
}

public class AdvancedTextPreprocessor : ITextPreprocessor
{
    //index, delayTime
    public Dictionary<int, float> IntervalDict = new Dictionary<int, float>();
    public List<RubyData> RubyList = new List<RubyData>();

    public bool TryGetRubyData(int index, out RubyData data)
    {
        data = null;
        data = RubyList.Find(r => r.StartIndex == index);
        return data != null;
    }

    public string PreprocessText(string text)
    {
        IntervalDict.Clear();
        RubyList.Clear();

        var preText = text;
        var pattern = "<.*?>";
        Match match = Regex.Match(preText, pattern);
        while (match.Success)
        {
            var label = match.Value.Substring(1, match.Length - 2);
            if (float.TryParse(label, out float result))
            {
                IntervalDict[match.Index - 1] = result;
            }
            else if (Regex.IsMatch(label, "^r=.+"))
            {
                RubyList.Add(new RubyData(match.Index, label.Substring(2)));
            }
            else if (label == "/r")
            {
                RubyList[^1].EndIndex = match.Index - 1;
            }

            preText = preText.Remove(match.Index, match.Length);

            if (Regex.IsMatch(label, "^sprite=.+"))
            {
                preText.Insert(match.Index, " ");
            }

            match = Regex.Match(preText, pattern);
        }


        pattern = @"(<(\d+)(\.\d+)?>)|(<r=.+?>)|(</r>)";
        preText = Regex.Replace(text, pattern, "");
        return preText;
    }
}
[RequireComponent(typeof(Widget))]
public class AdvancedText : TextMeshProUGUI
{
    private int _curIndex;

    public float defaultTime = 0.2f;

    private Widget _widget => GetComponent<Widget>();

    private Coroutine _typing;

    private Action _onFinished;

    AdvancedText()
    {
        textPreprocessor = new AdvancedTextPreprocessor();
    }

    private AdvancedTextPreprocessor SelfPreprocessor => (AdvancedTextPreprocessor) textPreprocessor;

    public void StartTyping(string content)
    {
        SetText(content);
        StartCoroutine(Typing());
    }

    public void Disappear(float duration = 0.2f)
    {
        _widget.Fade(0, duration, null);
        foreach (var item in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if(item != this)
                Destroy(item.gameObject);
        }
    }

    public void QuickShowRemain()
    {
        if (_typing != null)
        {
            StopCoroutine(_typing);
        }
        for (; _curIndex < m_characterCount; _curIndex++)
        {
            StartCoroutine(FadeInCharacter(_curIndex));
        }
        _onFinished?.Invoke();
    }

    public IEnumerator SetText(string content, PrintType printType, Action onFinished)
    {
        Disappear();
        yield return new WaitForSecondsRealtime(0.3f);
        
        SetText(content);
        yield return null;

        switch (printType)
        {
            case PrintType.Default:
                _widget.Fade(1, 0, null);
                SetAllRubyText();
                onFinished?.Invoke();
                break;
            case PrintType.Fade:
                _widget.Fade(1, 0.2f, null);
                SetAllRubyText();
                onFinished?.Invoke();
                break;
            case PrintType.Type:
                _widget.Fade(1, 0, null);
                _typing = StartCoroutine(Typing());
                _onFinished = onFinished;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(printType), printType, null);
        }
    }

    IEnumerator Typing()
    {
        ForceMeshUpdate();

        for (int i = 0; i < m_characterCount; i++)
        {
            SetSingleCharacter(i, 0);
        }

        // yield return new WaitForSecondsRealtime(defaultTime);

        _curIndex = 0;

        while (_curIndex < m_characterCount)
        {
            StartCoroutine(FadeInCharacter(_curIndex));
            _curIndex++;
            if (SelfPreprocessor.IntervalDict.TryGetValue(_curIndex, out float value))
            {
                yield return new WaitForSecondsRealtime(value);
            }
            else
            {
                yield return new WaitForSecondsRealtime(defaultTime);
            }
            yield return null;
        }
        
        _onFinished?.Invoke();
    }

    //alpha 0-255;
    public void SetSingleCharacter(int index, byte alpha)
    {
        var charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible)
            return;
        var matIndex = charInfo.materialReferenceIndex;
        var vertexIndex = charInfo.vertexIndex;
        for (int i = 0; i < 4; i++)
        {
            textInfo.meshInfo[matIndex].colors32[vertexIndex + i].a = alpha;
        }

        UpdateVertexData();
    }

    public void SetRubyText(RubyData data)
    {
        var prefab = Resources.Load<GameObject>("RubyText");
        var ruby = Instantiate(prefab, transform);
        var rubyText = ruby.GetComponent<TextMeshProUGUI>();
        rubyText.text = data.Content;
        var tColor = textInfo.characterInfo[data.StartIndex].color;
        tColor.a = 255;
        rubyText.color = textInfo.characterInfo[data.StartIndex].color;
        rubyText.rectTransform.localPosition = (textInfo.characterInfo[data.StartIndex].topLeft +
                                                textInfo.characterInfo[data.EndIndex].topRight) / 2;
    }

    public void SetAllRubyText()
    {
        foreach (var item in SelfPreprocessor.RubyList)
        {
            SetRubyText(item);
        }
    }

    IEnumerator FadeInCharacter(int index, float defaultDuration = 0.2f)
    {
        if (SelfPreprocessor.TryGetRubyData(index, out RubyData data))
        {
            SetRubyText(data);
        }
        //如果过度时间小于0则立即显示
        if (defaultDuration < 0)
        {
            SetSingleCharacter(index, 255);
            yield break;
        }
        
        float time = 0f;

        while (time < defaultDuration)
        {
            time = Mathf.Min(defaultDuration, time + Time.unscaledDeltaTime);
            SetSingleCharacter(index, (byte) (255 * time / defaultDuration));
            yield return null;
        }
    }
}