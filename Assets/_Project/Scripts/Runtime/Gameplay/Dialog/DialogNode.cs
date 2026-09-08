using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogNode
{
    public string id;
    [TextArea] public string text;
    public List<DialogChoice> choices = new List<DialogChoice>();
}
