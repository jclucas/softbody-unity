using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    // PARAMETERS

    public float mass = 1;

    public float k = 50;

    public float damping = 1;

    public float e = 0.5f;

    public int subdiv = 0;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public List<Force> forces = new List<Force>();

    // GEOMETRY

    private Mesh mesh;

    private int dim;

    // PARTICLES

    private Particle[] particles;

    private Dictionary<int, Particle> vertexMap = new Dictionary<int, Particle>();

    // the floor
    private Plane floor;

    // Start is called before the first frame update
    void Start() {
        
        // mesh = GetComponent<MeshFilter>().mesh;

        // set static values
        Particle.k = this.k;
        Particle.damping = this.damping;

        // set collision bounds
        floor = new Plane(Vector3.up, transform.InverseTransformPoint(Vector3.zero));

        dim = 2 + subdiv;
        particles = new Particle[dim * dim * dim];
        CreateCube();

        // add global forces
        forces.Add(new Force((p, state, dt) => state[p].mass * gravity, particles));

        // add edges
        forces.Add(new Force((p, state, dt) => {
            Vector3 f = Vector3.zero;
            foreach (var other in state[p].neighbors) {
                var d = GetDisplacement(state[p].position, state[other.Key].position, other.Value);
                f += GetSpringForce(d, state[p].velocity, k, damping);
            }
            return f;
        }, particles));

        // connect all particles for cube
        for (int i = 0; i < particles.Length; i++) {
            for (int j = 0; j < particles.Length; j++) {
                if (i != j) {
                    particles[i].AddEdge(j, ref particles[j]);
                }
            }
        }

    }

    // Update is called once per frame
    internal void Update() {
        
        var vertices = mesh.vertices;

        // apply all forces
        foreach (var f in forces) {
            f.Apply();
        }

        // collision detection
        foreach (var p in particles) {
            if (p.CollidesPlane(floor)) {
                var impulse = p.GetImpulsePlane(Vector3.up, e);
                p.MoveToPlane(floor);
                p.velocity += impulse;
            } 
        }

        // update geometry
        foreach (var p in particles) {
            p.UpdateMesh(ref vertices);
        }

        mesh.vertices = vertices;

    }

    // k: spring constant
    // c: damping constant
    private static Vector3 GetSpringForce(Vector3 d, Vector3 v, float k, float c) {
        return (d * k * -1) - (c * v);
    }

    public Vector3 GetDisplacement(Vector3 a, Vector3 b, float expected) {
        var actual = a - b;
        var displacement = Vector3.Magnitude(actual) - expected;
        return displacement * (actual / Vector3.Magnitude(actual));
    }

    private int GetArrayIndex(int x, int y, int z) {
        return ((x * dim) + y) * dim + z; 
    }

    private void CreateCube() {

        var step = 1f / (subdiv + 1);
        
        // construct dictionary of same points
        var points = new Dictionary<Vector3, List<int>>();

        var vertices = new Vector3[6 * dim * dim];
        var triangles = new int[36 * (dim - 1) * (dim - 1)];
        var uvs = new Vector2[6 * dim * dim];
        var idx = new int[6];

        // generate verts for each face

        for (int i = 0; i < dim; i++) {

            for (int j = 0; j < dim; j++) {

                var u = -0.5f + i * step;
                var v = -0.5f + j * step;

                Vector3[] pts = {
                    new Vector3(0.5f, u, v),    // +x
                    new Vector3(-0.5f, u, v),   // -x
                    new Vector3(u, 0.5f, v),    // +y
                    new Vector3(u, -0.5f, v),   // -y
                    new Vector3(u, v, 0.5f),    // +z
                    new Vector3(u, v, -0.5f)    // -z
                };

                for (int face = 0; face < 6; face++) {
                    idx[face] = face * dim * dim + i * dim + j;
                    uvs[idx[face]] = new Vector2(u, v);
                }

                for (int face = 0; face < 6; face++) {

                    vertices[idx[face]] = pts[face];

                    // add to vector3 -> vertex map
                    if (!points.ContainsKey(pts[face])) {
                        points.Add(pts[face], new List<int>());
                    }

                    points[pts[face]].Add(idx[face]);

                }

            }

        }

        // generate triangles for each face

        var index = new int[6, 4];

        for (int i = 0; i < dim - 1; i++) {

            for (int j = 0; j < dim - 1; j++) {

                // 4 adjacent vertices
                for (int face = 0; face < 6; face++) {
                    index[face, 0] = face * dim * dim + i * dim + j;
                    index[face, 1] = face * dim * dim + i * dim + j + 1;
                    index[face, 2] = face * dim * dim + (i + 1) * dim + j;
                    index[face, 3] = face * dim * dim + (i + 1) * dim + j + 1;
                }

                for (int face = 0; face < 6; face++) {

                    var faceOffset = face * 6 * (dim - 1) * (dim - 1);

                    if (face == 0 || face == 3 || face == 4) {
                        triangles[faceOffset + 6 * (i * (dim - 1) + j)] = index[face, 2];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 1] = index[face, 1];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 2] = index[face, 0];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 3] = index[face, 1];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 4] = index[face, 2];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 5] = index[face, 3];
                    } else {
                        triangles[faceOffset + 6 * (i * (dim - 1) + j)] = index[face, 0];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 1] = index[face, 1];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 2] = index[face, 2];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 3] = index[face, 3];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 4] = index[face, 2];
                        triangles[faceOffset + 6 * (i * (dim - 1) + j) + 5] = index[face, 1];
                    }
                        
                }

            }

        }

        // create mesh
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // attach mesh to game object
        GetComponent<MeshFilter>().mesh = mesh;

        // create particles

        // corners
        Vector3[] corners = {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f)
        };

        for (int i = 0; i < dim; i++) {
            for (int j = 0; j < dim; j++) {
                for (int k = 0; k < dim; k++) {
                    var p = new Vector3(-0.5f + step * i, -0.5f + step * j, -0.5f + step * k);
                    var vertexList = points.ContainsKey(p) ? points[p] : new List<int>();
                    particles[GetArrayIndex(i, j, k)] = new Particle(p, mass, vertexList);
                }
            }
        }

    }

}
