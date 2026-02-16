using UnityEngine;

[CreateAssetMenu(fileName = "Preset_Wind", menuName = "SkillPreset/Wind", order = 1)]
public class SkillPreset_Wind : BaseSkillPreset
{
    [Header("Parameter(Derivation)")]
    [SerializeField] private float m_spawnOffset = 1.0f;

    public float SpawnOffset => m_spawnOffset;
}
