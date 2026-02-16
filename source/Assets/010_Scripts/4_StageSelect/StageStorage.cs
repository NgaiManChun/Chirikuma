using UnityEngine;

[CreateAssetMenu(fileName = "StageStorage", menuName = "CreateStageStorage")]
public class StageStorage : ScriptableObject
{
    [SerializeField] public StageDataTable[] stages;

    // ScriptableObject‚ðCreateInstance()‚·‚é‚É‚ÍJsonUtility.FromJsonOverwrite‚ª•K—v
    public static StageStorage CreateScriptableObjectFromJSON(string jsonString)
    {
        StageStorage data = CreateInstance<StageStorage>();
        JsonUtility.FromJsonOverwrite(jsonString, data);
        return data;
    }
}