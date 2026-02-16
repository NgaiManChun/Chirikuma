using UnityEngine;

[CreateAssetMenu(fileName = "Preset_Airblower", menuName = "SkillPreset/Airblower", order = 1)]
public class SkillPreset_Airblower : BaseSkillPreset
{
    [Header("Parameter(Derivation)")]
    [SerializeField] private float m_offset = 3.0f;
    [SerializeField] private Vector3 m_rangeScale = Vector3.one;
    [SerializeField] private LayerMask m_vacuumTargetLayer;
    [SerializeField] private LayerMask m_obstacleLayer;

    public float Offset => m_offset;
    public Vector3 RangeScale => m_rangeScale;
    public LayerMask VacuumTargetLayer => m_vacuumTargetLayer;
    public LayerMask ObstacleLayer => m_obstacleLayer;
}
