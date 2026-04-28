using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways] // エディタ上でもUpdateが動く
public class Rope : MonoBehaviour, IRaiseAction
{
    // =======================================================
    // 設定
    // =======================================================

    [SerializeField]
    private List<GameObject> anchors = new List<GameObject>();
    [SerializeField]
    private float maxAmount = 1.0f;
    [SerializeField, ReadOnly]
    private float amount = 0.0f;
    [SerializeField]
    private UnityEvent<float> updateAmount;
    [SerializeField]
    private UnityEvent<float> updateT;

    // =======================================================
    // 内部状態
    // =======================================================

    private float startDistance = 0;
    private float distance = 0;

    private LineRenderer lineRenderer;

    void Start()
    {
        // LineRenderer取得
        lineRenderer = GetComponent<LineRenderer>();

        // 初期距離（始点～最初のアンカー）
        startDistance = Vector3.Distance(transform.position, anchors[0].transform.position);
        distance = startDistance;

        // ===================================================
        // ロープ描画初期化
        // ===================================================
        if (lineRenderer)
        {
            // 始点 + アンカー分
            lineRenderer.positionCount = anchors.Count + 1;

            // 始点
            lineRenderer.SetPosition(0, transform.position);

            // 各アンカー位置を設定
            for (int i = 0; i < anchors.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, anchors[i].transform.position);
            }
        }
    }

    void Update()
    {
        // ===================================================
        // 距離計算（伸び量の更新）
        // ===================================================

        distance = Vector3.Distance(transform.position, anchors[0].transform.position);

        // 初期距離との差分を伸び量とする
        // ※一度伸びたら縮まないように下限をamountにしている
        amount = Mathf.Clamp(distance - startDistance, amount, maxAmount);

        // ===================================================
        // ロープ描画更新
        // ===================================================

        if (lineRenderer)
        {
            // 始点更新
            lineRenderer.SetPosition(0, transform.position);

            // アンカー位置更新
            for (int i = 0; i < anchors.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, anchors[i].transform.position);
            }
        }

        // ===================================================
        // 外部通知
        // ===================================================

        // 生の伸び量
        if (updateAmount != null)
        {
            updateAmount.Invoke(amount);
        }

        // 0～1に正規化した値
        if (updateT != null)
        { 
            updateT.Invoke(amount / maxAmount); 
        }
    }

    public void Drop()
    {

    }

    public void Raise()
    {

    }

    public bool CanRaise()
    {
        // 最大まで伸びていなければ操作可能
        return amount < maxAmount;
    }
}
