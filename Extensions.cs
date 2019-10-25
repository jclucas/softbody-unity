using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

    // f(x + dx) = f(x) + f'(x) * dx
    public static Vector3 IntegrateEuler(this Vector3 x, Vector3 dxdt, float dt) {
        return x + dxdt * dt;
    }

    public static Vector3 IntegrateMidpoint(this Vector3 x, Vector3 dxdt, float dt) {
        // var mid = x + dt / 2 * dxdt;
        // return x + dt * (mid - x) / (dt / 2);
        var dx = dxdt * dt;
        return x + (x + dx / 2) * dt;
    }

    public delegate Vector3 Eval(Vector3 x, float dt);

    public static Vector3 Integrate(this Vector3 x, Eval dxdt, float dt) {
        var k1 = dxdt(x, dt);
        var k2 = dxdt(x + k1, dt / 2);
        var k3 = dxdt(x + k2, dt / 2);
        var k4 = dxdt(x + k3, dt);
        return x + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
    }

    public static Mesh WeldVertices(this Mesh mesh) { //}, float r) {

        var vertices = new List<Vector3>(mesh.vertices);
        var toRemove = new List<int>();
        var triangles = mesh.triangles;

        for (var i = 0; i < mesh.vertexCount; i++) {

            for (var j = i + 1; j < mesh.vertexCount; j++) {
                
                if (vertices[i].x == vertices[j].x && vertices[i].y == vertices[j].y && vertices[i].z == vertices[j].z) {

                    if (!toRemove.Contains(j)) {
                        toRemove.Add(j);
                    }

                    Debug.Log("Duplicate: " + i + ", " + j);

                    for (var k = 0; k < triangles.Length; k++) {
                        if (triangles[k] == j) {
                            triangles[k] = i;
                        }
                    }

                }

            }

        }

        toRemove.Sort();

        for (int i = toRemove.Count - 1; i >= 0; i--) {
            vertices.RemoveAt(toRemove[i] - 1);
        }

        vertices.TrimExcess();

        mesh.triangles = triangles;
        mesh.vertices = vertices.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;

    }

}