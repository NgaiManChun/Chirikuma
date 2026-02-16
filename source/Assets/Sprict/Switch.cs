using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Switch : MonoBehaviour
{
    float DefaultPosY;
    float BottomY;
    [SerializeField, Header("本体")]
    private GameObject button;
    [Header("スイッチが動くスピード")]
    public float Speed = 0.5f;
    [Header("スイッチが反応するもの(Tag検索)")]
    public string Tag = "Player";
    [Header("trueにすると１度だけ反応")]
    public bool isOnetime;
    public bool isActive;

    [Header("動作するオブジェクトを持ってきてSetMoveを選ぶ")]
    public UnityEvent MoveON;
    public UnityEvent MoveOFF;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultPosY = button.transform.position.y;
        BottomY = DefaultPosY - 0.3f;
    }

    // Update is called once per frame
    void Update()
    {

        if (isActive && button.transform.position.y > BottomY)
        {
            button.transform.position -= Vector3.up * Speed * Time.deltaTime;

            if (button.transform.position.y <= BottomY)
            {
                
                    
                MoveON.Invoke();
                //MoveDoor.isOpen = true;


                if(isOnetime == true)
                {
                    enabled = false;
                }
                
            }
        }
        else if (!isActive && button.transform.position.y < DefaultPosY)
        {
            button.transform.position += Vector3.up * Speed * Time.deltaTime;
            
           
            MoveOFF.Invoke();
                //MoveDoor.isOpen = false;
            
            
        }
    }

    
    
    private void OnTriggerStay(Collider other)
    {
        if (!isActive && other.CompareTag(Tag))
        {
            
                isActive = true;
            
            
        }

    }
    
    
    private void OnTriggerExit()
    {
        if (isActive)
        {
            
                isActive = false;
            
            
        };
    }
}


