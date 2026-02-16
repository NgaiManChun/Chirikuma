using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveForward;
    Vector3 jumpForward;
    public float speed = 150.0f;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 6.5f;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        moveForward = new Vector3(0f, 0f, 0f);
        jumpForward = new Vector3(0f, 1.0f, 0f);


    }

    void Update()
    {



        if (Input.GetKey(rightKey))
        {
            moveForward.x = 1;
            //transform.eulerAngles = new Vector3(0,90,0);
        }
        else if (Input.GetKey(leftKey))
        {
            moveForward.x = -1;
            //transform.eulerAngles = new Vector3(0,-90,0);
        }
        else
        {
            moveForward.x = 0;

        }

        

        if (Input.GetKeyUp(rightKey) || Input.GetKeyUp(leftKey))
        {
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 center = transform.position + Vector3.up * 2.5f;
        float radius = 0.45f;
        LayerMask layer = LayerMask.GetMask("Ground");
        bool isGround = Physics.CheckSphere(center, radius, layer);

        if (Input.GetKeyDown(jumpKey))
        {
            rb.AddForce(jumpForward * jumpPower, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1,0,1)).normalized;
        //Vector3 moveFoeward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;
        //rb.velocity = moveForward * speed + new Vector3(0,rb.velocity.y,0);

        //if (moveForward != Vector3.zero)
        //{
        //    transform.rotation = Quaternion.LookRotation(moveForward);
        //}

        rb.AddForce(moveForward * speed);
    }


}
