using NUnit.Framework;
using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public interface IDamage
{
    public void Send(float _value);
}


public class PlayerAttackAction : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField, Tooltip("ダメージ量")] private float m_damage;
    [SerializeField, Tooltip("攻撃継続時間")] private float m_attackDuration = 0.05f;
    [SerializeField, Tooltip("攻撃範囲")] private Vector3 m_attackRange = Vector3.one;
    [SerializeField, Tooltip("攻撃範囲のオフセット")] private Vector3 m_offset = Vector3.zero;
    [SerializeField, Tooltip("攻撃対象のレイヤー")] private LayerMask m_enemyLayer;

    [Header("InGame")]
    [SerializeField, ReadOnly] private GameObject m_player;
    [SerializeField, ReadOnly] private PlayerControllerForRigidBody m_playerCon;
    [SerializeField, ReadOnly] private bool m_isAttacking = false;
    [SerializeField, ReadOnly] private float m_attackTimeCount = 0.0f;

    public bool isAttacking => m_isAttacking;

    private void Reset()
    {
        Initialize();
    }

    private void Start()
    {
        if (m_player == null)
        {
            Initialize();
        }
    }

    private void Update()
    {
        m_isAttacking = (m_attackTimeCount > 0.0f) ? true : false;
    }

    private void FixedUpdate()
    {
        if (m_isAttacking == true || m_attackTimeCount > 0.0f)
        {
            //Vector3 center = transform.position + (m_offset * m_playerCon.DirectionX);
            Vector3 center = transform.position + (Vector3.Scale(m_offset, transform.localScale) * m_playerCon.DirectionX);
            //Vector3 rangeSize = new Vector3(m_attackRange.x / 2, m_attackRange.y / 2, m_attackRange.z / 2);
            Vector3 rangeSize = Vector3.Scale(m_attackRange, transform.localScale) * 2;
            Collider[] enemys = Physics.OverlapBox(center, rangeSize, Quaternion.identity, m_enemyLayer);

            if (enemys.Length != 0)
            {
                foreach (var a in enemys)
                {
                    IDamage enemy = a.GetComponent<IDamage>();

                    if (enemy != null)
                    {
                        enemy.Send(m_damage);
                    }
                    else
                    {
                        Debug.LogError($"{a.name}にIDamageがインターフェイスが継承されていません。");
                        continue;
                    }
                }
            }
        }

        if (m_attackTimeCount > 0.0f)
            m_attackTimeCount -= Time.deltaTime;
        else
            m_attackTimeCount = 0.0f;
    }


    private void Initialize()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerCon = m_player.GetComponent<PlayerControllerForRigidBody>();
    }

    public void Invoke()
    {
        m_attackTimeCount = m_attackDuration;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = (m_isAttacking) ? Color.red : Color.yellow;
        //Vector3 center = transform.position + (m_offset * m_playerCon.DirectionX);
        Vector3 center = transform.position + (Vector3.Scale(m_offset, transform.localScale) * m_playerCon.DirectionX);
        Gizmos.DrawWireCube(center, Vector3.Scale(m_attackRange, transform.localScale));
    }
}