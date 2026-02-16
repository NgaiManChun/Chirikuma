using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class CameraInformation : MonoBehaviour
{
    [Header("このエリアでのカメラズームアウト量")]
    public Vector3 CameraZoomOut = Vector3.zero;

    //視野角（度)
    private float FieldOfView = 60.0f;

    //アスペクト比
    private float Aspect = 1.777f; // 16:9 = 1.777～

    [SerializeField] Vector3 areaSize = new Vector3(1.0f, 1.0f, 1.0f);

    public enum CameraDirection
    {
        NONE = -1,
        FORWARD,
        RIGHT,
        LEFT
    };

    public CameraDirection direction = CameraDirection.NONE;

    private void Start()
    {
        SyncBoxColliderToFrustum();

        if (transform.rotation.y == 0.0f)
        {
            direction = CameraDirection.FORWARD;
        }
        else if (transform.rotation.y > 0.0f)
        {
            direction = CameraDirection.RIGHT;
        }
        else if(transform.rotation.y < 0.0f)
        {
            direction = CameraDirection.LEFT;
        }

    }

    //描画カメラフレーム
    private void OnDrawGizmos()
    {
        // 注視点
        Vector3 lookAtPosition = transform.position;

        // カメラの実際の位置（ズームアウト込み）
        Vector3 cameraPosition = lookAtPosition - transform.rotation * CameraZoomOut;

        // 視線方向を描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(cameraPosition, lookAtPosition);

        // カメラから注視点までの距離を使用
        float distance = Vector3.Distance(cameraPosition, lookAtPosition);

        // 視野錐（カメラのフレーム）を描画
        Gizmos.color = Color.cyan;
        DrawFrustum(cameraPosition, transform.rotation, FieldOfView, distance, Aspect);
    }

    //フレーム(四角形)部分
    private void DrawFrustum(Vector3 position, Quaternion rotation, float fov, float maxDistance, float aspect)
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, fov, maxDistance, 0.1f, aspect);
        Gizmos.matrix = originalMatrix;
    }

    //判定用BoxColliderの位置、サイズ、回転を計算
    void SyncBoxColliderToFrustum()
    {
        Transform colliderHolder = GetComponentInChildren<BoxCollider>().transform;
        if (colliderHolder == null) return;

        BoxCollider box = colliderHolder.GetComponent<BoxCollider>();
        if (box == null) return;

        Vector2 frameSize = GetFrustumFrameSize();
        areaSize = new Vector3(frameSize.x, frameSize.y, areaSize.z);

        // 距離
        Vector3 lookAt = transform.position;
        Vector3 cameraPos = lookAt - transform.rotation * CameraZoomOut;
        float distance = Vector3.Distance(cameraPos, lookAt);

        // サイズ設定（ワールド向き）
        //box.size = new Vector3(frameSize.x, frameSize.y, 1.0f);
        box.size = areaSize;

        // 中心位置（BoxColliderはローカルなので、0にする）
        box.center = Vector3.zero;

        // colliderHolderのTransformをカメラ位置＋方向に配置
        colliderHolder.position = transform.position;
        colliderHolder.rotation = transform.rotation;
    }

    //カメラフレームの縦横計算
    private Vector2 GetFrustumFrameSize()
    {
        // カメラから注視点までの距離
        Vector3 lookAtPosition = transform.position;
        Vector3 cameraPosition = lookAtPosition - transform.rotation * CameraZoomOut;
        float distance = Vector3.Distance(cameraPosition, lookAtPosition);

        // 高さ・幅を計算
        float halfHeight = Mathf.Tan(Mathf.Deg2Rad * FieldOfView * 0.5f) * distance;
        float halfWidth = halfHeight * Aspect;

        return new Vector2(halfWidth * 2f, halfHeight * 2f); // 横幅, 高さ
    }

}
