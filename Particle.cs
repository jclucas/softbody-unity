using UnityEngine;
using System.Collections.Generic;

public class Particle {

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public float mass;
    public float e;

    public Particle(Vector3 position, float mass, float e) {
        this.position = position;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
        this.e = e;
    }

    public void SetState(ParticleState state) {
        this.position = state.position;
        this.velocity = state.velocity;
        this.force = state.force;
    }

    private float GetDistance(Particle other) {
        return Vector3.Distance(position, other.position);
    }

    // impulse for collision with a plane with normal n
    public Vector3 GetImpulsePlane(Vector3 n, float e) {
        return ((-(1 + e) * (Vector3.Dot(this.velocity, n))) / (1/this.mass)) * n;
    }

    // move position of particle to given plane along velocity
    public void MoveToPlane(Plane p) {
        position += Vector3.Project(p.ClosestPointOnPlane(position) - position, velocity);
    }

    // move particle back in time
    public void MoveBack(Vector3 difference) {
        position += Vector3.Project(difference, velocity);
    }

    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !! warning.. assumes a.particle == b.particle !!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public static Particle operator +(Particle a, Particle b) {
        a.position += b.position;
        a.velocity += b.velocity;
        a.force += b.force;
        return a;
    }

    public static Particle operator *(Particle a, float s) {
        a.position *= s;
        a.velocity *= s;
        a.force *= s;
        return a;
    }

}

// struct of state variables for integration
public struct ParticleState {

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;

}