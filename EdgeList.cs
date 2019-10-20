using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeList {

    private Dictionary<int, Vector3>[] neighbors;

    public EdgeList(Mesh mesh) {

        neighbors = new Dictionary<int, Vector3>[mesh.vertexCount];

        for (var i = 0; i < mesh.vertexCount; i++) {
            neighbors[i] = new Dictionary<int, Vector3>();
        }

        // add edges from each triangle
        for (var i = 0; i < mesh.triangles.Length; i += 3) {

            var a = mesh.triangles[i];
            var b = mesh.triangles[i + 1];
            var c = mesh.triangles[i + 2];

            AddEdge(a, b, ref mesh);
            AddEdge(a, c, ref mesh);
            AddEdge(b, a, ref mesh);
            AddEdge(b, c, ref mesh);
            AddEdge(c, a, ref mesh);
            AddEdge(c, b, ref mesh);

        }

    }

    private void AddEdge(int a, int b, ref Mesh mesh) {
        neighbors[a].Add(b, mesh.vertices[b] - mesh.vertices[a]);
    }

}