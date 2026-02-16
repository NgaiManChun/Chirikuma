using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IRaiseAction
{
    public bool CanRaise();
    public void Raise();
    public void Drop();
}


public class PlayerRaise : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField, Tooltip("動いてる時の消費倍率")] private float m_moveConsumptionRate = 1.5f;

    [Header("Component")]
    [SerializeField] private PlayerControllerForRigidBody m_player;
    [SerializeField] private Transform m_raisePosition;

    [Header("InGame")]
    [SerializeField, ReadOnly] private List<GameObject> m_raiseTargetList = new List<GameObject>();
    [SerializeField, ReadOnly] private GameObject m_raiseObject;

    private GameObject m_extraCollider;

    public bool isRaise => m_raiseObject != null;

    Vector3 m_initPos;

    private void Start()
    {
        m_initPos = transform.localPosition;
    }

    private void Update()
    {
        // Exitせず消失したターゲットオブジェクトを消去
        m_raiseTargetList.RemoveAll(b => b == null);

        Vector3 pos = m_initPos;
        pos.x *= m_player.DirectionX;

        transform.localPosition = pos;

        if (m_raiseObject != null)
        {
            IRaiseAction action = m_raiseObject.GetComponent<IRaiseAction>();

            if (m_player.CurrentStamina > 0.0f && action.CanRaise())
            {
                float rate = 1.0f;

                if (Mathf.Abs(m_player.DirectionLocal.x) != 0.0f)
                    rate = m_moveConsumptionRate;

                m_player.SetStamina(m_player.CurrentStamina - Time.deltaTime * rate);
            }
            else
            {
                Drop();
            }
        }
    }

    private void Raise()
    {
        if (m_raiseTargetList.Count == 0)
        {
            Debug.LogError($"持ち上げる対象が登録されていないか、操作が無効化されています。");
            return;
        }

        Vector3 pos = this.transform.position;
        m_raiseTargetList.Sort((GameObject a, GameObject b) => {
            return Vector3.Distance(pos, a.transform.position).CompareTo(Vector3.Distance(pos, b.transform.position));
        });

        foreach(GameObject gameObject in m_raiseTargetList)
        {
            IRaiseAction action = gameObject.GetComponent<IRaiseAction>();
            if(action != null && action.CanRaise())
            {
                m_raiseObject = gameObject;
                m_raiseTargetList.Remove(m_raiseObject);
                action.Raise();
                break;
            }
        }

        if (m_raiseObject == null) return;

        m_raiseObject.transform.parent = this.transform;
        m_raiseObject.transform.localPosition = Vector3.zero;

        Rigidbody rigid = m_raiseObject.GetComponent<Rigidbody>();
        if (rigid != null) rigid.isKinematic = true;

        Collider collider = m_raiseObject.GetComponent<Collider>();

        // 持つものが壁にめり込まないようにColliderだけのコピーを追加
        m_extraCollider = Instantiate(m_raiseObject, this.transform);
        // Collider だけを残す。他のコンポーネントは削除
        foreach (var comp in m_extraCollider.GetComponentsInChildren<Component>())
        {
            if (comp is Transform || comp is Collider) continue;
            DestroyImmediate(comp);
        }

        if (collider != null) collider.enabled = false;
    }

    public void Drop()
    {
        m_raiseObject.transform.parent = null;

        var com = m_raiseObject.GetComponent<IRaiseAction>();
        if (com != null)
            com.Drop();

        Rigidbody rigid = m_raiseObject.GetComponent<Rigidbody>();
        if (rigid != null) rigid.isKinematic = false;

        Collider collider = m_raiseObject.GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        Destroy(m_extraCollider);

        m_raiseObject = null;
    }

    public void Invoke()
    {
        if (m_raiseObject == null)
        {
            if (m_player.CurrentStamina > 0.0f)
            {
                Raise();
            }
        }
        else
        {
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IRaiseAction>() != null)
        {
            m_raiseTargetList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IRaiseAction>() != null)
        {
            m_raiseTargetList.Remove(other.gameObject);
        }
    }
}
