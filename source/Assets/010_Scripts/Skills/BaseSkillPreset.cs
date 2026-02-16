using UnityEngine;

/// <summary>
/// スキルのパラメーターを保持した基底のプリセットクラス
/// </summary>
[System.Serializable]
public abstract class BaseSkillPreset : ScriptableObject
{
    [Header("SkillPrefab")]
    [SerializeField, Tooltip("アクティブした際に生成するオブジェクト")] protected GameObject m_skillPrefab;

    [Header("Activate Key")]
    [SerializeField] protected KeyCode m_keyCode;

    [Header("Parameter(Base)")]
    [SerializeField, Tooltip("スキル名")] protected string m_name;
    [SerializeField, Tooltip("使用可能までのクールタイム")] protected float m_cooltime;

    #region Property
    public GameObject SkillPrefab => m_skillPrefab;
    public string Name => m_name;
    public KeyCode KeyCode => m_keyCode;
    public float Cooltime => m_cooltime;
    #endregion
}