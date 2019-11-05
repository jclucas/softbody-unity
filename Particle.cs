using UnityEngine;
using System.Collections.Generic;

public class Particle {

    // List of associated mesh vertices
    List<int> vertices;

    public Vector3 force;

    public ParticleState state;

    public float mass;

    public static float k = 1;
    
    public static float damping = 0;

    // Adjacency list
    public Dictionary<Particle, float> neighbors;

    public Particle(Vector3 position, float mass, List<int> vertices) {
        this.vertices = vertices;
        state = new ParticleState(mass, position);
        neighbors = new Dictionary<Particle, float>();
    }

    public void AddEdge(ref Particle other) {
        if (!neighbors.ContainsKey(other)) {
            neighbors.Add(other, GetDistance(other));
        }
    }

    // update point state & mesh vertices
    public void SetPosition(Vector3 newPos, ref Vector3[] mesh) {
        state.position = newPos;
        foreach (var i in vertices) {
            mesh[i] = newPos;
        }
    }

    public void UpdateMesh(ref Vector3[] mesh) {
        foreach (var i in vertices) {
            mesh[i] = state.position;
        }
    }

    private float GetDistance(Particle other) {
        return Vector3.Distance(state.position, other.state.position);
    }

    public bool CollidesPlane(Plane p) {
        return (p.GetDistanceToPoint(state.position) < 0);
    }

    // impulse for collision with a plane with normal n
    public Vector3 GetImpulsePlane(Vector3 n, float e) {
        return ((-(1 + e) * (Vector3.Dot(this.state.velocity, n))) / (1/this.mass)) * n;
    }

    // private Vector3 GetDisplacement(Particle other) {}

}

public class ParticleState {

    // public Particle particle;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public float mass;

    public ParticleState(float mass) {
        // this. particle = particle;
        this.position = Vector3.zero;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
    }

    public ParticleState(float mass, Vector3 position) {
        // this.particle = particle;
        this.position = position;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
    }

    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !! warning.. assumes a.particle == b.particle !!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public static ParticleState operator +(ParticleState a, ParticleState b) {
        var state = new ParticleState(a.mass);
        state.position = a.position + b.position;
        state.velocity = a.velocity + b.velocity;
        state.force = a.force + b.force;
        return state;
    }

    public static ParticleState operator *(ParticleState a, float s) {
        var state = new ParticleState(a.mass);
        state.position = a.position * s;
        state.velocity = a.velocity * s;
        state.force = a.force * s;
        Debug.Log("==> " + a.position + " * " + s + " = " + state.position);
        return state;
    }

}