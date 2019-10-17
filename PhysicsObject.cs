using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // PUBLIC PROPERTIES

    public float mass;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    private Vector3 velocity;

    private Vector3 momentum;

    // Start is called before the first frame update
    void Start() {

        // initial values
        velocity = new Vector3(0, 0, 0);
        momentum = new Vector3(0, 0, 0);
        Debug.Log("Good morning, I am an object of mass " + mass);

    }

    // Update is called once per frame
    void Update() {
        
        // calculate forces
        var F = gravity * mass;

        // integrate position and rotation
        // s(t+dt) = s(t) + v(t)dt
        // q(t+dt) = q(t) + 0.5(w(t)q(t))dt
        transform.position += velocity * Time.deltaTime;

        // update momentum (integrate forces)
        // M(t+dt) = M(t) + F(t)dt
        momentum += F * Time.deltaTime;

        // collision detection
        DetectCollisions();

        // update velocities
        velocity = momentum / mass;

    }

    // UTILITY FUNCTIONS

    private void DetectCollisions() {

        // CHEATING just don't let it go below the floor
        if (transform.position.y < 0.5) {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }

    }

}
