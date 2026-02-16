using UnityEngine;

public class AreaProbe : MonoBehaviour
{
    //[Header("Parameter")]
    [Header("Parameter")]
    [SerializeField] private string m_searchTagTarget;
    [SerializeField] private LayerMask m_layermask;
    [SerializeField] private float m_rayDistance = 20;

    [Header("Component")]
    [SerializeField] private RoomArea m_roomArea;
    

    [Header("InGame")]
    [SerializeField, ReadOnly] private GameObject m_target;

    private void Awake()
    {
        m_target = GameObject.FindGameObjectWithTag(m_searchTagTarget);

        if(m_target == null)
        {
            Debug.LogError($"m_targetが見つかりませんでした。");
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 refPos = transform.position;
        Vector3 tarPos = m_target.transform.position;
        Vector3 direction = -(refPos - tarPos); // Debugのためにここでは正規化を行っていません。
        float distance = Vector3.Distance(refPos, tarPos);

        if (distance > m_rayDistance)
        {
            if(m_roomArea.probeRay == true)
            {
                m_roomArea.probeRay = false;
                if (m_roomArea.playerCollision == false)
                {
                    m_roomArea.RayHideArea(true);
                }
            }

            return;
        }

            if (!Physics.Raycast(refPos, direction.normalized, distance, m_layermask)) // 障害物レイヤーをプリセットから設定可能
        {
            if(m_roomArea.showUnmaskGraphic == true)
            {
                m_roomArea.RayHideArea(false);
            }

            // 可視光線を描画（吸い込み可能なら緑色の線）
            Debug.DrawRay(refPos, direction, Color.green);
        }
        else
        {
            if(m_roomArea.probeRay == true)
            {
                m_roomArea.probeRay = false;
                if (m_roomArea.playerCollision == false)
                {
                    m_roomArea.RayHideArea(true);
                }
            }
        }

        Debug.DrawRay(refPos, direction);
    }
}
