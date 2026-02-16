using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerForRigidBody : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private float m_jumpPower = 8.0f;
    [SerializeField, Tooltip("落下速度を調整できます。")] private float m_fallingSpeed = 1.0f;
    [SerializeField, Tooltip("最大スタミナ")] private float m_maxStamina = 3.0f;
    [SerializeField, Tooltip("最大回復するまでにかかる秒数")] private float m_fillStaminaSeconds = 1.0f;

    [Header("JumpParameter")]
    [SerializeField, Tooltip("ジャンプ判定の球体の半径を変更します。")] private float m_radius = 0.5f;
    [SerializeField, Tooltip("ジャンプ判定の球体のオフセット")] private Vector3 m_offset = Vector3.zero;
    [SerializeField, Tooltip("着地判定をするレイヤーを設定できます。")] private LayerMask m_detectionLayer;
    [SerializeField, Tooltip("ジャンプのクールダウンタイム")] private float m_jumpCoolDownTime = 0.5f;

    [Header("Component")]
    [SerializeField] private Rigidbody m_rigidBody;

    [Header("InGame")]
    [SerializeField, ReadOnly] private int m_direction = 1;
    [SerializeField, ReadOnly] private float m_axisX = 0.0f;
    [SerializeField, ReadOnly] private float m_axisY = 0.0f;
    [SerializeField, ReadOnly] private Vector3 m_moveDirectionLocal = Vector3.zero;
    [SerializeField, ReadOnly] private Vector3 m_moveDirection = Vector3.zero;
    [SerializeField, ReadOnly] private Vector3 m_velocity = Vector3.zero;
    [SerializeField, ReadOnly] private float m_currentStamina = 0.0f;
    [SerializeField, ReadOnly] private float m_healInterval = 0.5f;

    [SerializeField, ReadOnly] private bool m_isGround = false;
    [SerializeField, ReadOnly] private float m_jumpCoolDown = 0.0f;
    [SerializeField, ReadOnly] private bool m_isStan = false;
    //[SerializeField, ReadOnly] private bool m_canInput = true;

    private float m_count = 0.0f;

    private PlayerAttackAction m_attackAction;
    private PlayerRaise m_playerRaise;
    private PlayerInteractDetection m_interactDetection;

    #region Property
    public int DirectionX => m_direction;
    public Vector3 DirectionLocal => m_moveDirectionLocal;
    public Vector3 Velocity => m_rigidBody.linearVelocity;
    public float MaxStamina => m_maxStamina;
    public float CurrentStamina => m_currentStamina;
    public bool IsGround => m_isGround;
    public bool IsStan => m_isStan;
    //public bool CanInput => m_canInput;
    #endregion

    private void Awake()
    {

    }

    void Start()
    {
        m_attackAction = GetComponent<PlayerAttackAction>();
        m_playerRaise = GetComponentInChildren<PlayerRaise>();
        m_interactDetection = GetComponent<PlayerInteractDetection>();
        m_currentStamina = m_maxStamina;
    }

    void Update()
    {
        CheckGround();

        // スタンチェック
        if (m_currentStamina <= 0.0f)
        {
            m_isStan = true;
        }

        // スタミナ回復
        if (!m_playerRaise.isRaise && m_currentStamina < m_maxStamina)
        {
            // ボタン連打で永遠に持ててしまう不具合を対処
            if(m_count < m_healInterval)
            {
                m_count += Time.deltaTime;
            }
            else
            {
                float h = m_maxStamina / m_fillStaminaSeconds;
                m_currentStamina += Time.deltaTime * h;
            }

            if (m_isStan == true && m_currentStamina >= m_maxStamina)
                m_isStan = false;
        }

        // 連打対策
        if (m_playerRaise.isRaise)
            m_count = 0.0f;

        Mover();

        // ジャンプクールダウン
        m_jumpCoolDown = Mathf.Max(m_jumpCoolDown - Time.deltaTime, 0.0f);
    }

    // 固定フレームで呼び出しを行うことで実行環境のスペックに依存しなくなります。
    private void FixedUpdate()
    {
        
    }

    private void Mover()
    {
        // 前フレームの運動量の保存
        Vector3 old = m_rigidBody.linearVelocity;

        // 方向をVector3に変換
        m_moveDirectionLocal = new Vector3(m_axisX, 0, 0).normalized;        // 正規化
        m_moveDirection = transform.TransformDirection(m_moveDirectionLocal);// 方向をワールド空間に変換

        //// 入力にプレイヤー速度を適用
        m_velocity = (m_moveDirection * m_speed);

        // 前フレームの上下の運動を適用（主に落下速度）
        m_velocity.y = old.y;

        // 落下中の速度上昇を適用
        if (m_velocity.y < 0.0f)
            m_velocity.y += (Physics.gravity.y * m_fallingSpeed) * Time.deltaTime;

        //// プレイヤーの運動量へ適用
        m_rigidBody.linearVelocity = m_velocity;

        // エリア方向更新
        if (m_axisX != 0.0f)
        {
            m_direction = (int)m_axisX;
        }

        m_axisX = 0.0f;
        m_axisY = 0.0f;
    }

    /// <summary>
    /// 移動コマンドの受信
    /// </summary>
    public void Move(float x, float y)
    {
        if (!m_attackAction.isAttacking && !m_isStan && !m_interactDetection.isInteract)
        {
            m_axisX = x;
            m_axisY = y;
        }
    }

    /// <summary>
    /// 上方向にジャンプする。
    /// </summary>
    public void Jump()
    {
        if (m_isGround && !m_isStan && m_jumpCoolDown == 0.0f && !m_interactDetection.isInteract && !m_attackAction.isAttacking)
        {
            m_jumpCoolDown = m_jumpCoolDownTime;
            m_rigidBody.AddForce(new Vector3(0, m_jumpPower, 0), ForceMode.Impulse);
        }
    }

    public void Attack()
    {
        if (m_isGround && !m_isStan && !m_interactDetection.isInteract && !m_playerRaise.isRaise)
        {
            m_attackAction.Invoke();
        }
    }

    public void PickAndDrop()
    {
        if (!m_isStan)
        {
            m_playerRaise.Invoke();
        }
    }

    public void Cleaning()
    {
        if (m_isGround && !m_isStan && !m_playerRaise.isRaise)
        {
            m_interactDetection.Invoke();
        }
    }

    public void SetStamina(float value)
    {
        m_currentStamina = Mathf.Clamp(value, 0.0f, m_maxStamina);
    }

    

    /// <summary>
    /// 地面に着地しているか判定
    /// </summary>
    private void CheckGround()
    {
        //Vector3 pos = transform.position + m_offset;
        Vector3 pos = transform.position + Vector3.Scale(m_offset, transform.localScale);
        
        m_isGround = Physics.CheckSphere(pos, m_radius * transform.localScale.magnitude, m_detectionLayer);

        //Vector3 center = transform.position;
        //center.y -= m_capsuleCollider.height / 2 - m_capsuleCollider.radius * 0.5f;

        //m_isGround = Physics.CheckSphere(center, m_capsuleCollider.radius * 0.9f, m_detectionLayer);
    }

    /// <summary>
    /// CheckGroundの判定を可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        //Vector3 pos = transform.position + m_offset;
        Vector3 pos = transform.position + Vector3.Scale(m_offset, transform.localScale);
        Gizmos.DrawWireSphere(pos, m_radius * transform.localScale.magnitude);

        //Vector3 orijin = gameObject.transform.position;
        //orijin.y -= m_capsuleCollider.height / 2 - (m_capsuleCollider.radius * 0.5f);

        //Gizmos.DrawWireSphere(orijin, m_capsuleCollider.radius * 0.9f);
    }

    private void Reset()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }
}
