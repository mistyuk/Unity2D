// Assets/Scripts/Managers/EventManager.cs
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[EventManager] Initialized and set as singleton.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Define UnityEvents for various game events
    public UnityEvent<EnemyData> OnEnemyDeath = new UnityEvent<EnemyData>();
    public UnityEvent OnPlayerLevelUp = new UnityEvent();

    public void TriggerEnemyDeath(EnemyData enemyData)
    {
        if (OnEnemyDeath != null)
        {
            Debug.Log($"[EventManager] Triggering OnEnemyDeath event for {enemyData.enemyName} with {enemyData.experienceReward} XP reward.");
            OnEnemyDeath.Invoke(enemyData);
        }
    }
}
