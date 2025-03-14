using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class NameDatabase
{
    private static List<string> names;

    public static void LoadNames()
    {
        if (names == null)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "medieval_names.json");
            if (File.Exists(path))
            {
                string jsonText = File.ReadAllText(path);
                NameListWrapper wrapper = JsonUtility.FromJson<NameListWrapper>("{\"names\":" + jsonText + "}");
                names = wrapper.names;
            }
            else
            {
                Debug.LogError("Name file not found!");
                names = new List<string>();
            }
        }
    }

    public static string GetRandomName()
    {
        if (names == null || names.Count == 0) return "Unknown";
        return names[Random.Range(0, names.Count)];
    }
}

[System.Serializable]
public class NameListWrapper
{
    public List<string> names;
}