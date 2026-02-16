using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    [SerializeField] private Vector3 m_staticPos;

    [Header("InGame")]
    [SerializeField, ReadOnly] private Camera m_camera;
    [SerializeField, ReadOnly] private ChaseTarget m_target;

    private void Awake()
    {
        m_camera = Camera.main;
        m_target = m_camera.GetComponent<ChaseTarget>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_target.enabled = false;
            m_camera.transform.position = m_staticPos;
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_target.enabled = true;
            //m_camera.transform.position = m_staticPos;
        }
    }
}
