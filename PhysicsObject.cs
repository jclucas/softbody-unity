using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // PUBLIC PROPERTIES

    public float mass = 1;

    public float internalK = 50;

    public float collisionK = 1000;

    public float damping = 1;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public Vector3[] accel;

    public Vector3[] velocity;

    public Vector3[] momentum;

    // GEOMETRY

    private Mesh mesh;

    private EdgeList edges;

    // Start is called before the first frame update
    void Start() {
        
        mesh = GetComponent<MeshFilter>().mesh;
        edges = new EdgeList(mesh);
        accel = new Vector3[mesh.vertexCount];
        velocity = new Vector3[mesh.vertexCount];
        momentum = new Vector3[mesh.vertexCount];

        for (var i = 0; i < mesh.vertexCount; i++) {
            Debug.Log("VERTEX " + i + ": " + mesh.vertices[i]);
            accel[i] = gravity;
            velocity[i] = Vector3.zero;
            momentum[i] = Vector3.zero;
        }

    }

    // Update is called once per frame
    internal void Update() {
        
        var vertices = mesh.vertices;

        // calculate forces
        for (var i = 0; i < mesh.vertexCount; i++) {
            accel[i] = gravity + getEdgeForce(i) / mass;
        }

        for (var i = 0; i < mesh.vertexCount; i++) {

            // integrate position and rotation
            vertices[i] = vertices[i].Integrate(velocity[i], Time.deltaTime);

            // collision detection, get penalty force
            var penalty = DetectCollisions(i);

            // update momentum (integrate forces)
            var F = accel[i] * mass;
            momentum[i] = momentum[i].IntegrateEuler(F + penalty, Time.deltaTime);

            // update velocities
            velocity[i] = momentum[i] / mass;
        
        }

        mesh.vertices = vertices;

    }

    // UTILITY FUNCTIONS

    private Vector3 DetectCollisions(int i) {

        // world space transform
        var world = transform.TransformPoint(mesh.vertices[i]);

        // CHEATING just don't let it go below the floor
        if (world.y < 0) {

            // back up to before collision
            var surface = transform.InverseTransformPoint(new Vector3(world.x, 0, world.z));
            var delta = mesh.vertices[i] - surface;

            // calculate spring force
            return getSpringForce(delta, velocity[i], collisionK, damping);

        } else {
            return Vector3.zero;
        }

    }

    private Vector3 getEdgeForce(int i) {

        Vector3 force = Vector3.zero;

        foreach (int n in edges.GetNeighbors(i)) {

            var F = getSpringForce(edges.GetDisplacement(i, n), velocity[n] - velocity[i], internalK, damping) / mass;
            if (Vector3.Magnitude(F) > 0.01) {
                
                Debug.Log("Vertex " + i + " applying a force of " +  F + " to " + n);
                force += F;
            }
        }

        return force;

    }

    // k: spring constant
    // c: damping constant
    private static Vector3 getSpringForce(Vector3 d, Vector3 v, float k, float c) {
        return (d * k * -1) - (c * v);
    }

}
