// Assets/Scripts/Managers/GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelManager levelManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[EventManager] Instance created and set.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("[EventManager] Duplicate instance detected and destroyed.");
        }
}

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath.AddListener(HandleEnemyDeath);
            Debug.Log("[GameManager] Subscribed to OnEnemyDeath event.");
        }
        else
        {
            Debug.LogError("[GameManager] EventManager Instance is missing in the scene.");
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath.RemoveListener(HandleEnemyDeath);
            Debug.Log("[GameManager] Unsubscribed from OnEnemyDeath event.");
        }
    }

    private void HandleEnemyDeath(EnemyData enemyData)
    {
        if (levelManager != null && enemyData != null)
        {
            Debug.Log($"[GameManager] Enemy killed: {enemyData.enemyName}. Awarding {enemyData.experienceReward} XP.");
            levelManager.AddExperience(enemyData.experienceReward);
        }
        else if (levelManager == null)
        {
            Debug.LogError("[GameManager] LevelManager is not assigned.");
        }
        else if (enemyData == null)
        {
            Debug.LogError("[GameManager] Received null EnemyData in HandleEnemyDeath.");
        }
    }
}
