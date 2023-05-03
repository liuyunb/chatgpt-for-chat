using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Executor", menuName = "Dialog/Sequence")]
public class Sequence_SO : ScriptableObject
{
    public List<EventNodeBase_SO> nodeList = new List<EventNodeBase_SO>();
    private int _curIndex = 0;
    public Action<bool> OnFinished;

    public void Init(Action<bool> onFinished)
    {
        OnFinished += onFinished;
        _curIndex = 0;
        foreach (var node in nodeList)
        {
            node.curState = NodeState.Waiting;
        }
    }
    
    public void Execute()
    {
        ExecuteNext();
    }

    public void ExecuteNext()
    {
        if (_curIndex < nodeList.Count)
        {
            nodeList[_curIndex].Init(OnNodeFinished);
            if (nodeList[_curIndex].curState == NodeState.Waiting)
            {
                nodeList[_curIndex].Execute();
            }
            _curIndex++;
        }
        else
        {
            OnFinished?.Invoke(true);
        }
    }

    public void OnNodeFinished(bool success)
    {
        if (success)
        {
            ExecuteNext();
        }
        else
        {
            OnFinished?.Invoke(false);
        }
    }

}