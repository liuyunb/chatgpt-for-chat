using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogData
{
    public string speaker;
    [TextArea]
    public string content;
    public bool needType;
    public bool canQuickShow;
    public bool autoNext;
}

[CreateAssetMenu(fileName = "DialogNode", menuName = "Dialog/ShowDialog")]
public class EN_ShowDialog : EventNodeBase_SO
{
    public List<DialogData> dataList = new List<DialogData>();
    public override void Init(Action<bool> onFinished)
    {
        base.Init(onFinished);
    }

    public override void Execute()
    {
        base.Execute();
        UIManager.Instance.dialog.OpenDialog(dataList, OnFinished);
    }
    
}