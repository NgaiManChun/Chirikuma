using UnityEngine;

[CreateAssetMenu(fileName = "Preset_Electical", menuName = "SkillPreset/Electical", order = 1)]
public class SkillPreset_Electical : BaseSkillPreset
{
    [Header("Parameter(Derivation)")]
    [SerializeField] private float m_lifeTime = 3.0f;

    public float LifeTime => m_lifeTime;
}
