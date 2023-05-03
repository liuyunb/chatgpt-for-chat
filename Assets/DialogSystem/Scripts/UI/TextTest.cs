using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTest : MonoBehaviour
{
    public Sequence_SO startSequence;
    public DialogUI dialog;
    public AdvancedText textCom;
    public Widget widget;
    [TextArea]
    public string content;

    private void Start()
    {
        // textCom.StartTyping(content);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var executor = Instantiate(startSequence);
            // dialog.OpenDialog();
            executor.Init((_)=>Debug.Log("Finished"));
            executor.Execute();
        }
    }
}
