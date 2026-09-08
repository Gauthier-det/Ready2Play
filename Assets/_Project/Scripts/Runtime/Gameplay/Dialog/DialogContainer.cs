using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Ready2Play/Dialog Container")]
public class DialogContainer : ScriptableObject
{
    public List<DialogEntryPoint> entryPoints = new List<DialogEntryPoint>();
    public List<DialogNode> nodes = new List<DialogNode>();

    private Dictionary<string, DialogNode> _nodesById;

    private void OnEnable()
    {
        BuildLookup();
    }

    public string ResolveEntryNodeId(IReadOnlyList<string> history)
    {
        foreach (DialogEntryPoint entryPoint in entryPoints)
        {
            if (entryPoint.condition.Evaluate(history))
            {
                return entryPoint.targetNodeId;
            }
        }
        return null;
    }

    public DialogNode GetNodeById(string id)
    {
        if (_nodesById == null)
        {
            BuildLookup();
        }
        _nodesById.TryGetValue(id, out DialogNode node);
        return node;
    }

    private void BuildLookup()
    {
        _nodesById = new Dictionary<string, DialogNode>();
        foreach (DialogNode node in nodes)
        {
            _nodesById[node.id] = node;
        }
    }
}
