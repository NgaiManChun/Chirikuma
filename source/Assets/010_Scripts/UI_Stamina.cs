//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stamina : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private Vector3 m_worldOffset; // オブジェクト位置のオフセット
    [SerializeField] private Color m_defaultColor = Color.yellow;
    [SerializeField] private Color m_lowStaminaReportColor = Color.red;
    [SerializeField] private Color m_stanStateStaminaColor = Color.gray;
    [SerializeField] private float m_fadeSpeed = 2;
    [SerializeField, Tooltip("点滅するタイミング(%)"), Range(0.01f, 0.99f)] private float m_lowStaminaReportPercent = 0.2f;

    [Header("Component")]
    [SerializeField] private RectTransform m_rect;
    [SerializeField] private Image m_image;

    [Header("InGame")]
    [SerializeField, ReadOnly] private Camera m_mainCamera;
    [SerializeField, ReadOnly] private PlayerControllerForRigidBody m_player;
    [SerializeField, ReadOnly] private PlayerRaise m_raise;

    private float m_count;

    private void Reset()
    {
        m_rect = this.GetComponent<RectTransform>();
        m_image = this.GetComponentInChildren<Image>();
    }

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerForRigidBody>();
        m_raise = FindAnyObjectByType<PlayerRaise>();
        m_mainCamera = Camera.main;

        Color color = m_defaultColor;
        color.a = 0.0f;
        m_image.color = color;
    }

    void Start()
    {

    }

    void Update()
    {
        m_image.fillAmount = m_player.CurrentStamina / m_player.MaxStamina;

        Color color = m_image.color;
        // 消費している
        if (m_player.MaxStamina > m_player.CurrentStamina)
        {
            if (color.a < 1.0f)
            {
                color.a += Time.deltaTime * m_fadeSpeed;

                m_image.color = color;
            }
        }
        // 最大まで溜まっている
        else if (m_player.MaxStamina <= m_player.CurrentStamina)
        {
            if (color.a > 0.0f)
                color.a -= Time.deltaTime * m_fadeSpeed;

            m_image.color = color;
        }

        Color changeColor;
        float alpha = m_image.color.a;
        // スタミナ残り僅か
        if (m_player.CurrentStamina <= m_player.MaxStamina * m_lowStaminaReportPercent)
        {
            if (m_count < 1.0f)
                m_count += Time.deltaTime * 5;
        }
        else
        {
            if (m_count > 0.0f)
                m_count -= Time.deltaTime * 5;
        }

        if (m_player.IsStan == false)
            changeColor = Color.Lerp(m_defaultColor, m_lowStaminaReportColor, m_count);
        else
            changeColor = m_stanStateStaminaColor;
        changeColor.a = alpha;
        m_image.color = changeColor;

        DrawScreen();

    }

    void DrawScreen()
    {
        Vector3 offset = m_worldOffset;
        offset.x *= m_player.DirectionX;
        Vector3 drawPos = m_player.transform.position + offset;
        m_rect.position = RectTransformUtility.WorldToScreenPoint(m_mainCamera, drawPos);
    }
}
