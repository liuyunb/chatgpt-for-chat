using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Waiting, Executing, Finished
}
public class EventNodeBase_SO : ScriptableObject
{
    protected Action<bool> OnFinished;
    public NodeState curState = NodeState.Waiting;

    protected virtual void Awake()
    {
        curState = NodeState.Waiting;
    }

    public virtual void Init(Action<bool> onFinished)
    {
        OnFinished += onFinished;
        OnFinished += OnFinishNode;
    }

    public virtual void Execute()
    {
        curState = NodeState.Executing;
    }

    public virtual void OnFinishNode(bool success)
    {
        curState = NodeState.Finished;
    }
}
