using Unity.VisualScripting;
using UnityEngine;
using static CameraInformation;

public class CameraAreaCollision : MonoBehaviour
{
    //所属するカメラエリア
    private CameraInformation cameraInformation = null;

    //対応するカメラ
    private AreaCamera areaCamera = null;

    private GameObject player;

    private AreaRumba areaRumba = null;

    private GameObject Rumba = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CameraManager cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if (cameraManager == null)
        {
            Debug.Log("カメラマネージャーがありません");
        }

        areaCamera = cameraManager.AreaCamera.GetComponent<AreaCamera>();
        if(areaCamera == null)
        {
            Debug.Log("エリアにカメラがありません");
        }

        cameraInformation = GetComponentInParent<CameraInformation>();
        if(cameraInformation == null)
        {
            Debug.Log("カメラ情報がありません");
        }

        player=cameraManager.TargetPlayer;
        Rumba = cameraManager.TargetRumba;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーがこのエリアに触れたら、次の移動先としてセットする
        //if (other.CompareTag(areaCamera.TargetTag))
        if (other.gameObject == player)
        {
            Debug.Log("Playerが範囲内に入りました:" + cameraInformation.gameObject.name);
            areaCamera.SetArea(cameraInformation);
        }

        //int layerindex = other.gameObject.layer;
        //if (LayerMask.LayerToName(layerindex) == "Cleaner")
        //{
        //    areaRumba.SetArea(cameraInformation);
        //}
        if (other.gameObject == Rumba)
        {
            areaRumba.SetArea(cameraInformation);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        //プレイヤーがこのエリアから出たら、移動先のエリアのにカメラを動かす
        //if (other.CompareTag(areaCamera.TargetTag))
        if (other.gameObject == player)
        {
            Debug.Log("Playerが範囲外に出ました");
            areaCamera.MoveArea(cameraInformation);
        }
        //int layerindex = other.gameObject.layer;
        //if (LayerMask.LayerToName(layerindex) == "Cleaner")
        //{
        //    areaRumba.MoveArea(cameraInformation);
        //}
        if (other.gameObject == Rumba)
        {
            areaRumba.MoveArea(cameraInformation);
        }
    }

}
