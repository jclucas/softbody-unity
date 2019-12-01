using System.Collections.Generic;
using UnityEngine;
using static Force;

public class ParticleSystem {
    
    public Particle[] particles;

    public List<Force> forces = new List<Force>();

    public int size { get => particles.Length; }

    // the floor
    public Plane floor;

    public ParticleSystem(int size) {
        particles = new Particle[size];
    }

    public void Update() {
        foreach (var f in forces) {
            f.Apply();
        }
    }

    public void AddForce(EvalFunction e) {
        forces.Add(new Force(e, particles));
    }

    public void DetectCollisions() {

        foreach (var p in particles) {
            if (p.CollidesPlane(floor)) {
                var impulse = p.GetImpulsePlane(Vector3.up, p.e);
                p.MoveToPlane(floor);
                p.velocity += impulse / p.mass;
            } 
        }

    }

    public float GetDistance(int a, int b) {
        return Vector3.Distance(particles[a].position, particles[b].position);
    }

}