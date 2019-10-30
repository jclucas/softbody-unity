using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(Particle particle, Dictionary<Particle, ParticleState> state, float dt);

    // evaluation function
    public EvalFunction eval;

    // particles to apply it to
    public List<Particle> particles;

    public Force(EvalFunction eval) {
        this.eval = eval;
    }

    public Force(EvalFunction eval, List<Particle> particles) {
        this.eval = eval;
        this.particles = particles;
    }

    public void Apply() {
        ParticleState[] newState = new ParticleState[particles.Count];
        for (var i = 0; i < particles.Count; i++) {
            newState[i] = particles[i].state;
        }
        newState = newState.IntegrateMidpoint(this, Time.deltaTime);
        for (var i = 0; i < particles.Count; i++) {
            particles[i].state = newState[i];
        }
    }

}