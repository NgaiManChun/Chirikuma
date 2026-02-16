using System.IO;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private StageStorage m_stageStorage;

    private const string m_fileName = "saveData.json";
    private string m_filePath;

    public StageDataTable[] Storage => m_stageStorage.stages;

    private void Awake()
    {
#if !UNITY_EDITOR
        string appPath = Application.persistentDataPath;
#else
        string appPath = Application.dataPath;
#endif
        m_filePath = appPath + "/" + m_fileName;

        if(!File.Exists(m_filePath))
        {
            Save();
        }

        Load();
    }

    private void Save()
    {
        // ファイルにデータを書き込む
        string jsonStr = JsonUtility.ToJson(m_stageStorage, true);
        File.WriteAllText(m_filePath, jsonStr);
    }

    private void Load()
    {
        string jsonStr = File.ReadAllText(m_filePath);
        StageStorage data = StageStorage.CreateScriptableObjectFromJSON(jsonStr);
    }
}