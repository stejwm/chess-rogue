using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class NameDatabase
{
    private static List<NameEntry> names;

    public static void LoadNames()
    {
        if (names == null)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "medieval_names.json");
            Debug.Log(path);
            if (File.Exists(path))
            {
                string jsonText = File.ReadAllText(path);
                NameListWrapper wrapper = JsonUtility.FromJson<NameListWrapper>("{\"names\":" + jsonText + "}");
                names = wrapper.names;
            }
            else
            {
                Debug.LogError("Name file not found!");
                names = new List<NameEntry>();
            }
        }
    }

    public static NameEntry GetRandomNameEntry()
    {
        if (names == null || names.Count == 0) return null;
        return names[Random.Range(0, names.Count)];
    }

    public static string GetRandomNameByGender(Gender gender)
    {
        if (names == null || names.Count == 0) return "Unknown";
        List<NameEntry> filtered = names.FindAll(n => n.gender == gender);
        if (filtered.Count == 0) return "Unknown";
        return filtered[Random.Range(0, filtered.Count)].name;
    }
}

[System.Serializable]
public class NameListWrapper
{
    public List<NameEntry> names;
}

[System.Serializable]
public class NameEntry
{
    public string name;
    public Gender gender;
}