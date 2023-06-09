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
    [SerializeField] LayerMask DistanceMask;
    [SerializeField] List<ZoomThreshold> ZoomThresholds = new List<ZoomThreshold>();


    [Header("Pan Settings")]
    [Tooltip("Speed the camera can pan. Negative inverts")]
    public float PanSpeed;

    Vector3 PreMosPos;

    [Tooltip("The limitations on panning the camera")]
    public Vector2 PanBoundsX;
    public Vector2 PanBoundsY;

    // Update is called once per frame
    void Update()
    {
        // Only allow us to pan when the ui is not blocking
        if (!UIElement.IsBlocking)
        {
            PanCamera();
            ZoomCamera();
        }
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
            Vector3 pos = transform.position;
            
            pos += new Vector3(mouseDelta.x * PanSpeed, mouseDelta.y * PanSpeed) * Time.deltaTime;

            PreMosPos = Input.mousePosition;

            pos.x = Mathf.Clamp(pos.x, PanBoundsX.x, PanBoundsX.y);
            pos.y = Mathf.Clamp(pos.y, PanBoundsY.x, PanBoundsY.y);

            // Clamp the position
            transform.position = pos ;
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
        TileUICam.cullingMask = DistanceMask;
        foreach (ZoomThreshold zoom in ZoomThresholds)
        {
            if (transform.position.z >= zoom.Threshold)
                TileUICam.cullingMask = zoom.Mask;
        }


    }
    [System.Serializable]
    public class ZoomThreshold
    {
        public LayerMask Mask;
        public float Threshold;

        public override bool Equals(object obj)
        {
            var threshold = obj as ZoomThreshold;
            return threshold != null &&
                   Threshold == threshold.Threshold;
        }

        public static bool operator ==(ZoomThreshold t1, ZoomThreshold t2)
        {
            return t1.Threshold == t2.Threshold;
        }
        public static bool operator !=(ZoomThreshold t1, ZoomThreshold t2)
        {
            return t1.Threshold == t2.Threshold;
        }

    }
}
