using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Button
{
    protected override void Start()
    {
        base.Start();
        onClick.AddListener(ClickEvent);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
    }

    protected virtual void ClickEvent()
    {
        
    }
}
