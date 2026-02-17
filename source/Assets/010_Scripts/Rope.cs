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

    private float startDistance = 0;
    private float distance = 0;

    private float EndDistance = 0;
    private Vector3 targetStartPosition;
    private LineRenderer lineRenderer;

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
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, anchors[0].transform.position);
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
    }

    public void Drop()
    {

    }

    public void Raise()
    {

    }

    public bool CanRaise()
    {
        return amount < maxAmount;
    }
}
