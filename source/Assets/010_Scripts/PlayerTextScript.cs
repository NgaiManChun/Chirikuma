using UnityEngine;

public class PlayerTextScript : MonoBehaviour
{
    public AreaCamera Camera;
    [Header("フレーム枠")]
    public GameObject CloudFrame;
    [Header("ボタン")]
    public GameObject Key;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //フレーム枠と操作ボタン等のアクティブをfalseに
        CloudFrame.SetActive(false);
        Key.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //カメラ切替可能になったらフレーム枠等を表示
        if (Camera.VerticalInfo() == true)
        {
            CloudFrame.SetActive(true);
            Key.SetActive(true);
            
        }
        else
        {
            CloudFrame.SetActive(false);
            Key.SetActive(false);

        }

        //if (Input.GetKey(KeyCode.K))
        //{
        //    CloudFrame.SetActive(true);
        //}
        //else
        //{
        //    CloudFrame.SetActive(false);
        //}

    }
}
