using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(int particle, ParticleState[] state, float dt);

    // evaluation function
    public EvalFunction eval;

    // particles to apply it to
    public List<Particle> particles;

    // this is illegal
    // public Force(EvalFunction eval) {
    //     this.eval = eval;
    // }

    public Force(EvalFunction eval, List<Particle> particles) {
        this.eval = eval;
        this.particles = particles;
    }

    public void Apply() {
        var newState = new ParticleState[particles.Count];
        for (var i = 0; i < particles.Count; i++) {
            newState[i] = particles[i].state;
        }
        newState = newState.Integrate(this, Time.fixedDeltaTime);
        for (var i = 0; i < particles.Count; i++) {
            particles[i].state = newState[i];
        }
    }

}