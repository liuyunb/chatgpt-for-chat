using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ChoiceData
{
    [TextArea]
    public string content;
}
[CreateAssetMenu(fileName = "OptionNode", menuName = "Dialog/ShowChoice")]
public class EN_ShowChoice : EventNodeBase_SO
{
    public List<ChoiceData> dataList = new List<ChoiceData>();
    public List<Sequence_SO> sequenceList = new List<Sequence_SO>();

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Execute()
    {
        base.Execute();
        UIManager.Instance.OpenOptionPanel(dataList, OnOptionClick);
        
    }

    public void OnOptionClick(int index)
    {
        if (index < sequenceList.Count)
        {
            sequenceList[index].Init(OnFinished);
            sequenceList[index].Execute();
        }
        else
        {
            OnFinished?.Invoke(true);
        }
        UIManager.Instance.CloseOptionPanel();
    }
}
