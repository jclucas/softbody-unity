using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // PUBLIC PROPERTIES

    public float mass = 1;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public Vector3 accel = gravity;

    public Vector3 velocity = new Vector3(0, 0, 0);

    public Vector3 momentum = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start() {

        // initial values
        // velocity = new Vector3(0, 0, 0);
        // momentum = new Vector3(0, 0, 0);
        Debug.Log("Good morning, I am an object of mass " + mass);

    }

    // Update is called once per frame
    internal void Update() {
        
        // calculate forces
        var F = accel * mass;

        // integrate position and rotation
        // s(t+dt) = s(t) + v(t)dt
        // q(t+dt) = q(t) + 0.5(w(t)q(t))dt
        transform.position += velocity * Time.deltaTime;

        // update momentum (integrate forces)
        // M(t+dt) = M(t) + F(t)dt
        momentum += F * Time.deltaTime;

        // collision detection
        var impulse = DetectCollisions();

        // update momentum
        momentum += F * Time.deltaTime + impulse;

        // update velocities
        velocity = momentum / mass;

    }

    // UTILITY FUNCTIONS

    private Vector3 DetectCollisions() {

        // CHEATING just don't let it go below the floor
        if (transform.position.y < 0.5) {
            // back up to before collision
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            // calculate impulse
            // CHEATING just absorb all momentum
            return momentum * -1;
        } else {
            return Vector3.zero;
        }

    }

}
