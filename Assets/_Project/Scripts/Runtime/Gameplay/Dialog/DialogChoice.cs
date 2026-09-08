using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogChoice
{
    public string choiceCode;
    public string text;
    [DialogNodeId] public string nextNodeId;
    public DialogCondition condition = new DialogCondition();

    public bool IsAvailable(IReadOnlyList<string> history)
    {
        return condition.Evaluate(history);
    }
}
