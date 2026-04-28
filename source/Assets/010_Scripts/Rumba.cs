using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.ShaderKeywordFilter;
//using UnityEditor.UIElements;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Rumba : MonoBehaviour, IRaiseAction
{

    // =======================================================
    // 参照・判定設定
    // =======================================================

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Header("地面チェックボックスOffset")]
    private float floorBoxYOffset = 0;
    [SerializeField, Header("地面チェックボックスサイズ")]
    private Vector3 floorBox = Vector3.one;
    [SerializeField, Header("地面レイヤー")]
    private LayerMask groundLayerMask;
    [SerializeField, Header("障害物Ray座標")]
    private Transform obstaclesDetect;
    [SerializeField, Header("障害物レイヤー")]
    private LayerMask obstaclesLayerMask;
    [SerializeField, Header("口の当たり判定位置")]
    private Transform dustDetect;
    [SerializeField, Header("口の当たり判定半径")]
    private float dustDetectRadius = 0.15f;
    [SerializeField, Header("埃レイヤー")]
    private LayerMask dustLayerMask;
    [SerializeField]
    private string enemyTag = "Enemy";

    // =======================================================
    // ステータス・行動設定
    // =======================================================

    [SerializeField]
    private int maxHP = 3;
    [SerializeField]
    private int currentHP = 3;
    [SerializeField]
    private float speed = 0.5f;
    [SerializeField, Header("行き止まりに止まる時間")]
    private float stopTime = 1.0f;
    [SerializeField, Header("埃を食べる時間")]
    private float eatTime = 0.5f;
    [SerializeField, Header("ダメージ受けた時の無敵時間")]
    private float damageTime = 3.0f;
    [SerializeField, Header("ダメージ受けた時の逃げる時間")]
    private float runningTime = 5.0f;
    [SerializeField, Header("逃げる時のスピードの倍率")]
    private float runningMagnification = 3.0f;

    // =======================================================
    // 状態
    // =======================================================

    [SerializeField]
    private AnimationClip eatAnimation;
    [SerializeField, Header("持ち上げられてるか")]
    private bool isPickedup = false;
    [SerializeField, Header("稼働中")]
    private bool inWork = false;

    [SerializeField, ReadOnly]
    private int direct = 1;
    [SerializeField, ReadOnly]
    Vector3 areaRight = Vector3.right;
    [SerializeField, ReadOnly]
    private bool obstacleDetected;
    [SerializeField, ReadOnly]
    private bool onFloor = false;
    [SerializeField, ReadOnly]
    private bool isEating = false;
    [SerializeField, ReadOnly]
    private bool inGoal = false;

    // 回収した埃の数
    private int collectedDust = 0;

    // 各行動の経過時間
    private float currentStopTime = 0.0f;
    private float currentEatTime = 0.0f;
    private float currentDamageTime = 0.0f;
    private float currentRunningTime = 0.0f;

    private PlayerControllerForRigidBody player = null;
    private GameManager gameManager = null;

    void Start()
    {
        // プレイヤーとゲーム管理クラスを取得
        player = FindFirstObjectByType<PlayerControllerForRigidBody>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {

        // =======================================================
        // 地面判定
        // =======================================================

        onFloor = Physics.CheckBox(transform.position - transform.up * floorBoxYOffset * transform.localScale.y, Vector3.Scale(transform.localScale, floorBox), transform.rotation, groundLayerMask);

        // =======================================================
        // 持ち上げられている時
        // プレイヤーの向きに合わせる
        // =======================================================

        if (isPickedup)
        {
            if (player)
            {
                direct = player.DirectionX;
                transform.LookAt(transform.position + areaRight * direct);
            }
        }

        // =======================================================
        // 通常稼働中の行動
        // 地面にいて、持ち上げられておらず、稼働中の時のみ動く
        // =======================================================

        if (onFloor && !isPickedup && inWork)
        {
            // 障害物判定
            RaycastHit hit;
            Ray obstacleRay = new Ray(transform.position, obstaclesDetect.position - transform.position);
            if (Physics.Raycast(obstacleRay, out hit, Vector3.Distance(obstaclesDetect.position, transform.position), obstaclesLayerMask))
            {
                obstacleDetected = true;
            }
            else
            {
                obstacleDetected = false;
            }

            // 口元にある埃を検出
            Collider[] eatingColliders = Physics.OverlapSphere(dustDetect.position, dustDetectRadius * transform.localScale.magnitude, dustLayerMask);
            if(eatingColliders.Length > 0)
            {
                isEating = true;
            }

            // ===================================================
            // ダメージ後の逃走中
            // ===================================================
            if (currentRunningTime > 0.0f)
            {
                transform.position += transform.forward * speed * runningMagnification * Time.deltaTime;
                currentRunningTime = Mathf.Max(currentRunningTime - Time.deltaTime, 0.0f);
            }

            // ===================================================
            // 埃を食べている途中
            // 一定時間経過後に埃を回収
            // ===================================================
            else if (isEating)
            {
                currentEatTime += Time.deltaTime;
                if (currentEatTime > eatTime)
                {
                    for (int i = 0; i < eatingColliders.Length; i++)
                    {
                        collectedDust++;
                        gameManager.AddDust(1);
                        Destroy(eatingColliders[i].gameObject);
                    }
                    isEating = false;
                    currentEatTime = 0.0f;
                }
            }

            // ===================================================
            // 障害物に当たった場合
            // 一定時間停止してから方向転換
            // ===================================================
            else if (obstacleDetected)
            {
                currentStopTime += Time.deltaTime;
                if (currentStopTime > stopTime)
                {
                    TrunAway();
                }
            }

            // ===================================================
            // 通常移動
            // ===================================================
            else
            {
                transform.position += transform.forward * speed * Time.deltaTime;
                currentStopTime = 0.0f;
            }
        }

        // =======================================================
        // ダメージ後の無敵・点滅演出
        // =======================================================
        if (currentDamageTime > 0.0f)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Abs(Mathf.Sin(Time.time * 10));
            spriteRenderer.color = color;
            currentDamageTime = Mathf.Max(currentDamageTime - Time.deltaTime, 0.0f);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void TrunAway()
    {
        // 進行方向を反転
        direct = -direct;

        // areaRight基準で向きを変える
        transform.LookAt(transform.position + areaRight * direct);

        // 停止時間をリセット
        currentStopTime = 0.0f;
    }

    public void SetMaxHP(int value)
    {
        maxHP = value;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public void SetHP(int value) {
        currentHP = value;
    }

    public int GetHP() { return currentHP; }
    public void SetPickup(bool value)
    {
        isPickedup = value;
    }

    public bool IsInWork()
    {
        return inWork;
    }

    public bool IsInGoal()
    { 
        return inGoal; 
    }

    public void SetInWork(bool value)
    {
        inWork = value;
    }

    public bool IsPickedup { get {  return isPickedup; } }

    private void OnCollisionEnter(Collision collision)
    {
        // 無敵時間中でなければ敵からダメージを受ける
        if (currentDamageTime == 0 && collision.gameObject.CompareTag(enemyTag))
        {
            currentHP--;

            // 敵から逃げるため方向転換
            TrunAway();

            // 無敵時間と逃走時間を開始
            currentDamageTime = damageTime;
            currentRunningTime = runningTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ゴールに到達したら稼働停止
        if (other.CompareTag("Goal"))
        {
            inWork = false;
            inGoal = true;
        }
    }

    void IRaiseAction.Raise()
    {
        // 持ち上げ状態にする
        isPickedup = true;

        // 持ち上げ中はダメージ点滅を解除
        currentDamageTime = 0;
    }

    void IRaiseAction.Drop()
    {
        // 持ち上げ解除
        isPickedup = false;
    }

    public void SetAreaDirect(Vector3 v)
    {
        // エリアの右方向を設定
        areaRight = v;

        // 現在の進行方向に合わせて向きを更新
        transform.LookAt(transform.position + areaRight * direct);
    }

    public bool CanRaise()
    {
        // 常に持ち上げ可能
        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 障害物検出Ray
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, obstaclesDetect.position);

        // 埃検出範囲
        Gizmos.DrawWireSphere(dustDetect.position, dustDetectRadius * transform.localScale.magnitude);

        // 地面判定Box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position - transform.up * floorBoxYOffset * transform.localScale.y, Vector3.Scale(transform.localScale, floorBox));
    }

    
#endif
}
