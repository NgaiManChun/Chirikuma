using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform forcusTransform; // プレイヤーのTransform

    [Header("Parameter")]
    [SerializeField] private float m_mouseSensi = 5f; // 回転速度
    [SerializeField] private float m_smoothCameraSpeed = 1.0f;
    [SerializeField] private float m_distanceFromPlayer = 3f; // カメラとプレイヤーの距離
    [SerializeField] private Vector2 m_maxRotationX = new Vector2(-75, 75);
    //[SerializeField] private float m_maxRotationX = 75;
    [SerializeField] private float m_wallPadding = 0.15f;
    [SerializeField] private Vector2 m_closeUpDistance = new Vector2(0.2f, 1f);

#if UNITY_EDITOR
    [Header("Setting")]
    [SerializeField] private bool m_smoothingCamera = true;
#endif

    private Vector3 m_lastCameraPos = Vector3.zero;
    private float mouseW = 1.0f;

    private void FixedUpdate()
    {
        // マウスの入力を取得
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        mouseW += Input.GetAxisRaw("Mouse ScrollWheel");

        mouseW = Mathf.Clamp(mouseW, m_closeUpDistance.x, m_closeUpDistance.y);

        // プレイヤーの位置を中心にカメラを回転させる
        Vector3 rotation = new Vector3(-mouseY, mouseX, 0) * m_mouseSensi; // X軸とY軸回転

        transform.RotateAround(forcusTransform.position, Vector3.up, rotation.y);
        transform.RotateAround(forcusTransform.position, transform.right, rotation.x);



        //カメラの高さ上限を設定
        Vector3 localEulerAngles = transform.eulerAngles;
        localEulerAngles.x = ClampAngle(localEulerAngles.x, m_maxRotationX.x, m_maxRotationX.y);
        localEulerAngles.z = 0;
        transform.eulerAngles = localEulerAngles;

        if (localEulerAngles.x >= m_maxRotationX.y && localEulerAngles.x <= 360 + m_maxRotationX.x)
        {
            transform.position = m_lastCameraPos;
        }

        // カメラの位置をプレイヤーに追従させる
        Vector3 desiredPosition = forcusTransform.position - transform.forward * m_distanceFromPlayer * mouseW;

        // カメラの位置を壁の表面に制限
        RaycastHit hit;
        if (Physics.Raycast(forcusTransform.position, desiredPosition - forcusTransform.position, out hit, m_distanceFromPlayer * mouseW))
        {
            desiredPosition = hit.point + hit.normal * m_wallPadding;
        }

        Debug.DrawRay(forcusTransform.position, desiredPosition - forcusTransform.position, Color.yellow);


#if UNITY_EDITOR

        if (m_smoothingCamera == true)
        {
            //===段々寄っていくカメラを実装した場合は下記を使用してください。
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * m_smoothCameraSpeed);
            //===
        }
        else
        {
            //===壁を検知した場合すぐにカメラを移動する場合は下記を使用してください。
            transform.position = desiredPosition;
            //===
        }

#else
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * m_smoothCameraSpeed);
#endif

        m_lastCameraPos = transform.position;
    }


    //EularAngleを考慮したClamp
    //https://gist.github.com/johnsoncodehk/2ecb0136304d4badbb92bd0c1dbd8bae
    public float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;

        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}