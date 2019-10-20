using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // PUBLIC PROPERTIES

    public float mass = 1;

    public float k = 1;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public Vector3 accel = gravity;

    public Vector3 velocity = new Vector3(0, 0, 0);

    public Vector3 momentum = new Vector3(0, 0, 0);

    // GEOMETRY

    private Mesh mesh;

    private EdgeList edges;

    // Start is called before the first frame update
    void Start() {

        // initial values
        // velocity = new Vector3(0, 0, 0);
        // momentum = new Vector3(0, 0, 0);
        
        mesh = GetComponent<MeshFilter>().mesh;
        edges = new EdgeList(mesh);

    }

    // Update is called once per frame
    internal void Update() {
        
        // calculate forces
        var F = accel * mass;

        // integrate position and rotation
        transform.position = transform.position.IntegrateMidpoint(velocity, Time.deltaTime);

        // collision detection, get penalty force
        var penalty = DetectCollisions();

        // update momentum (integrate forces)
        momentum = momentum.IntegrateMidpoint(F + penalty, Time.deltaTime);

        // update velocities
        velocity = momentum / mass;

    }

    // UTILITY FUNCTIONS

    private Vector3 DetectCollisions() {

        // CHEATING just don't let it go below the floor
        if (transform.position.y < 0.5) {

            // back up to before collision
            var surface = new Vector3(transform.position.x, 0.5f, transform.position.z);
            var delta = transform.position - surface;

            // calculate spring force
            return getSpringForce(delta, k);

        } else {
            return Vector3.zero;
        }

    }

    private static Vector3 getSpringForce(Vector3 d, float k) {
        return d * k * -1;
    }

}
