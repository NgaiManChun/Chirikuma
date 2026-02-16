using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface VacuumTarget
{
    public void ActivateVacuum(Transform _direction);
}

public class Skill_Vacuum : MonoBehaviour, ISkill
{
    [Header("SkillPreset")]
    [SerializeField] private SkillPreset_Vacuum m_preset;

    [Header("Parameter")]
#if UNITY_EDITOR
    [SerializeField] private Color m_gizmosColor = Color.red;
#endif

    [Header("InGame")]
    [SerializeField, ReadOnly] private GameObject m_player;
    [SerializeField, ReadOnly] private PlayerControllerForRigidBody m_playerController;
    [SerializeField, ReadOnly] private Vector3 m_inputDirection = Vector3.right;
    [SerializeField, ReadOnly] private bool m_vacuumStart = false;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerController = m_player.GetComponent<PlayerControllerForRigidBody>();

        // SkillHolderへの登録
        var holder = FindFirstObjectByType<SkillHolder>();
        holder.Register(this);
    }

    private void Start()
    {
        m_inputDirection = Vector3.right;
    }

    private void Update()
    {
        m_inputDirection.x = m_playerController.DirectionX;

        m_vacuumStart = Input.GetKey(m_preset.KeyCode);
    }

    public void DataSetting()
    {

    }

    public void Activate()
    {
        // FixedUpdateで実行したいため中身無し
    }

    public string GetName()
    {
        return m_preset.Name;
    }

    public KeyCode InputKey()
    {
        return m_preset.KeyCode;
    }

    public bool SkillAvaible()
    {
        return m_vacuumStart;
    }

    private void FixedUpdate()
    {
        // 吸引開始のフラグ処理
        if (m_vacuumStart == false) return;

        // 吸引範囲の中心位置
        Vector3 center = transform.position;
        center.x += m_preset.Offset * m_inputDirection.x;

        // 吸引範囲のサイズ決定（OverlapBoxで使用するために半分のサイズに計算しなおす）
        Vector3 halfSize = m_preset.RangeScale / 2;

        // ごみが範囲に存在しているか？
        List<Collider> trashList = Physics.OverlapBox(center, halfSize, Quaternion.identity, m_preset.VacuumTargetLayer).ToList();

        // ごみが存在しなかったらReturn
        if (trashList.Count <= 0) return;

        // 発見したごみ
        foreach(var trash in trashList)
        {
            Vector3 playerPos = transform.position;
            Vector3 trashPos = trash.transform.position;
            Vector3 direction = -(playerPos - trashPos); // Debugのためにここでは正規化を行っていません。

            // プレイヤーとごみの間に障害物があるかどうか検知します。
            if (!Physics.Raycast(playerPos, direction.normalized, Vector3.Distance(playerPos, trashPos), m_preset.ObstacleLayer)) // 障害物レイヤーをプリセットから設定可能
            {
                // InterfaceであるTrashObjectをtargetに代入
                var target = trash.GetComponent<VacuumTarget>();

                // targetに値があれば
                if (target != null)
                {
                    // Vacuum()を実行
                    target.ActivateVacuum(m_player.transform);
                }

                // 可視光線を描画（吸い込み可能なら緑色の線）
                Debug.DrawRay(playerPos, direction, Color.green);
            }
#if UNITY_EDITOR
            else
            {
                // 可視光線を描画（吸い込み可能なら緑色の赤）
                Debug.DrawRay(playerPos, direction, Color.red);
            }
#endif
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = m_gizmosColor;

        Vector3 center = transform.position;
        center.x += m_preset.Offset * m_inputDirection.x;

        Gizmos.DrawWireCube(center, m_preset.RangeScale);
    }
#endif
}