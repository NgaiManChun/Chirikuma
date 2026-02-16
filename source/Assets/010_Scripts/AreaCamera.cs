using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using static CameraInformation;

public class AreaCamera : MonoBehaviour
{
    [Header("カメラマネージャー")]
    [SerializeField] public CameraManager cameraManager = null;

    [Header("描画用カメラ情報(初期カメラをセット)")]
    [SerializeField] public CameraInformation cameraInformation = null;

    //次のカメラエリア
   private CameraInformation nextCameraInformation = null;
   
    //水平方向のカメラエリア
   private CameraInformation horizonInfo = null;
    //垂直方向のカメラエリア
   private CameraInformation verticalInfo = null;
    //戻るためのカメラエリア
   private CameraInformation backInfo = null;

    [Header("補間スピード")]
    [SerializeField] private float moveSpeed = 1.0f;

    [Header("エリア移動の起点となる対象オブジェクトのタグ")]
    public string TargetTag = "Player";
    //追跡するプレイヤーオブジェクト
    private GameObject Player = null;

    //カメラ移動フラグ
    private bool isMoving = false;
    //カメラ移動処理達成率
    private float transitionProgress = 0.0f;

    [Header("カメラの移動入力")]
    [SerializeField] private KeyCode moveKey = KeyCode.W;

    [SerializeField] public Rumba rumba = null;

    //プレイヤーの移動補間用の一時変数
    private Vector3 PlayerPos, nextPlayerPos;

    void Start()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if(cameraManager == null)
        {
            Debug.Log("カメラマネージャーがありません");
        }
        //ターゲットのプレイヤーを設定
        Player = cameraManager.TargetPlayer;

        if (cameraInformation == null)
        {
            Debug.Log("最初のカメラが設定されていません。");
            return;
        }

        rumba=cameraManager.TargetRumba.GetComponent<Rumba>();

        // カメラの初期位置設定
        transform.position = cameraInformation.transform.position - cameraInformation.CameraZoomOut;
        transform.LookAt(cameraInformation.transform.position);

        
    }

    void Update()
    {
        //移動フラグがONかつ次のカメラエリアがセットされている
        if (isMoving && nextCameraInformation != null)
        {
            //移動進捗率を加算
            transitionProgress += Time.deltaTime * moveSpeed;

            //進捗率が1.0以上で移動終了
            if (transitionProgress >= 1.0f)
            {
                //進捗率を1.0fに補正
                transitionProgress = 1.0f;
             
            }

            // 補間：位置
            transform.position = Vector3.Lerp(
                cameraInformation.transform.position - cameraInformation.transform.rotation * cameraInformation.CameraZoomOut,
                nextCameraInformation.transform.position - nextCameraInformation.transform.rotation * nextCameraInformation.CameraZoomOut,
                transitionProgress
            );

            // 補間：回転
            transform.rotation = Quaternion.Slerp(
                cameraInformation.transform.rotation,
                nextCameraInformation.transform.rotation,
                transitionProgress
                );

            // プレイヤーポジションの補間
            Player.transform.position = Vector3.Lerp(
               PlayerPos,
               nextPlayerPos,
               transitionProgress
           );

            //移動フラグがOFFになっていた移動終了処理
            if (transitionProgress >= 1.0f)
            {
                //プレイヤーのFreeze情報を更新
                SetObjectFreeze(Player);

                //現在のカメラ情報を更新
                cameraInformation = nextCameraInformation;
                //次のカメラ情報を消去
                nextCameraInformation = null;

                //強制的に止めていたプレイヤーの移動処理を有効化
                cameraManager.TargetPlayer.GetComponent<PlayerControllerForRigidBody>().enabled = true;

                isMoving = false;
            }
        }

        if (!isMoving)
        {
            //奥方向のカメラ移動
            if (Input.GetKey(moveKey))
            {
                InputMoveArea();

                //ルンバを持っていたらルンバのカメラも移動
                if (rumba.IsPickedup)
                {
                    //GetComponentInParent<AreaRumba>().MoveAreaInput(nextCameraInformation);
                    //SetObjectFreeze(rumba.gameObject);
                    rumba.GetComponent<Rumba>().SetAreaDirect(nextCameraInformation.transform.right);
                }
            }
        }

        //RayUpdate();

        //プレイヤーy軸をカメラに合わせる
        Player.transform.rotation = transform.rotation;

    }

    public void SetArea(CameraInformation info)
    {
        if (isMoving) { return; }

        if(info == cameraInformation) { return; }

        if (info.direction == cameraInformation.direction)
        {
            if (horizonInfo == null)
            {
                horizonInfo = info;
            }
        }
        else if (info.direction != cameraInformation.direction)
        {
            if(verticalInfo == null)
            {
                verticalInfo = info;
            }
        }
    }


    //カメラの自動移動
    public void MoveArea(CameraInformation info)
    {
        if (isMoving) { return; }
        if(nextCameraInformation != null) { return; }

        if(info == verticalInfo)
        {
            verticalInfo = null;
            backInfo = null;
        }

        if(info == cameraInformation && horizonInfo != null)
        {
            nextCameraInformation = horizonInfo;
            horizonInfo = null;
            MovePara();
        }
       
    }

    //カメラの入力移動
    private void InputMoveArea()
    {
        if (isMoving) { return; }
        if (nextCameraInformation != null) { return; }
        if(verticalInfo == null) { return;}

        nextCameraInformation=verticalInfo;
        verticalInfo = cameraInformation;
        MovePara();


    }

    private void MovePara()
    {
        //移動フラグON
        isMoving = true;
        //移動新緑率をリセット
        transitionProgress = 0.0f;


        //移動処理を制御する関数がないため、一旦移動処理スクリプトを無理やり無効化する
        Player.GetComponent<PlayerControllerForRigidBody>().enabled = false;
        //移動速度を0にする
        Player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        PlayerPos = Player.transform.position;
        if (nextCameraInformation.direction == CameraDirection.FORWARD)
        {
            nextPlayerPos = new Vector3(PlayerPos.x, PlayerPos.y, nextCameraInformation.transform.position.z);
        }
        //カメラの方向が左右ならプレイヤーのX座標は動かない
        else
        {
            nextPlayerPos = new Vector3(nextCameraInformation.transform.position.x, PlayerPos.y, PlayerPos.z);
        }

    }

    //プレイヤーのFreeze設定
    private void SetPlayerFreeze()
    {
        if (nextCameraInformation == null || Player == null) return;

        Rigidbody rb = Player.GetComponent<Rigidbody>();
        if (rb == null) return;

        //カメラの方向が前方ならプレイヤーのZ座標は動かない
        if(nextCameraInformation.direction == CameraDirection.FORWARD)
        {
            //Freeze設定
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            //プレイヤーのZ座標をエリアのZ座標に合わせる(ズレ防止)
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, nextCameraInformation.transform.position.z);
        }
        //カメラの方向が左右ならプレイヤーのX座標は動かない
        else
        {
            //Freeze設定
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
            //プレイヤーのX座標をエリアのX座標に合わせる(ズレ防止)
            Player.transform.position = new Vector3(nextCameraInformation.transform.position.x, Player.transform.position.y, Player.transform.position.z);
        }


    }

    private void SetObjectFreeze(GameObject obj)
    {
        if (nextCameraInformation == null || obj == null) return;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) return;

        //カメラの方向が前方ならプレイヤーのZ座標は動かない
        if (nextCameraInformation.direction == CameraDirection.FORWARD)
        {
            //Freeze設定
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            //プレイヤーのZ座標をエリアのZ座標に合わせる(ズレ防止)
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, nextCameraInformation.transform.position.z);
        }
        //カメラの方向が左右ならプレイヤーのX座標は動かない
        else
        {
            //Freeze設定
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
            //プレイヤーのX座標をエリアのX座標に合わせる(ズレ防止)
            obj.transform.position = new Vector3(nextCameraInformation.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        }


    }

    public bool IsCameraMoving()
    {
        return isMoving; // AreaCamera 内部のフラグ
    }
    
    public bool VerticalInfo()
    {
        if (verticalInfo != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    

    //プレイヤーとカメラの間にある物を消す(急ピッチ作成につきバグあり)
    //private List<GameObject> hiddenObjects = new List<GameObject>();

    //private void RayUpdate()
    //{
    //    // 既に隠していたものを表示に戻す
    //    foreach (GameObject obj in hiddenObjects)
    //    {
    //        if (obj != null)
    //        {
    //            Renderer renderer = obj.GetComponent<Renderer>();
    //            if (renderer != null) renderer.enabled = true;
    //        }
    //    }
    //    hiddenObjects.Clear();

    //    Vector3 toPlayer = Player.transform.position - transform.position;
    //    float distance = toPlayer.magnitude;

    //    Ray ray = new Ray(transform.position, toPlayer);

    //    Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);

    //    // プレイヤーとの間にあるすべてのオブジェクトを取得
    //    RaycastHit[] hits = Physics.RaycastAll(ray, distance);

    //    foreach (RaycastHit hit in hits)
    //    {
    //        GameObject hitObject = hit.collider.gameObject;

    //        // プレイヤー自身は無視
    //        if (hitObject.CompareTag("Player")) continue;

    //        Renderer renderer = hitObject.GetComponent<Renderer>();
    //        if (renderer != null)
    //        {
    //            renderer.enabled = false;
    //            hiddenObjects.Add(hitObject);
    //        }
    //    }
    //}

}
