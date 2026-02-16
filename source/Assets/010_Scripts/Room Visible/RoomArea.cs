using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class RoomArea : MonoBehaviour
{
    [Header("Require Attach")]
    [SerializeField] private Image m_image;
    [SerializeField] private Unmask m_visibleArea;
    [SerializeField] private Transform m_rayRefrencePos;

    public bool probeRay = false;
    public bool playerCollision = false;

    public bool showUnmaskGraphic => m_visibleArea.showUnmaskGraphic;

    private void Start()
    {
        m_visibleArea.showUnmaskGraphic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideArea(false);
            playerCollision = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideArea(true);
            playerCollision = false;
        }
    }

    public void HideArea(bool _value)
    {
        m_visibleArea.showUnmaskGraphic = _value;
    }

    public void RayHideArea(bool _value)
    {
        m_visibleArea.showUnmaskGraphic = _value;
        probeRay = true;
    }
}