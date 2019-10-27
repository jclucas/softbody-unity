using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(ParticleState state, float dt);

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
        foreach (var p in particles) {
            p.state = p.state.IntegrateEuler(this, Time.deltaTime);
        }
    }

}