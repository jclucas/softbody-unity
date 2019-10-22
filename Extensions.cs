using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

    // f(x + dx) = f(x) + f'(x) * dx
    public static Vector3 IntegrateEuler(this Vector3 value, Vector3 deriv, float step) {
        return value + deriv * step;
    }

    public static Vector3 IntegrateMidpoint(this Vector3 value, Vector3 deriv, float step) {
        var mid = value + step / 2 * deriv;
        return value + step * (mid - value) / (step / 2);
    }

    // private Vector3 Integrate() {
    // }

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