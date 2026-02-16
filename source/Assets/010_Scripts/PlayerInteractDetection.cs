using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerInteract
{
    public void IInteract();
}


public class PlayerInteractDetection : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private Color m_gizmosColor = Color.green;
    [SerializeField] private float m_radius;
    [SerializeField] private LayerMask m_interactionTargetLayerMask;
    [SerializeField] private LayerMask m_obstacleLayer;

    [Header("InGame")]
    [SerializeField, ReadOnly] private GameObject m_player;
    [SerializeField, ReadOnly] private PlayerControllerForRigidBody m_playerController;
    [SerializeField, ReadOnly] private Vector3 m_inputDirection = Vector3.right;
    [SerializeField, ReadOnly] private IPlayerInteract m_nearInteract;
    [SerializeField, ReadOnly] private bool m_isInteract = false;

    public bool isInteract => m_isInteract;

    private bool isInvoke = false;

    private void Reset()
    {
        if (m_player == null)
            m_player = gameObject;
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerController = m_player.GetComponent<PlayerControllerForRigidBody>();
    }

    void Start()
    {
        m_inputDirection = Vector3.right;
    }

    void Update()
    {
        m_inputDirection.x = m_playerController.DirectionX;
        SearchInteraction();

        if (m_nearInteract != null && isInvoke)
        {
            m_nearInteract.IInteract();
            m_isInteract = true;
        }
        else
        {
            m_isInteract = false;
        }
        isInvoke = false;
    }

    private void SearchInteraction()
    {
        // 範囲の中心位置
        Vector3 center = transform.position;

        // 範囲のサイズ決定（OverlapBoxで使用するために半分のサイズに計算しなおす）
        float radius = m_radius * m_player.transform.localScale.magnitude;

        // 範囲に存在しているか？
        List<Collider> detectionList = Physics.OverlapSphere(center, radius, m_interactionTargetLayerMask).ToList();

        // 存在しなかったら
        if (detectionList.Count <= 0) {
            m_nearInteract = null;
            return;
        };

        Vector3 playerPos = transform.position;     // プレイヤーの位置

        // プレイヤーとターゲットの間に障害物がない＆＆最も近いターゲットの探索
        IPlayerInteract near = null;      // 最も近いターゲット
        float mostNearDistance = -1;

        foreach (var obj in detectionList)
        {
            Vector3 direction = -(playerPos - obj.transform.position); // Debugのためにここでは正規化を行っていません。
            float distance = Vector3.Distance(playerPos, obj.transform.position);
            Debug.DrawRay(playerPos, direction);

            if (!Physics.Raycast(playerPos, direction.normalized, Vector3.Distance(playerPos, obj.transform.position), m_obstacleLayer))
            {
                // mostNearDistanceが-1（未定義）または保存していた距離よりも新規の距離の方が近い場合
                if (mostNearDistance == -1 || distance < mostNearDistance)
                {
                    near = obj.GetComponent<IPlayerInteract>();

                    if (near == null)
                    {
                        Debug.LogError($"このオブジェクト({obj.name})にはIPlayerInteractがついていません。");
                        continue;
                    }

                    mostNearDistance = distance;
                }
            }
        }

        m_nearInteract = near;
    }

    public void Invoke()
    {
        isInvoke = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = m_gizmosColor;

        Gizmos.DrawWireSphere(m_player.transform.position, m_radius * m_player.transform.localScale.magnitude);
    }
#endif
}
