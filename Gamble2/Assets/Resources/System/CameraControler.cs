using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{

    [Header("Zoom Settings")]
    public float MaxHeight;
    public float MinHeight;
    [Tooltip("Speed camera zooms. Negative inverts")]
    public float ZoomSpeed;

    [SerializeField] Camera TileUICam;
    [SerializeField] float MaskDistanceThreshold;
    [SerializeField] LayerMask CloseMask;
    [SerializeField] LayerMask DistanceMask;


    [Header("Pan Settings")]
    [Tooltip("Speed the camera can pan. Negative inverts")]
    public float PanSpeed;

    Vector3 PreMosPos;

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        ZoomCamera();
    }

    void PanCamera()
    {

        //If pan starts recenters
        if (Input.GetKeyDown(KeyCode.Mouse0))
            PreMosPos = Input.mousePosition;


        //Moves camera about
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mouseDelta = PreMosPos - Input.mousePosition;

            transform.position += new Vector3(mouseDelta.x * PanSpeed, mouseDelta.y * PanSpeed) * Time.deltaTime;

            PreMosPos = Input.mousePosition;
        }

    }

    void ZoomCamera()
    {
        transform.position += Vector3.forward * Input.mouseScrollDelta.y * ZoomSpeed;

        if (transform.position.z > MaxHeight)
            transform.position = new Vector3(transform.position.x, transform.position.y, MaxHeight);
        else if (transform.position.z < MinHeight)
            transform.position = new Vector3(transform.position.x, transform.position.y, MinHeight);


        //Turns on and off tile names based off distance.
        if (transform.position.z >= MaskDistanceThreshold)
            TileUICam.cullingMask = CloseMask;
        else
            TileUICam.cullingMask = DistanceMask;

    }
}
