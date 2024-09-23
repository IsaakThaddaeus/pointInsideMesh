# Point Inside Mesh and Closest Point Calculation in Unity

This Unity C# script provides efficient methods to determine whether a point is located inside a triangular mesh and to compute the closest point on the mesh's surface from an internal point. Additionally, the **Möller-Trumbore algorithm** is implemented to check ray-triangle intersections.

## Features

- **Check if a Point is Inside a Mesh**
  - Uses a ray-casting technique to determine if a point is inside a mesh.
  - An optimized version (`isPointInsideMeshFast`) is provided, which significantly improves performance by reducing unnecessary triangle checks.

- **Find Closest Point on Mesh**
  - Computes the closest point on the surface of the mesh from a point located inside the mesh.

- **Ray-Triangle Intersection (Möller-Trumbore Algorithm)**
  - Efficiently checks if a ray intersects with a triangle, which is used in both point-inside-mesh detection and closest-point calculation.

## Methods

### `isPointInsideMesh(GameObject meshObject, Vector3 point)`
Checks if a given point is inside the specified mesh by casting a ray and counting the number of intersections.

### `isPointInsideMeshFast(GameObject meshObject, Vector3 point)`
A faster implementation of the point-inside-mesh check, optimized by only considering triangles that could possibly intersect with the ray based on bounding checks.

### `closestPointOnMesh(GameObject meshObject, Vector3 point)`
Calculates the closest point on the surface of the mesh to a point located inside the mesh.

### `IntersectRayTriangle(Vector3 rayOrigin, Vector3 rayDirection, Vector3 v0, Vector3 v1, Vector3 v2, out float t)`
Implements the **Möller-Trumbore** algorithm to determine if a ray intersects with a triangle.

## Usage

1. Add the script to your Unity project.
2. Use `isPointInsideMesh` or `isPointInsideMeshFast` to check if a point is within a triangular mesh.
3. Use `closestPointOnMesh` to find the nearest point on the mesh from a given internal point.
4. The `IntersectRayTriangle` method is also available for custom ray-triangle intersection calculations.

### Example

```csharp
GameObject meshObject = ...; // Your mesh object
Vector3 point = ...;         // The point to check

bool isInside = pointInsideMesh.isPointInsideMesh(meshObject, point);
Vector3 closestPoint = pointInsideMesh.closestPointOnMesh(meshObject, point);
```
