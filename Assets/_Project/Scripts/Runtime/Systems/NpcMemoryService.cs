using System.Collections.Generic;
using UnityEngine;

public class NpcMemoryService : MonoBehaviour
{
    private static NpcMemoryService _instance;

    public static NpcMemoryService Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("NpcMemoryService instance is null. Make sure there is a NpcMemoryService in the scene.");
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private readonly Dictionary<string, List<string>> _historyByNpcId = new Dictionary<string, List<string>>();
    private readonly Dictionary<string, HashSet<int>> _visitedOrdersByNpcId = new Dictionary<string, HashSet<int>>();

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

    public IReadOnlyList<string> GetHistory(string npcId)
    {
        return GetOrCreateHistory(npcId);
    }

    public void RecordChoice(string npcId, string choiceCode)
    {
        GetOrCreateHistory(npcId).Add(choiceCode);
    }

    public void MarkOrderVisited(string npcId, int order)
    {
        GetOrCreateVisitedOrders(npcId).Add(order);
    }

    public bool HasVisitedOrder(string npcId, int order)
    {
        return GetOrCreateVisitedOrders(npcId).Contains(order);
    }

    private List<string> GetOrCreateHistory(string npcId)
    {
        if (!_historyByNpcId.TryGetValue(npcId, out List<string> history))
        {
            history = new List<string>();
            _historyByNpcId[npcId] = history;
        }
        return history;
    }

    private HashSet<int> GetOrCreateVisitedOrders(string npcId)
    {
        if (!_visitedOrdersByNpcId.TryGetValue(npcId, out HashSet<int> orders))
        {
            orders = new HashSet<int>();
            _visitedOrdersByNpcId[npcId] = orders;
        }
        return orders;
    }
}
