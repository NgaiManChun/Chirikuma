using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    float DefaultPosY;
    [Header("扉が開く高さ")]
    public float OpenPosY = 10f;
    [Header("開くスピード")]
    public float Speed = 10f;

    public bool isOpen;
    public void SetMove(bool value)
    {
        isOpen = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //if (isOpen && transform.position.y < OpenPosY)
        //{
        //    transform.position += Vector3.up * Speed * Time.deltaTime;
        //}
        //else if (!isOpen && transform.position.y > DefaultPosY)
        //{
        //    transform.position -= Vector3.up * Speed * Time.deltaTime;
        //}

        float targetY = isOpen ? OpenPosY : DefaultPosY;
        float direction = Mathf.Sign(targetY - transform.position.y);
        float newY = transform.position.y + direction * Speed * Time.deltaTime;

        // オーバーシュートしないようClamp
        if ((direction > 0 && newY > targetY) || (direction < 0 && newY < targetY))
            newY = targetY;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
