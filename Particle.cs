using UnityEngine;
using System.Collections.Generic;

public class Particle {

    // List of associated mesh vertices
    List<int> vertices;

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;
    public float mass;

    // Adjacency list
    public Dictionary<int, float> structural;
    public Dictionary<int, float> shear;
    public Dictionary<int, float> bend;

    public Particle(Vector3 position, float mass, List<int> vertices) {
        this.vertices = vertices;
        this.position = position;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
        structural = new Dictionary<int, float>();
        shear = new Dictionary<int, float>();
        bend = new Dictionary<int, float>();
    }

    public void SetState(ParticleState state) {
        this.position = state.position;
        this.velocity = state.velocity;
        this.force = state.force;
    }

    public void AddStructuralSpring(int index, ref Particle other) {
        if (!structural.ContainsKey(index)) {
            structural.Add(index, GetDistance(other));
        }
    }

    public void AddShearSpring(int index, ref Particle other) {
        if (!shear.ContainsKey(index)) {
            shear.Add(index, GetDistance(other));
        }
    }

    public void AddBendSpring(int index, ref Particle other) {
        if (!bend.ContainsKey(index)) {
            bend.Add(index, GetDistance(other));
        }
    }

    public void UpdateMesh(ref Vector3[] mesh) {
        foreach (var i in vertices) {
            mesh[i] = position;
        }
    }

    private float GetDistance(Particle other) {
        return Vector3.Distance(position, other.position);
    }

    public bool CollidesPlane(Plane p) {
        return (p.GetDistanceToPoint(position) < 0);
    }

    // impulse for collision with a plane with normal n
    public Vector3 GetImpulsePlane(Vector3 n, float e) {
        return ((-(1 + e) * (Vector3.Dot(this.velocity, n))) / (1/this.mass)) * n;
    }

    // move position of particle to given plane along velocity
    public void MoveToPlane(Plane p) {
        var scale = (p.ClosestPointOnPlane(position).y - position.y) / velocity.y;
        position += velocity.normalized * scale;
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