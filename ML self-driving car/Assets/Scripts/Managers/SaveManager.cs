using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveManager
{
    public static SaveData CurrentSaveData { get; private set; } = new SaveData();
    public const string SaveDirectory = "Saves/";
    public const string FileName = "Save.game";
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
                string json = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<SaveData>(json);
            }
            else
            {
                Debug.LogError("Save file does not exist!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occurred while loading the save file: " + e.Message);
        }

        CurrentSaveData = data != null ? data : new SaveData();
    }

}
