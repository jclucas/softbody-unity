using System.Collections.Generic;
using UnityEngine;
using static Force;

public class ParticleSystem {
    
    /// <summary>
    /// Array of particles in the system.
    /// </summary>
    private Particle[] particles;

    /// <summary>
    /// List of forces applied to the system.
    /// </summary>
    public List<Force> forces = new List<Force>();

    public int size { get => particles.Length; }

    private Transform transform;

    /// <summary>
    /// Create an empty particle system.
    /// </summary>
    /// <param name="size">Maximum number of particles.</param>
    public ParticleSystem(int size) {
        particles = new Particle[size];
    }

    public ParticleSystem(int size, Transform transform) {
        particles = new Particle[size];
        this.transform = transform;
    }

    /// <summary>
    /// Integrate the system and apply new state.
    /// </summary>
    public void Update() {
        particles = particles.Integrate(forces, Time.fixedDeltaTime);
    }

    /// <summary>
    /// Add an existing particle.
    /// </summary>
    /// <param name="index">Array position to insert.</param>
    /// <param name="p">An existing particle to insert.</param>
    public void AddParticle(int index, Particle p) {
        particles[index] = p;
    }

    /// <summary>
    /// Create and add a new particle.
    /// </summary>
    /// <param name="index">Array position to insert.</param>
    /// <param name="position">Initial position of new particle.</param>
    /// <param name="mass">Mass of new particle.</param>
    /// <param name="e">Coefficient of restitution for the new particle.</param>
    public void SpawnParticle(int index, Vector3 position, float mass, float e) {
        particles[index] = new Particle(position, mass, e);
    }

    /// <summary>
    /// Get the position of a particle.
    /// </summary>
    /// <param name="index">Array index of a particle.</param>
    /// <returns>The particle's position in object space.</returns>
    public Vector3 GetPosition(int index) {
        return transform.InverseTransformPoint(particles[index].position);
    }

    /// <summary>
    /// Set the position of a particle.
    /// </summary>
    /// <param name="index">Array index of a particle.</param>
    /// <param name="newPos">New position in object space.</param>
    public void SetPosition(int index, Vector3 newPos) {
        particles[index].position = transform.TransformPoint(newPos);
    }

    /// <summary>
    /// Create and add a force field to the system.
    /// </summary>
    /// <param name="e">Force evaluation function.</param>
    public void AddForceField(EvalFunction e) {
        forces.Add(new ForceField(e));
    }

    /// <summary>
    /// Create and add a unary force to the system.
    /// </summary>
    /// <param name="p">Array index of the particle the force affects.</param>
    /// <param name="e">Force evaluation function.</param>
    public void AddUnaryForce(int p, EvalFunction e) {
        forces.Add(new UnaryForce(p, e));
    }

    /// <summary>
    /// Create and add a binary force to the system.
    /// </summary>
    /// <param name="p1">Array index of the first particle.</param>
    /// <param name="p2">Array index of the second particle.</param>
    /// <param name="e">Force evaluation function.</param>
    public void AddBinaryForce(int p1, int p2, EvalFunction e) {
        forces.Add(new BinaryForce(p1, p2, e));
    }

    /// <summary>
    /// Perform collision detection and response.
    /// </summary>
    public void DetectCollisions() {

        var colliders = Object.FindObjectsOfType<CollisionObject>();

        foreach (var p in particles) {

            foreach (var obj in colliders) {

                if (obj.Collides((p.position))) {
                    var normal = obj.GetCollisionAmount(p.position);
                    var impulse = p.GetImpulsePlane(normal.normalized, p.e);
                    p.MoveBack(normal);
                    p.velocity += impulse / p.mass;
                }

            }

        }

    }

    /// <summary>
    /// Get the distance between two particles.
    /// </summary>
    /// <param name="a">Index of the first particle.</param>
    /// <param name="b">Index of the second particle.</param>
    /// <returns>The distance between the two particles.</returns>
    public float GetDistance(int a, int b) {
        return Vector3.Distance(particles[a].position, particles[b].position);
    }

}