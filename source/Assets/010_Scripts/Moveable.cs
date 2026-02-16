using UnityEngine;

public class Moveable : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float t = 0.0f;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(startPosition, target.position, t);
    }

    public void SetT(float value)
    {
        t = Mathf.Clamp(value, 0.0f, 1.0f);
    }

    public float GetT() { return t; }
}
