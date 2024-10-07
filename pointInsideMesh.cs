using UnityEngine;

public static class pointInsideMesh
{
    // Checks if a point is inside a mesh
    public static bool isPointInsideMesh(GameObject meshObject, Vector3 point)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int numberOfIntersections = 0;

        Vector3 rayDirection = new Vector3(1f, 0.1f, 0.3f);
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


    // Checks if a point is inside a mesh more efficiently.
    // This implementation is approximately twice as fast as the previous one.
    // To further optimize, consider adding a bounding box check to quickly eliminate triangles
    // that are outside the relevant region, reducing unnecessary intersection tests.
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


    // Computes the closest point on the surface of the mesh from a point located inside the mesh 
    public static Vector3 closestPointOnMesh(GameObject meshObject, Vector3 point, out Vector3 closestNormal)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Matrix4x4 localToWorld = meshObject.transform.localToWorldMatrix;

        Vector3 closestPoint = Vector3.zero;
        closestNormal = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            v0 = localToWorld.MultiplyPoint3x4(v0);
            v1 = localToWorld.MultiplyPoint3x4(v1);
            v2 = localToWorld.MultiplyPoint3x4(v2);


            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;

            Vector3 rayDirection = Vector3.Cross(edge1, edge2);

            if (IntersectRayTriangle(point, rayDirection, v0, v1, v2, out float t))
            {
                if(closestPoint == Vector3.zero || Vector3.Distance(point, point + rayDirection * t) < Vector3.Distance(point, closestPoint)){
                    closestPoint = point + rayDirection * t;
                    closestNormal = rayDirection.normalized;
                }
            }

        }

        return closestPoint;
    }

    // Computes the closest point on the surface of the mesh
    public static Vector3 closestPointOnMeshBoth(GameObject meshObject, Vector3 point, out Vector3 closestNormal)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Matrix4x4 localToWorld = meshObject.transform.localToWorldMatrix;

        Vector3 closestPoint = Vector3.zero;
        closestNormal = Vector3.zero;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            v0 = localToWorld.MultiplyPoint3x4(v0);
            v1 = localToWorld.MultiplyPoint3x4(v1);
            v2 = localToWorld.MultiplyPoint3x4(v2);


            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;

            Vector3 rayDirection = Vector3.Cross(edge1, edge2);
            float t;

            if (IntersectRayTriangle(point, rayDirection, v0, v1, v2, out t))
            {
                if (closestPoint == Vector3.zero || Vector3.Distance(point, point + rayDirection * t) < Vector3.Distance(point, closestPoint))
                {
                    closestPoint = point + rayDirection * t;
                    closestNormal = rayDirection.normalized;
                }
            }


            if (IntersectRayTriangle(point, -rayDirection, v0, v1, v2, out t))
            {
                if (closestPoint == Vector3.zero || Vector3.Distance(point, point + rayDirection * t) < Vector3.Distance(point, closestPoint))
                {
                    closestPoint = point - rayDirection * t;
                    closestNormal = rayDirection.normalized;
                }
            }
        }

        return closestPoint;
    }


    // Executes the Möller-Trumbore algorithm to determine if a ray intersects with a given triangle.
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
