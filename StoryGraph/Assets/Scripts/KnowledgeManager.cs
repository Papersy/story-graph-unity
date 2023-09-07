using UnityEngine;

public class KnowledgeManager : MonoBehaviour
{
    public KnowledgeManager Instance;

    public void Awake()
    {
        Instance = this;
    }
}