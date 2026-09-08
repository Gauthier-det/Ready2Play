using System;
using UnityEngine;

[Serializable]
public class DialogEntryPoint
{
    public DialogCondition condition = new DialogCondition();
    [DialogNodeId] public string targetNodeId;
}
