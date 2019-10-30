using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(Particle particle, Dictionary<Particle, ParticleState> state, float dt);

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
        Dictionary<Particle, ParticleState> newState = new Dictionary<Particle, ParticleState>();
        foreach (var p in particles) {
            newState.Add(p, p.state);
        }
        newState = newState.IntegrateMidpoint(this, Time.fixedDeltaTime);
        foreach (var p in particles) {
            p.state = newState[p];
        }
    }

}