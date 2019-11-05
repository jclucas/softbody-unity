using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(int particle, Particle[] state, float dt);

    // evaluation function
    public EvalFunction eval;

    // particles to apply it to
    public List<Particle> particles;

    public Force(EvalFunction eval, List<Particle> particles) {
        this.eval = eval;
        this.particles = particles;
    }

    public void Apply() {
        particles = new List<Particle>(particles.ToArray().Integrate(this, Time.fixedDeltaTime));
    }

}