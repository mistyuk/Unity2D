using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    // Singleton Instance for easy access
    public static TagManager Instance { get; private set; }

    // Dictionary to store GameObjects and their tags
    private Dictionary<GameObject, HashSet<string>> tagDictionary = new Dictionary<GameObject, HashSet<string>>();

    void Awake()
    {
        // Set up the Singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the TagManager across different scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to register multiple tags for a specific GameObject
    public void RegisterTags(GameObject obj, List<string> tags)
    {
        if (!tagDictionary.ContainsKey(obj))
        {
            tagDictionary[obj] = new HashSet<string>();
        }

        foreach (string tag in tags)
        {
            tagDictionary[obj].Add(tag);
        }
    }

    // Method to add a single tag to a GameObject
    public void AddTag(GameObject obj, string tag)
    {
        if (!tagDictionary.ContainsKey(obj))
        {
            tagDictionary[obj] = new HashSet<string>();
        }
        tagDictionary[obj].Add(tag);
    }

    // Method to remove a tag from a GameObject
    public void RemoveTag(GameObject obj, string tag)
    {
        if (tagDictionary.ContainsKey(obj))
        {
            tagDictionary[obj].Remove(tag);
            if (tagDictionary[obj].Count == 0)
            {
                tagDictionary.Remove(obj);
            }
        }
    }

    // Method to check if a GameObject has a specific tag
    public bool HasTag(GameObject obj, string tag)
    {
        return tagDictionary.ContainsKey(obj) && tagDictionary[obj].Contains(tag);
    }

    // Method to get all tags of a GameObject
    public HashSet<string> GetTags(GameObject obj)
    {
        if (tagDictionary.ContainsKey(obj))
        {
            return tagDictionary[obj];
        }
        return new HashSet<string>();
    }
}
