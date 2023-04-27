using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class LaunchArcMesh : MonoBehaviour
{
    Mesh mesh;
    public float meshWidth;

    public float velocity;
    public float angle;
    public int resolution;



    public Transform target;

    float g; //force of gravity on the y axis
    float radianAngle;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    private void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
        {
            MakeArcMesh(CalculateArcArray());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeArcMesh(CalculateArcArray());
    }

    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2]; // because every quad is 2 triangles so 6 vertices, and we want to render both sides

        for (int i = 0; i <= resolution; i++)
        {
            //set vertices
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x);

            //set triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 + 6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    //Create an array of Vector 3 positions for the arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        if (target == null)
        {

            radianAngle = Mathf.Deg2Rad * angle;
            float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

            for (int i = 0; i <= resolution; i++)
            {
                float t = (float)i / (float)resolution;
                arcArray[i] = CalculateArcPoint(t, maxDistance);
            }
        }
        else
        {
            float targetDistance = Vector2.Distance( transform.position , target.position);

            radianAngle = Mathf.Deg2Rad * angle;
            velocity = Mathf.Sqrt((g * targetDistance) / Mathf.Sin(2 * radianAngle));
            //float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;
            float maxDistance = targetDistance;

            for (int i = 0; i <= resolution; i++)
            {
                float t = (float)i / (float)resolution;
                arcArray[i] = CalculateArcPoint(t, maxDistance);
            }

            // Rotate to fit the angle
            Vector2 angleVec = target.position-transform.position ;
            float rotAngle = Vector2.SignedAngle(Vector2.up, angleVec);

            transform.localRotation = Quaternion.Euler(0, -rotAngle, 0);
        }

        return arcArray;
    }

    public void SetEndPosition(Vector3 endPos)
    {
        target.position = endPos;    
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);
    }
}