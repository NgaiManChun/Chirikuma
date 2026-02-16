using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface AirblowerTarget
{
    public void ActivateAirblower();
}


public class Skill_Airblower : MonoBehaviour, ISkill
{
    [Header("Parameter")]
    [SerializeField] private SkillPreset_Airblower m_preset;

    [Header("Parameter")]
#if UNITY_EDITOR
    [SerializeField] private Color m_gizmosColor = Color.red;
#endif

    [Header("InGame")]
    [SerializeField, ReadOnly] private GameObject m_player;
    [SerializeField, ReadOnly] private PlayerControllerForRigidBody m_playerController;
    [SerializeField, ReadOnly] private Vector3 m_inputDirection = Vector3.right;
    [SerializeField, ReadOnly] private bool m_blowerStart = false;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerController = m_player.GetComponent<PlayerControllerForRigidBody>();

        // SkillHolderへの登録
        var holder = FindFirstObjectByType<SkillHolder>();
        holder.Register(this);
    }

    void Start()
    {
        m_inputDirection = Vector3.right;
    }

    void Update()
    {
        m_inputDirection.x = m_playerController.DirectionX;

        m_blowerStart = Input.GetKey(m_preset.KeyCode);
    }

    private void FixedUpdate()
    {
        // 吸引開始のフラグ処理
        if (m_blowerStart == false) return;

        // 吸引範囲の中心位置
        Vector3 center = transform.position;
        center.x += m_preset.Offset * m_inputDirection.x;

        // 吸引範囲のサイズ決定（OverlapBoxで使用するために半分のサイズに計算しなおす）
        Vector3 halfSize = m_preset.RangeScale / 2;

        // 送風対象が存在するか
        List<Collider> targetList = Physics.OverlapBox(center, halfSize, Quaternion.identity).ToList();

        // 送風対象が存在しなかったらReturn
        if (targetList.Count <= 0) return;

        // 発見した対象
        foreach (var obj in targetList)
        {
            var target = obj.GetComponent<AirblowerTarget>();

            if (target != null)
            {
                Vector3 playerPos = transform.position;
                Vector3 trashPos = obj.transform.position;
                Vector3 direction = -(playerPos - trashPos); // Debugのためにここでは正規化を行っていません。

                // プレイヤーと送風対象の間に障害物があるかどうか検知します。
                if (!Physics.Raycast(playerPos, direction.normalized, Vector3.Distance(playerPos, trashPos), m_preset.ObstacleLayer)) // 障害物レイヤーをプリセットから設定可能
                {
                    target.ActivateAirblower();

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
    }

    public void Activate()
    {

    }

    public void DataSetting()
    {

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
        return true;
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
