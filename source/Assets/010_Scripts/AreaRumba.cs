using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.ShaderGraph;
using UnityEngine;
using static CameraInformation;

public class AreaRumba : MonoBehaviour
{
    [Header("ルンバ")]
    [SerializeField] GameObject Rumba;

    [Header("現在のエリア")]
    [SerializeField] public CameraInformation cameraInformation;

    //次のカメラエリア
    [SerializeField] private CameraInformation nextCameraInformation = null;

    [SerializeField] private CameraInformation horizonInfo = null;    

    void Start()
    {
        Rumba = GetComponent<CameraManager>().TargetRumba;
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    public void SetArea(CameraInformation info)
    {
        //現在カメラが空っぽなら
        if (cameraInformation == null)
        {
            //今のカメラエリアをセットして即リターン
            cameraInformation = info;
            return;
        }

        //現在カメラと同じなら即リターン
        if (info == cameraInformation) { return; }

        //現在カメラと同じ方向を向くカメラエリアなら(水平移動先のエリアなら)
        if (info.direction == cameraInformation.direction)
        {
            //次の移動先候補としてセット
            //if (horizonInfo == null)
            //{
                horizonInfo = info;
           // }
        }
    }


    // ルンバの水平エリアへの移動
    public void MoveArea(CameraInformation info)
    {
        //水平移動先が設定されていないなら即リターン
        if (horizonInfo == null) { return; }

        //今出たエリアが現在のエリアとおなじなら
        if (info == cameraInformation)
        {
            //所属エリアをセットしていた水平移動先に変える
            cameraInformation = horizonInfo;
            //水平移動先は空っぽにする
            horizonInfo = null;
        }

    }

    //ルンバの奥移動
    public void MoveAreaInput(CameraInformation info)
    {
        //入力移動先が現在のエリアと同じなら即リターン
        if(cameraInformation == info) { return; } 
        //現在の移動先を入力された情報にセットする
        cameraInformation = info;
    }
   
}
