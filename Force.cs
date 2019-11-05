using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(int particle, Particle[] state, float dt);

    // evaluation function
    public EvalFunction eval;

    // particles to apply it to
    public Particle[] particles;

    public Force(EvalFunction eval, Particle[] particles) {
        this.eval = eval;
        this.particles = particles;
    }

    public void Apply() {
        particles = particles.Integrate(this, Time.fixedDeltaTime);
    }

}