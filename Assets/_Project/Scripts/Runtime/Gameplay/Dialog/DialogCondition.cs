using System;
using System.Collections.Generic;
using System.Linq;

public enum DialogConditionType
{
    None,
    LastEquals,
    Contains
}

[Serializable]
public class DialogCondition
{
    public DialogConditionType type = DialogConditionType.None;
    [DialogChoiceCode] public string value;
    [DialogChoiceCode] public List<string> values = new List<string>();

    public bool Evaluate(IReadOnlyList<string> history)
    {
        switch (type)
        {
            case DialogConditionType.None:
                return true;
            case DialogConditionType.LastEquals:
                return history.Count > 0 && history[history.Count - 1] == value;
            case DialogConditionType.Contains:
                return values.All(v => history.Contains(v));
            default:
                return false;
        }
    }
}
