using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.Networking;
using System.IO;

public class FileHandler : MonoBehaviour
{
    private const string FileExtension = "json";
    [SerializeField] private SaveDataManager saveDataManager;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    private void OpenFileWebGL()
    {
        UploadFile(gameObject.name, "OnFileUpload", $".{FileExtension}", false);
    }

    // Called from browser
    public void OnFileUpload(string url)
    {
        StartCoroutine(OutputRoutine(url));
    }

    private void SaveFileWebGL(){
        var bytes = Encoding.UTF8.GetBytes(MapToJson());
        DownloadFile(gameObject.name, "OnFileDownload", "", bytes, bytes.Length);
    }

    // Called from browser
    public void OnFileDownload() {
        Debug.Log("File Successfully Downloaded");
    }
#endif

    // Standalone platforms & editor
    private void OpenFileStandalone()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open", "", FileExtension, false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Web Request ERROR: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            saveDataManager.LoadMapData(SaveManager.Import(json));
        }
    }
    private void SaveFileStandalone()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save as", "", "", FileExtension);
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, MapToJson());
        }
    }
    private string MapToJson()
    {
        SaveData saveData = new SaveData();
        saveDataManager.UpdateMap();
        saveDataManager.SaveMapData(saveData);
        return SaveManager.Export(saveData);
    }
    public void OpenFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenFileWebGL();
#else
        OpenFileStandalone();
#endif
    }
    public void SaveFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveFileWebGL();
#else
        SaveFileStandalone();
#endif
    }
}
