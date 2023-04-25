using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Geometry
{
    public static class ConvexHull
    {
        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        public static int orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            float val = (q.y - p.y) * (r.x - q.x) -
                    (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0; // collinear
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        public static List<Vector2> ConvexHullPoints(List<Vector2> points)
        {
            // There must be at least 3 points
            if (points.Count < 3) return null;

            // Initialize Result
            List<Vector2> hull = new List<Vector2>();

            // Find the leftmost point
            int l = 0;
            for (int i = 1; i < points.Count; i++)
                if (points[i].x < points[l].x)
                    l = i;

            // Start from leftmost point, keep moving 
            // counterclockwise until reach the start point
            // again. This loop runs O(h) times where h is
            // number of points in result or output.
            int p = l, q;
            do
            {
                // Add current point to result
                hull.Add(points[p]);

                // Search for a point 'q' such that 
                // orientation(p, q, x) is counterclockwise 
                // for all points 'x'. The idea is to keep 
                // track of last visited most counterclock-
                // wise point in q. If any point 'i' is more 
                // counterclock-wise than q, then update q.
                q = (p + 1) % points.Count;

                for (int i = 0; i < points.Count; i++)
                {
                    // If i is more counterclockwise than 
                    // current q, then update q
                    if (orientation(points[p], points[i], points[q])
                                                        == 2)
                        q = i;
                }

                // Now q is the most counterclockwise with
                // respect to p. Set p as q for next iteration, 
                // so that q is added to result 'hull'
                p = q;

            } while (p != l); // While we don't come to first 
                              // point

            return hull;
        }
    }
}

