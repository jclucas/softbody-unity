using System.Collections.Generic;
using UnityEngine;
using static Force;

public class ParticleSystem {
    
    public Particle[] particles;

    public List<Force> forces = new List<Force>();

    public int size { get => particles.Length; }

    public ParticleSystem(int size) {
        particles = new Particle[size];
    }

    public void Update() {
        particles = particles.Integrate(forces, Time.fixedDeltaTime);
    }

    public void AddForceField(EvalFunction e) {
        forces.Add(new ForceField(e));
    }

    public void AddUnaryForce(int p, EvalFunction e) {
        forces.Add(new UnaryForce(p, e));
    }

    public void AddBinaryForce(int p1, int p2, EvalFunction e) {
        forces.Add(new BinaryForce(p1, p2, e));
    }

    public void DetectCollisions() {

        var colliders = Object.FindObjectsOfType<CollisionPlane>();

        foreach (var p in particles) {

            foreach (var plane in colliders) {

                if (plane.Collides((p.position))) {
                    var impulse = p.GetImpulsePlane(plane.normal, p.e);
                    p.MoveBack(plane.GetCollisionAmount(p.position));
                    p.velocity += impulse / p.mass;
                }

            }
            
        }

    }

    public float GetDistance(int a, int b) {
        return Vector3.Distance(particles[a].position, particles[b].position);
    }

}