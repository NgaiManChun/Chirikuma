using UnityEngine;

[System.Serializable]
public class StageDataTable
{
    public enum ChallengeState
    {
        UnEntry,    // 未挑戦
        Entering,   // 挑戦済み
        Clear,      // クリア済み
    }

    [Header("内部データ用パラメータ")]
    [SerializeField] public int m_id = -1;
    [SerializeField] public string m_scenePath = "Scene_";
    [SerializeField] public ChallengeState m_challengeState = ChallengeState.UnEntry;

    [Header("UI用パラメータ")]
    [SerializeField] public string m_name = "ステージ名";
    [SerializeField, TextArea] public string m_context = "ステージ詳細";
}