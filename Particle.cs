using UnityEngine;
using System.Collections.Generic;

public class Particle {

    // List of associated mesh vertices
    List<int> vertices;

    public Vector3 force;

    public ParticleState state;

    public static float k = 1;
    
    public static float damping = 0;

    // Adjacency list
    public Dictionary<Particle, float> neighbors;

    public Particle(Vector3 position, float mass, List<int> vertices) {
        this.vertices = vertices;
        state = new ParticleState(position, mass);
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

    // private Vector3 GetDisplacement(Particle other) {}

}

public class ParticleState {

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public readonly float mass;

    public ParticleState(float mass) {
        this.position = Vector3.zero;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
    }

    public ParticleState(Vector3 position, float mass) {
        this.position = position;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
    }

    public ParticleState IntegrateEuler(Force f, float dt) {
        var newState = new ParticleState(this.mass);
        newState.force = f.eval(this, dt);
        newState.velocity = this.velocity + newState.force / newState.mass * dt;
        newState.position = this.position + newState.velocity * dt;
        return newState;
    }

}