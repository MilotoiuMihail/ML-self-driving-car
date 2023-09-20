using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveManager
{
    public const string SaveDirectory = "Saves";
    public const string FileName = "Save.json";
    public static SaveData CurrentSaveData { get; private set; } = new SaveData();
    public static string SaveDirectoryPath = Path.Combine(Application.persistentDataPath, SaveDirectory);
    public static void Save()
    {
        CheckForDirectory();
        string json = JsonConvert.SerializeObject(CurrentSaveData);
        File.WriteAllText(Path.Combine(SaveDirectoryPath, FileName), json);
    }
    public static void Load()
    {
        CheckForDirectory();
        string filePath = Path.Combine(SaveDirectoryPath, FileName);
        SaveData data = null;
        try
        {
            if (File.Exists(filePath))
            {
                data = LoadFromFile(filePath);
            }
            else
            {
                data = LoadDefault(filePath);
                Debug.LogWarning("Save file does not exist! Using default.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("An error occurred while loading the save file. Using default. Error message: " + e.Message);
            data = LoadDefault(filePath);
        }

        CurrentSaveData = data != null ? data : new SaveData();
    }
    private static void CheckForDirectory()
    {
        if (!Directory.Exists(SaveDirectoryPath))
        {
            Directory.CreateDirectory(SaveDirectoryPath);
        }
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
    private static SaveData LoadDefault(string filePath)
    {
        CreateDefaultSaveFile(filePath);
        return LoadFromFile(filePath);
    }
    public static SaveData Import(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("An error occurred while importing the save file. Error message: " + e.Message);
        }
        return new SaveData();
    }
    public static string Export(SaveData saveData)
    {
        return JsonConvert.SerializeObject(saveData);
    }
}
