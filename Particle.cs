using UnityEngine;

public class Particle {

    /// <summary>
    /// Position of the particle.
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// Velocity of the particle.
    /// </summary>
    public Vector3 velocity;
    
    /// <summary>
    /// Force on the particle.
    /// </summary>
    public Vector3 force;
    
    /// <summary>
    /// Mass of the particle.
    /// </summary>
    public float mass;
    
    /// <summary>
    /// Coefficient of restitution for the particle.
    /// </summary>
    public float e;

    /// <summary>
    /// If true, the particle will not move, but continues to accumulate force and momentum.
    /// </summary>
    public bool frozen;

    /// <summary>
    /// Create a new particle with zero velocity and force.
    /// </summary>
    /// <param name="position">Initial position of the particle.</param>
    /// <param name="mass">Mass of the particle.</param>
    /// <param name="e">Coefficient of restitution.</param>
    public Particle(Vector3 position, float mass, float e) {
        this.position = position;
        this.velocity = Vector3.zero;
        this.force = Vector3.zero;
        this.mass = mass;
        this.e = e;
        this.frozen = false;
    }

    /// <summary>
    /// Set the state of this particle object from a state struct.
    /// </summary>
    /// <param name="state">A position, velocity, and force.</param>
    public void SetState(ParticleState state) {
        this.position = state.position;
        this.velocity = state.velocity;
        this.force = state.force;
    }

    /// <summary>
    /// Calculate the impulse on the particle due to a collision.
    /// </summary>
    /// <param name="n">Normal of the collision surface.</param>
    /// <returns>Impulse on the particle.</returns>
    public Vector3 GetImpulsePlane(Vector3 n) {
        return ((-(1 + e) * (Vector3.Dot(this.velocity, n))) / (1/this.mass)) * n;
    }

    /// <summary>
    /// Move the particle a specified distance in the direction of its velocity.
    /// </summary>
    /// <param name="difference">Vector from the particle's current position to an acceptable position.</param>
    public void MoveBack(Vector3 difference) {
        position += Vector3.Project(difference, velocity);
    }

}

/// <summary>
/// Struct of state properties used in integration. Used in intermediate integration steps.
/// </summary>
public struct ParticleState {

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;

}