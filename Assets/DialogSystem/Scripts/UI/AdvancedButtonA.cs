
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Widget))]
public class AdvancedButtonA : AdvancedButton
{
    private Transform _highLight;
    private Animator _anim;
    private TextMeshProUGUI _content;
    public int index;

    private static readonly int ClickHash = Animator.StringToHash("Click");

    protected override void Awake()
    {
        base.Awake();
        _highLight = transform.Find("HighLight/Glow");
        _anim = GetComponent<Animator>();
        _content = transform.Find("Content").GetComponent<TextMeshProUGUI>();
    }

    protected override void Start()
    {
        base.Start();

    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _highLight.gameObject.SetActive(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _highLight.gameObject.SetActive(false);
    }

    protected override void ClickEvent()
    {
        _anim.SetTrigger(ClickHash);
    }

    public void SetText(string content)
    {
        _content.text = content;
    }

    public void Init(string content, int i, Action<int> callBack)
    {
        index = i;
        SetText(content);
        onClick.AddListener(() =>
        {
            callBack?.Invoke(index);
        });
    }
}