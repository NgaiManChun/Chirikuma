using UnityEngine;
using UnityEngine.UI;

public class FollowVisibleArea : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Transform m_followTarget;
    [SerializeField] private Image m_image;

    void Start()
    {
        m_image.enabled = true;
    }

    void Update()
    {
        Vector3 pos = m_followTarget.position;
        pos.z = 0;

        this.gameObject.transform.position = pos;
    }
}
