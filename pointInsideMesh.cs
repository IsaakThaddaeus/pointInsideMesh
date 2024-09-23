using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class pointInsideMesh
{
    public static bool isPointInsideMesh(GameObject meshObject, Vector3 point)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int numberOfIntersections = 0;

        Vector3 rayDirection = new Vector3(1f, 0f, 0f);
        Matrix4x4 localToWorld = meshObject.transform.localToWorldMatrix;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            v0 = localToWorld.MultiplyPoint3x4(v0);
            v1 = localToWorld.MultiplyPoint3x4(v1);
            v2 = localToWorld.MultiplyPoint3x4(v2);

            if (IntersectRayTriangle(point, rayDirection, v0, v1, v2, out float t))
            {
                numberOfIntersections++;
            }
        }

        return numberOfIntersections % 2 != 0;
    }

    //Seems to be twice as fast as the above, bounding box check should be included
    public static bool isPointInsideMeshFast(GameObject meshObject, Vector3 point)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int numberOfIntersections = 0;

        Vector3 rayDirection = new Vector3(1f, 0f, 0f);
        Matrix4x4 localToWorld = meshObject.transform.localToWorldMatrix;

        Vector3[] transformedVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            transformedVertices[i] = localToWorld.MultiplyPoint3x4(vertices[i]);
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transformedVertices[triangles[i]];
            Vector3 v1 = transformedVertices[triangles[i + 1]];
            Vector3 v2 = transformedVertices[triangles[i + 2]];

            if (v0.x < point.x && v1.x < point.x && v2.x < point.x){
                continue;
            }

            bool hasPointRight = (v0.z > point.z || v1.z > point.z || v2.z > point.z);
            bool hasPointLeft = (v0.z < point.z || v1.z < point.z || v2.z < point.z);

            if (!(hasPointRight && hasPointLeft)){
                continue;
            }

            bool hasPointAbove = (v0.y > point.y || v1.y > point.y || v2.y > point.y);
            bool hasPointBelow = (v0.y < point.y || v1.y < point.y || v2.y < point.y);

            if (!(hasPointAbove && hasPointBelow)){
                continue;
            }

            if (IntersectRayTriangle(point, rayDirection, v0, v1, v2, out float t))
            {
                numberOfIntersections++;
            }
        }

        return numberOfIntersections % 2 != 0;
    }
    public static bool IntersectRayTriangle(Vector3 rayOrigin, Vector3 rayDirection, Vector3 v0, Vector3 v1, Vector3 v2, out float t)
    {
        t = 0;

        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;

        Vector3 pVec = Vector3.Cross(rayDirection, edge2);
        float det = Vector3.Dot(edge1, pVec);

        if (det > -float.Epsilon && det < float.Epsilon)
        {
            return false;
        }

        float invDet = 1.0f / det;


        Vector3 tVec = rayOrigin - v0;
        float u = Vector3.Dot(tVec, pVec) * invDet;

        if (u < 0 || u > 1)
        {
            return false;
        }

        Vector3 qVec = Vector3.Cross(tVec, edge1);
        float v = Vector3.Dot(rayDirection, qVec) * invDet;

        if (v < 0 || u + v > 1)
        {
            return false;
        }

        t = Vector3.Dot(edge2, qVec) * invDet;

        return t > 0;
    }

}
