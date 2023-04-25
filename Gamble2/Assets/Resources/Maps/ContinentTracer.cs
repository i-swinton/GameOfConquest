using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContinentTracer : MonoBehaviour
{
    // The positions to be convex hulled
    List<PolygonCollider2D> colliders;

    LineRenderer tracer;

    List<Vector2> tracePoints;

    public void CreateTrace(List<PolygonCollider2D> cols)
    {
        // Set the colliders
        colliders = cols;
        // Create the list
        tracePoints = new List<Vector2>();
        // Initialize the linerenderer
        tracer = GetComponent<LineRenderer>();

        // Fill out trace points
        FillPoints();

       var hull = Geometry.ConvexHull.ConvexHullPoints(tracePoints);
    }

    public void FillPoints()
    {
        for(int i=0; i < colliders.Count; ++i)
        {
            // Add the range of collider points
            tracePoints.AddRange(colliders[i].points.ToList());
        }
    }

}
