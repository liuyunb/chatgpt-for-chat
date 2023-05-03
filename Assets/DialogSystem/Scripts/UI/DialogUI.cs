using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PrintType
{
    Default, Fade, Type
}
[RequireComponent(typeof(Widget))]
public class DialogUI : MonoBehaviour
{
    public AdvancedText dialogContent;
    public TextMeshProUGUI speakerName;
    public Animator cursorClick;
    private static readonly int ClickHash = Animator.StringToHash("Click");

    private Widget _widget;

    public List<DialogData> dataList = new List<DialogData>();
    private int _curIndex;

    private Action<bool> _onFinished;

    private bool _autoNext;
    private bool _needType;
    private bool _canQuickShow;
    private bool _isFinished;
    private bool _interactable;

    private bool _isOpen = false;
    //防止点击按钮时触发onfinish
    private bool _nodeEnd = false;

    public bool CanQuickShow => !_isFinished && _canQuickShow;
    public bool AutoNext => _autoNext && _isFinished;

    private void Awake()
    {
        _widget = GetComponent<Widget>();
    }

    private void Update()
    {
        if(_interactable) UpdateInput();
    }

    public void UpdateInput()
    {
        if (_isFinished && Input.GetMouseButtonDown(0))
        {
            cursorClick.SetTrigger(ClickHash);
        }

        if (!_nodeEnd && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (CanQuickShow)
            {
                dialogContent.QuickShowRemain();
            }
            else if (_isFinished)
            {
                PrintNext();
            }
        }
    }

    public void PrintNext()
    {
        if (_curIndex < dataList.Count)
        {
            cursorClick.GetComponent<Widget>().Fade(0, 0.2f, null);
            var data = dataList[_curIndex];
            
            _isFinished = false;
            _canQuickShow = data.canQuickShow;
            _needType = data.needType;
            _autoNext = data.autoNext;
            
            var printType = _needType ? PrintType.Type : PrintType.Fade;
            StartCoroutine(dialogContent.SetText(data.content, printType, OnFinishDialog));
            speakerName.text = data.speaker;
            _curIndex++;
        }
        else
        {
            _nodeEnd = true;
            _onFinished?.Invoke(true);
        }
    }

    public void OnFinishDialog()
    {
        _isFinished = true;
        if(AutoNext)
            PrintNext();
        else
        {
            cursorClick.GetComponent<Widget>().Fade(1, 0.2f, null);
        }
    }

    public void OpenDialog(List<DialogData> data, Action<bool> onFinished)
    {
        dataList = data;
        _curIndex = 0;
        _interactable = true;
        dialogContent.Disappear(0);
        _onFinished = onFinished;
        _nodeEnd = false;
        if(!_isOpen)
            _widget.Fade(1, 0.2f, PrintNext);
        else
        {
            PrintNext();
        }
    }

    public void CloseDialog()
    {
        _isOpen = false;
        _widget.Fade(0, 0.2f, null);
    }
}
