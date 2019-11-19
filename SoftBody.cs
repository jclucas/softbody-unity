﻿using System.Collections.Generic;
using UnityEngine;

public class SoftBody : PhysicsObject {

    // PARAMETERS

    public float stiffness = 50;

    public float damping = 1;

    public float shearStiffness = 50;

    public float shearDamping = 1;

    public float bendStiffness = 50;

    public float bendDamping = 1;

    public int subdiv = 0;

    // GEOMETRY

    private Mesh mesh;

    private int dim;

    private float step;

    private Dictionary<Particle, List<int>> vertexMap;

    // PARTICLES

    private ParticleSystem particles;

    // TODO: clarify unity execute order
    protected void Awake() {

        dim = 2 + subdiv;
        step = 1f / (subdiv + 1);
        particles = new ParticleSystem(dim * dim * dim);
        CreateCube();

        // set collision bounds
        particles.floor = new Plane(Vector3.up, transform.InverseTransformPoint(Vector3.zero));

        // add global forces
        particles.AddForce((p, state, dt) => state[p].mass * gravity);

        // add spring forces
        particles.AddForce((p, state, dt) => {
            Vector3 f = Vector3.zero;
            foreach (var other in state[p].structural) {
                var d = GetDisplacement(state[p].position, state[other.Key].position, other.Value);
                var v = GetRelativeVelocity(state[p], state[other.Key]);
                f += GetSpringForce(d, v, stiffness, damping);
            }
            return f;
        });

        particles.AddForce((p, state, dt) => {
            Vector3 f = Vector3.zero;
            foreach (var other in state[p].shear) {
                var d = GetDisplacement(state[p].position, state[other.Key].position, other.Value);
                var v = GetRelativeVelocity(state[p], state[other.Key]);
                f += GetSpringForce(d, v, shearStiffness, shearDamping);
            }
            return f;
        });

        particles.AddForce((p, state, dt) => {
            Vector3 f = Vector3.zero;
            foreach (var other in state[p].bend) {
                var d = GetDisplacement(state[p].position, state[other.Key].position, other.Value);
                var v = GetRelativeVelocity(state[p], state[other.Key]);
                f += GetSpringForce(d, v, bendStiffness, bendDamping);
            }
            return f;
        });

    }

    // Update is called once per frame
    protected override void Update() {
        
        particles.Update();
        ApplyForces();
        DetectCollisions();

        if (mesh) {
            UpdateGeometry();
        }

    }

    // TODO: fix naughty access
    void OnDrawGizmosSelected() {

        Gizmos.color = Color.green;

        foreach (var p in particles.particles) {
            foreach (var n in p.structural) {
                Gizmos.DrawLine(transform.TransformPoint(p.position), transform.TransformPoint(particles.particles[n.Key].position));
            }
        }

        Gizmos.color = Color.yellow;

        foreach (var p in particles.particles) {
            foreach (var n in p.shear) {
                Gizmos.DrawLine(transform.TransformPoint(p.position), transform.TransformPoint(particles.particles[n.Key].position));
            }
        }

        Gizmos.color = Color.blue;

        foreach (var p in particles.particles) {
            foreach (var n in p.bend) {
                Gizmos.DrawLine(transform.TransformPoint(p.position), transform.TransformPoint(particles.particles[n.Key].position));
            }
        }

    }

    protected override void DetectCollisions() {
        
        particles.DetectCollisions();

    }

    // PRIVATE METHODS

    private void CreateCube() {

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
                    uvs[idx[face]] = new Vector2(u + 0.5f, v + 0.5f);
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
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        // attach mesh to game object
        GetComponent<MeshFilter>().mesh = mesh;

        // keep track of which mesh vertices to move with particles
        vertexMap = new Dictionary<Particle, List<int>>();

        // create particles
        for (int i = 0; i < dim; i++) {
            for (int j = 0; j < dim; j++) {
                for (int k = 0; k < dim; k++) {
                    var p = new Vector3(-0.5f + step * i, -0.5f + step * j, -0.5f + step * k);
                    var vertexList = points.ContainsKey(p) ? points[p] : new List<int>();
                    var particle = new Particle(p, mass, e);
                    particles.particles[GetArrayIndex(i, j, k)] = particle;
                    vertexMap[particle] = vertexList;
                }
            }
        }

        // connect particles in sub-cube
        for (int plane = 0; plane < dim - 1; plane++) {
            for (int u = 0; u < dim; u++) {
                for (int v = 0; v < dim; v++) {
                    
                    AddStructuralSpring(GetArrayIndex(plane, u, v), GetArrayIndex(plane + 1, u, v));
                    AddStructuralSpring(GetArrayIndex(u, plane, v), GetArrayIndex(u, plane + 1, v));
                    AddStructuralSpring(GetArrayIndex(u, v, plane), GetArrayIndex(u, v, plane + 1));

                    if (plane < dim - 2) {
                        AddBendSpring(GetArrayIndex(plane, u, v), GetArrayIndex(plane + 2, u, v));
                        AddBendSpring(GetArrayIndex(u, plane, v), GetArrayIndex(u, plane + 2, v));
                        AddBendSpring(GetArrayIndex(u, v, plane), GetArrayIndex(u, v, plane + 2));
                    }
                
                }
            }
        }

        // planar shear springs
        for (int plane = 0; plane < dim; plane++) {
            for (int u = 0; u < dim - 1; u++) {
                for (int v = 0; v < dim - 1; v++) {
                    
                    AddStructuralSpring(GetArrayIndex(plane, u, v), GetArrayIndex(plane, u + 1, v + 1));
                    AddStructuralSpring(GetArrayIndex(plane, u + 1, v), GetArrayIndex(plane, u, v + 1));
                    AddStructuralSpring(GetArrayIndex(u, plane, v), GetArrayIndex(u + 1, plane, v + 1));
                    AddStructuralSpring(GetArrayIndex(u + 1, plane, v), GetArrayIndex(u, plane, v + 1));
                    AddStructuralSpring(GetArrayIndex(u, v, plane), GetArrayIndex(u + 1, v + 1, plane));
                    AddStructuralSpring(GetArrayIndex(u + 1, v, plane), GetArrayIndex(u, v + 1, plane));
                
                }
            }
        }

        // diagonal shear springs
        for (int i = 0; i < dim - 1; i++) {
            for (int j = 0; j < dim - 1; j++) {
                for (int k = 0; k < dim - 1; k++) {
                    
                    AddShearSpring(GetArrayIndex(i, j, k), GetArrayIndex(i + 1, j + 1, k + 1));
                    AddShearSpring(GetArrayIndex(i + 1, j, k), GetArrayIndex(i, j + 1, k + 1));
                    AddShearSpring(GetArrayIndex(i, j + 1, k), GetArrayIndex(i + 1, j, k + 1));
                    AddShearSpring(GetArrayIndex(i, j, k + 1), GetArrayIndex(i + 1, j + 1, k));
                
                }
            }
        }

    }

    private void UpdateGeometry() {

        var vertices = mesh.vertices;
        
        foreach (var p in vertexMap.Keys) {
            foreach (var i in vertexMap[p]) {
                vertices[i] = p.position;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

    }

    // HELPER METHODS

    private static Vector3 GetSpringForce(Vector3 d, Vector3 v, float k, float c) {
        return (d * k * -1) - (c * v);
    }

    public static Vector3 GetDisplacement(Vector3 a, Vector3 b, float expected) {
        if (a == b) {
            return Vector3.zero;
        }
        var actual = a - b;
        var displacement = Vector3.Magnitude(actual) - expected;
        return displacement * (actual / Vector3.Magnitude(actual));
    }

    public static Vector3 GetRelativeVelocity(Particle a, Particle b) {
        return Vector3.Project(a.velocity - b.velocity, a.position - b.position);
    }

    private int GetArrayIndex(int x, int y, int z) {
        return ((x * dim) + y) * dim + z; 
    }

    private void AddStructuralSpring(int p1, int p2) {
        particles.particles[p1].AddStructuralSpring(p2, ref particles.particles[p2]);
        particles.particles[p2].AddStructuralSpring(p1, ref particles.particles[p1]);
    }

    private void AddShearSpring(int p1, int p2) {
        particles.particles[p1].AddShearSpring(p2, ref particles.particles[p2]);
        particles.particles[p2].AddShearSpring(p1, ref particles.particles[p1]);
    }

    private void AddBendSpring(int p1, int p2) {
        particles.particles[p1].AddBendSpring(p2, ref particles.particles[p2]);
        particles.particles[p2].AddBendSpring(p1, ref particles.particles[p1]);
    }

}
