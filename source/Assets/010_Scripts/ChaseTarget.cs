using UnityEngine;

[ExecuteAlways]
public class ChaseTarget : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private GameObject m_target;
    [SerializeField, Range(0.01f, 10f)] private float m_smooth = 1;
    [SerializeField] private Vector3 m_offset = Vector3.zero;

    [SerializeField] private Vector3 m_zoomOutOffset = Vector3.zero;
    [SerializeField, Tooltip("ズームのアニメーション時間（秒）")] private float m_zoomTime = 1.0f;
    private PlayerControllerForRigidBody m_PlayerController = null;
    private float m_zoomCurrentTime = 0.0f;

    private void Start()
    {
        if (m_target)
        {
            m_PlayerController = m_target.GetComponent<PlayerControllerForRigidBody>();
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = m_target.transform.position + m_offset;
        
        if (Input.GetKey(KeyCode.P))
        {
            m_zoomCurrentTime = Mathf.Min(m_zoomCurrentTime +Time.deltaTime, m_zoomTime);
            //if (m_PlayerController)
            //{
            //    m_PlayerController.SetCanInput(false);
            //}
        }
        else
        {
            m_zoomCurrentTime = Mathf.Max(m_zoomCurrentTime - Time.deltaTime, 0.0f);
            //if (m_PlayerController)
            //{
            //    m_PlayerController.SetCanInput(true);
            //}
        }
        targetPos += m_zoomOutOffset * (m_zoomCurrentTime / m_zoomTime);
        Vector3 tPos = Vector3.Lerp(transform.position, targetPos, m_smooth * Time.deltaTime);

        transform.position = tPos;

        //transform.LookAt(m_target.transform);
    }

    
}
