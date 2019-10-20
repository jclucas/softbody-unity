using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeList {

    private Dictionary<int, float>[] neighbors;

    private Mesh mesh;

    public EdgeList(Mesh mesh) {

        this.mesh = mesh;
        neighbors = new Dictionary<int, float>[mesh.vertexCount];

        for (var i = 0; i < mesh.vertexCount; i++) {
            neighbors[i] = new Dictionary<int, float>();
        }

        // add edges from each triangle
        for (var i = 0; i < mesh.triangles.Length; i += 3) {

            var a = mesh.triangles[i];
            var b = mesh.triangles[i + 1];
            var c = mesh.triangles[i + 2];

            AddEdge(a, b);
            AddEdge(a, c);
            AddEdge(b, a);
            AddEdge(b, c);
            AddEdge(c, a);
            AddEdge(c, b);

        }

    }

    private void AddEdge(int a, int b) {
        if (!neighbors[a].ContainsKey(b)) {
            neighbors[a].Add(b, Vector3.Distance(mesh.vertices[b], mesh.vertices[a]));
        }
    }

    public int[] GetNeighbors(int v) {
        int[] n = new int[neighbors[v].Keys.Count];
        neighbors[v].Keys.CopyTo(n, 0);
        return n;
    }

    public Vector3 GetDisplacement(int a, int b) {
        var actual = mesh.vertices[b] - mesh.vertices[a];
        return Vector3.Normalize(actual) * (Vector3.Magnitude(actual) - neighbors[a][b]);
    }

}