using UnityEngine;

public class TrashCamera : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = target.position + offset;
        }
    }
}
