using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Rope : MonoBehaviour, IRaiseAction
{
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
    //[SerializeField]
    //private Transform p0;
    //[SerializeField]
    //private Transform p1;
    //[SerializeField] 
    //private GameObject target;

    private float startDistance = 0;
    private float distance = 0;

    private float EndDistance = 0;
    private Vector3 targetStartPosition;
    private LineRenderer lineRenderer;

    //[SerializeField]
    //private Vector3 force = Vector3.zero;
    //[SerializeField]
    //private Transform targetPosition;
    //[SerializeField]
    //private int remain = 40;

    //private Rigidbody rb;
    //public ConfigurableJoint joint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        startDistance = Vector3.Distance(transform.position, anchors[0].transform.position);
        distance = startDistance;
        if (lineRenderer)
        {
            lineRenderer.positionCount = anchors.Count + 1;
            lineRenderer.SetPosition(0, transform.position);
            for (int i = 0; i < anchors.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, anchors[i].transform.position);
            }
        }
        

        //EndDistance = Vector3.Distance(target.transform.position, p1.position);
        //targetStartPosition = target.transform.position;


        //rb = GetComponent<Rigidbody>();
        //joint = GetComponent<ConfigurableJoint>();
        //remain--;
        //if (remain > 0)
        //{
        //    GameObject next = Instantiate(gameObject);
        //    next.transform.position = transform.position - transform.up;
        //    next.GetComponent<Rope>().joint.connectedBody = rb;
        //}
        //else {
        //    transform.position = targetPosition.position;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, anchors[0].transform.position);
        //amount = distance - startDistance;
        //amount = Mathf.Max(distance - startDistance, amount);
        //amount = Mathf.Min(amount, maxAmount);
        amount = Mathf.Clamp(distance - startDistance, amount, maxAmount);
        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, transform.position);
            for (int i = 0; i < anchors.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, anchors[i].transform.position);
            }
        }
        if (updateAmount != null)
        {
            updateAmount.Invoke(amount);
        }
        if(updateT != null)
        { 
            updateT.Invoke(amount / maxAmount); 
        }
        
        //float distance = Vector3.Distance(transform.position, p0.position);
        //if (distance > StartDistance && distance - StartDistance < EndDistance)
        //{
        //    target.transform.position = targetStartPosition + (p1.position - targetStartPosition).normalized * (distance - StartDistance);
        //}
        //if (lineRenderer)
        //{
        //    lineRenderer.SetPosition(0, transform.position);
        //    lineRenderer.SetPosition(2, target.transform.position);
        //}
        //if (remain == 0)
        //    rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void Drop()
    {
        //throw new System.NotImplementedException();
    }

    public void Raise()
    {
        //throw new System.NotImplementedException();
    }

    public bool CanRaise()
    {
        return amount < maxAmount;
    }
}
