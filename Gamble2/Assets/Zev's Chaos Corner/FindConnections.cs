using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindConnections : MonoBehaviour
{
    public MapTile Tile;
    public List<Collider2D> Results;
    List<Vector3> points = new List<Vector3>();
    public ContactFilter2D Filter;
    Vector3 Center;
    public bool ShowConnections = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Connect();
        }    
    }

    private void Connect()
    {
        PolygonCollider2D Collider = GetComponent<PolygonCollider2D>();

        Center = Collider.bounds.center;
        Collider.OverlapCollider(Filter, Results);
        foreach (Collider2D item in Results)
        {

            Vector3 point = item.ClosestPoint(Center);


            float angle = Vector3.SignedAngle(point, Center,Vector3.forward);
            angle = Mathf.Rad2Deg* Mathf.Asin((point - Center).normalized.y);
            Debug.Log($"{(point - Center).normalized.y}, {angle}, {item.GetComponent<MapTile>().NameDisplay.text}");

            angle %= 90;
            angle = Mathf.Abs(angle);

            if (angle > 22 && angle < 70)
            { }//    continue;
            else
            {
                points.Add(item.ClosestPoint(Center));
                points.Add(item.bounds.center);
            }
        }
        
    }
    private void OnDrawGizmos()
    {
        if (ShowConnections)
        {

            Gizmos.color = Color.red;
            foreach (Vector3 item in points)
            {
                Gizmos.DrawSphere(item, 0.25f);

            }
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Center, 0.25f);
        }
    }

    public void ToggleShow()
    {
        ShowConnections = !ShowConnections;

        if (ShowConnections)
        {
            Connect();
        }
        else
        {
            points = new List<Vector3>();
        }
    }
}
