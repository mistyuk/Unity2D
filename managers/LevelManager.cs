// Assets/Scripts/Managers/LevelManager.cs
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public PlayerStats playerStats; // Assign via Inspector
    public LevelProgression levelProgression;

    [Header("Level Info")]
    public int currentLevel = 1;
    public int currentExperience = 0;

    public UnityEvent OnLevelUp = new UnityEvent();

    private int currentExperienceRequirement;

    private void Start()
    {
        if (levelProgression != null)
        {
            UpdateExperienceRequirement();
        }
        else
        {
            Debug.LogError("LevelProgression is not assigned in LevelManager.");
        }
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
         Debug.Log($"[LevelManager] Gained {amount} XP. Total XP: {currentExperience}");

        while (currentExperience >= currentExperienceRequirement && currentLevel < levelProgression.maxLevel)
        {
            currentExperience -= currentExperienceRequirement;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        UpdateExperienceRequirement();
        ApplyStatIncreases();

        Debug.Log($"Leveled up to Level {currentLevel}!");

        OnLevelUp?.Invoke();
    }

    private void UpdateExperienceRequirement()
    {
        if (currentLevel - 1 < levelProgression.experienceRequirements.Length)
        {
            currentExperienceRequirement = levelProgression.experienceRequirements[currentLevel - 1];
            Debug.Log($"XP required for next level: {currentExperienceRequirement}");
        }
        else
        {
            currentExperienceRequirement = int.MaxValue;
            Debug.Log("Max level reached.");
        }
    }

    private void ApplyStatIncreases()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned in LevelManager.");
            return;
        }

        playerStats.currentHealth = playerStats.baseHealth + playerStats.healthPerLevel * (currentLevel - 1);
        playerStats.currentDefense = playerStats.baseDefense + playerStats.defensePerLevel * (currentLevel - 1);
        playerStats.currentAttackPower = playerStats.baseAttackPower + playerStats.attackPowerPerLevel * (currentLevel - 1);
        playerStats.currentCriticalChance = playerStats.baseCriticalChance + playerStats.criticalChancePerLevel * (currentLevel - 1);
    }

    public int GetExperienceForNextLevel()
    {
        return currentExperienceRequirement - currentExperience;
    }

    public float GetExperienceProgress()
    {
        return (float)currentExperience / currentExperienceRequirement;
    }

    public int GetCurrentExperienceRequirement()
    {
        return currentExperienceRequirement;
    }
}
