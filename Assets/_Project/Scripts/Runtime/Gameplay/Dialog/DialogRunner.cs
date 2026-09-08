using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogRunner : MonoBehaviour
{
    private static DialogRunner _instance;

    public static DialogRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("DialogRunner instance is null. Make sure there is a DialogRunner in the scene.");
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public event Action<DialogNode, List<DialogChoice>> OnNodeChanged;
    public event Action OnDialogEnded;

    private static readonly DialogNode RepeatNode = new DialogNode
    {
        id = "__repeat__",
        text = "...",
        choices = new List<DialogChoice>
        {
            new DialogChoice { choiceCode = "__repeat__", text = "...", nextNodeId = "" }
        }
    };

    private DialogContainer _container;
    private string _npcId;
    private bool _skipHistory;

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartDialog(string npcId, DialogContainer container, bool alreadyVisited = false)
    {
        _npcId = npcId;
        _container = container;
        _skipHistory = alreadyVisited;

        GameManager.Instance.SetState(GameState.Dialog);

        if (alreadyVisited)
        {
            GoToNode(RepeatNode);
            return;
        }

        IReadOnlyList<string> history = NpcMemoryService.Instance.GetHistory(npcId);
        string entryNodeId = container.ResolveEntryNodeId(history);
        DialogNode entryNode = container.GetNodeById(entryNodeId);

        if (entryNode == null)
        {
            Debug.LogError($"DialogRunner: no entry node resolved for NPC '{npcId}'. Check the DialogContainer's entry points.");
            return;
        }

        GoToNode(entryNode);
    }

    public void SelectChoice(DialogChoice choice)
    {
        if (!_skipHistory)
        {
            NpcMemoryService.Instance.RecordChoice(_npcId, choice.choiceCode);
        }

        if (string.IsNullOrEmpty(choice.nextNodeId))
        {
            EndDialog();
            return;
        }

        DialogNode nextNode = _container.GetNodeById(choice.nextNodeId);
        if (nextNode == null)
        {
            Debug.LogError($"DialogRunner: next node '{choice.nextNodeId}' not found in container '{_container.name}'.");
            EndDialog();
            return;
        }

        GoToNode(nextNode);
    }

    private void GoToNode(DialogNode node)
    {
        IReadOnlyList<string> history = NpcMemoryService.Instance.GetHistory(_npcId);
        List<DialogChoice> availableChoices = new List<DialogChoice>();
        foreach (DialogChoice choice in node.choices)
        {
            if (choice.IsAvailable(history))
            {
                availableChoices.Add(choice);
            }
        }

        OnNodeChanged?.Invoke(node, availableChoices);
    }

    private void EndDialog()
    {
        _container = null;
        _npcId = null;
        GameManager.Instance.SetState(GameState.FreeExploration);
        OnDialogEnded?.Invoke();
    }
}
