//using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class AreaCameraUI : MonoBehaviour
{
    [Header("UIアイコン")]
    [SerializeField] private RectTransform TargetIcon;

    [Header("ターゲット（ルンバ）")]
    [SerializeField] private Transform Target;

    [Header("エリアカメラ")]
    [SerializeField] private Camera Cam;

    [Header("スクリーン端のマージン")]
    [SerializeField] private float EdgeMargin = 50f;


    void Update()
    {
        if (Target == null || Cam == null || TargetIcon == null)
            return;

        if (Cam.GetComponent<AreaCamera>().IsCameraMoving())
        {
           TargetIcon.gameObject.SetActive(false);
            return;
        }

        if (CheckInCameraArea())
        {
            SetUIPosition();
        }
        else
        {
            TargetIcon.gameObject.SetActive(false);
        }
    }

    private bool CheckInCameraArea()
    {
        Vector3 screenPos = Cam.WorldToScreenPoint(Target.position);

        // ターゲットがカメラの前方にあるか
        bool isInFront = screenPos.z > 0;

        // スクリーン範囲外かどうか
        bool isOutOfScreen =
            screenPos.x < 0 || screenPos.x > Screen.width ||
            screenPos.y < 0 || screenPos.y > Screen.height || !isInFront;

        TargetIcon.gameObject.SetActive(isOutOfScreen);

        return  isOutOfScreen;
    }

    private void SetUIPosition()
    {
        Vector3 screenPos = Cam.WorldToScreenPoint(Target.position);

        // 水平方向 or 奥行き方向に応じてレーン制限
        float screenCenterX = Screen.width / 2f;
        float screenCenterY = Screen.height / 2f;

        // 方向ベクトル
        Vector3 toTarget = (Target.position - Cam.transform.position).normalized;

        // カメラのローカル空間での相対方向
        Vector3 localDir = Cam.transform.InverseTransformDirection(toTarget);

        Vector2 finalPos = Vector2.zero;

        if (Mathf.Abs(localDir.x) > Mathf.Abs(localDir.z))
        {
            // 横方向に優先して外れてる → 左右端
            if (localDir.x > 0)
                finalPos.x = Screen.width - EdgeMargin;
            else
                finalPos.x = EdgeMargin;

            finalPos.y = Mathf.Clamp(screenPos.y, EdgeMargin, Screen.height - EdgeMargin);
        }
        else
        {
            // 奥行き方向に優先して外れてる → 上下端
            if (localDir.z > 0)
                finalPos.y = Screen.height - EdgeMargin;
            else
                finalPos.y = EdgeMargin;

            finalPos.x = Mathf.Clamp(screenPos.x, EdgeMargin, Screen.width - EdgeMargin);
        }

        TargetIcon.position = finalPos;

    }

}
