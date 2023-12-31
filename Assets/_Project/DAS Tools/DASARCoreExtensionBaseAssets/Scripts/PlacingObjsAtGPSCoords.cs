using System;
using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using AR_Fukuoka;

/// <summary>
/// This class is an extension of the PlacingObjsAtGPSCoords class, but capable of placing more than one object using device GPS coords.
/// </summary>
public class PlacingObjsAtGPSCoords : MonoBehaviour
{
    //GeospatialAPIを用いたトラッキング情報
    [SerializeField] AREarthManager EarthManager;
    //GeospatialAPIとARCoreの初期化と結果
    [SerializeField] VpsInitializer Initializer;
    //結果表示用のUI 
    [SerializeField] Text OutputText;
    //方位の許容精度(値の変更はInspectorから)
    [SerializeField] double HeadingThreshold = 25;
    //水平位置の許容精度(値の変更はInspectorから)
    [SerializeField] double HorizontalThreshold = 20;

    /*
    //オブジェクトを置く緯度
    [SerializeField] double Latitude;
    //オブジェクトを置く経度
    [SerializeField] double Longitude;
    //オブジェクトを置く高さ[m] (ジオイド高+標高。求め方はAR_Fukuoka/elevation.txtを参照)
    [SerializeField] double Altitude;
    */

    // Current set list of GPS Coordinates to place ContentPrefab.
    [SerializeField] private List<GPSCoords> m_GPSCoords = new List<GPSCoords>();

    //オブジェクトの向き(北=0°)
    [SerializeField] double Heading;
    //表示オブジェクトの元データ
    [SerializeField] GameObject ContentPrefab;
    //実際に表示するオブジェクト
    GameObject displayObject;
    //アンカー作成に使用
    [SerializeField] ARAnchorManager AnchorManager;
    bool initialized = false;


    [Serializable] public class GPSCoords
    {
        public string placeName;
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }

    // Update is called once per frame
    void Update()
    {
        //初期化失敗またはトラッキングができていない場合は何もしないで戻る
        if (!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
        {
            return;
        }
        //トラッキングの状態を表示する際に使用
        string status = "";
        //トラッキング結果を取得
        GeospatialPose pose = EarthManager.CameraGeospatialPose;
        //トラッキング精度がthresholdより悪い(値が大きい)場合
        if (pose.HeadingAccuracy > HeadingThreshold ||
                pose.HorizontalAccuracy > HorizontalThreshold)
        {
            status = "低精度：周辺を見回してください";
        }
        else
        {
            status = "高精度：High Tracking Accuracy";
            if (!initialized)
            {
                initialized = true;
                //表示オブジェクトを実際に登場させる
                //SpawnObject(pose, ContentPrefab);

                for (int i = 0; i < m_GPSCoords.Count; i++)
                {
                    SpawnLoadedObject(pose, ContentPrefab, m_GPSCoords[i]);
                }
            }
        }
        //結果を表示
        ShowTrackingInfo(status, pose);
    }

    //表示オブジェクトを実際に登場させる
    void SpawnLoadedObject(GeospatialPose pose,GameObject prefab, GPSCoords coords)
    {
        //スマホの高さ-1.5mでおよそ地面の高さとする(tentatively)
        //Altitude = pose.Altitude - 1.5f;

        // New: use altitude coords from GPSCoords object.
        coords.Altitude = pose.Altitude - 1.5f;

        //角度の補正
        //Create a rotation quaternion that has the +Z axis pointing in the same direction as the heading value (heading=0 means north direction)
        //https://developers.google.com/ar/develop/unity-arf/geospatial/developer-guide-android#place_a_geospatial_anchor
        Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);

        //指定した位置・向きのアンカーを作成
        //ARGeospatialAnchor anchor = AnchorManager.AddAnchor(Latitude, Longitude, Altitude, quaternion);

        // New: Set the GPSCoords object values in the AddAnchor method.
        ARGeospatialAnchor anchor = AnchorManager.AddAnchor(coords.Latitude, coords.Longitude, coords.Altitude, quaternion);

        //アンカーが正しく作られていればオブジェクトを実体化
        if (anchor != null)
        {
            displayObject = Instantiate(ContentPrefab, anchor.transform);
        }
    }

    void SpawnObject(GeospatialPose pose, GameObject prefab)
    {
        //スマホの高さ-1.5mでおよそ地面の高さとする(tentatively)
        //Altitude = pose.Altitude - 1.5f;

        // New: Save current GPS coordinates for this spot.
        double altitude = pose.Altitude - 1.5f;
        double latitude = pose.Latitude;
        double longitude = pose.Longitude;

        //角度の補正
        //Create a rotation quaternion that has the +Z axis pointing in the same direction as the heading value (heading=0 means north direction)
        //https://developers.google.com/ar/develop/unity-arf/geospatial/developer-guide-android#place_a_geospatial_anchor
        Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);

        //指定した位置・向きのアンカーを作成
        //ARGeospatialAnchor anchor = AnchorManager.AddAnchor(Latitude, Longitude, Altitude, quaternion);

        // New: Set the GPSCoords object values in the AddAnchor method.
        ARGeospatialAnchor anchor = AnchorManager.AddAnchor(latitude, longitude, altitude, quaternion);

        //アンカーが正しく作られていればオブジェクトを実体化
        if (anchor != null)
        {
            displayObject = Instantiate(ContentPrefab, anchor.transform);
        }
    }

    void ShowTrackingInfo(string status, GeospatialPose pose)
    {
        if (OutputText == null) return;
        OutputText.text = string.Format(
            "\n" +
            "Latitude/Longitude: {0}°, {1}°\n" +
            "Horizontal Accuracy: {2}m\n" +
            "Altitude: {3}m\n" +
            "Vertical Accuracy: {4}m\n" +
            "Heading: {5}°\n" +
            "Heading Accuracy: {6}°\n" +
            "{7} \n"
            ,
            pose.Latitude.ToString("F6"),  //{0}
            pose.Longitude.ToString("F6"), //{1}
            pose.HorizontalAccuracy.ToString("F6"), //{2}
            pose.Altitude.ToString("F2"),  //{3}
            pose.VerticalAccuracy.ToString("F2"),  //{4}
            pose.Heading.ToString("F1"),   //{5}
            pose.HeadingAccuracy.ToString("F1"),   //{6}
            status //{7}
        );
    }

    public void SaveGPSCoords()
    {
        //string json = Json
    }

    public void LoadGPSCoords()
    {

    }

    public void ShareGPSCoords()
    {

    }
}