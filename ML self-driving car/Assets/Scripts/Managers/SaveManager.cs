using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveManager
{
    public const string SaveDirectory = "Saves";
    public const string FileName = "Save.json";
    public static SaveData CurrentSaveData { get; private set; } = new SaveData();
    public static void Save()
    {
        string directory = Path.Combine(Application.persistentDataPath, SaveDirectory);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string json = JsonConvert.SerializeObject(CurrentSaveData);
        File.WriteAllText(Path.Combine(directory, FileName), json);
    }
    public static void Load()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveDirectory, FileName);
        SaveData data = null;
        try
        {
            if (File.Exists(filePath))
            {
                data = LoadFromFile(filePath);
            }
            else
            {
                CreateDefaultSaveFile(filePath);
                data = LoadFromFile(filePath);
                Debug.LogWarning("Save file does not exist! Using default.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("An error occurred while loading the save file. Using default. Error message: " + e.Message);
            CreateDefaultSaveFile(filePath);
            data = LoadFromFile(filePath);
        }

        CurrentSaveData = data != null ? data : new SaveData();
    }
    private static void CreateDefaultSaveFile(string saveFilePath)
    {
        TextAsset defaultSaveFile = Resources.Load<TextAsset>("DefaultSave");
        File.WriteAllText(saveFilePath, defaultSaveFile.text);
    }
    private static SaveData LoadFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<SaveData>(json);
    }
}
