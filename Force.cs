using UnityEngine;

public abstract class Force {

    public delegate Vector3 EvalFunction(int particle, Particle[] state, float dt);

    // evaluation function
    internal EvalFunction eval;

    // particles to apply it to
    // public Particle[] particles;

    public Force(EvalFunction eval) { //, Particle[] particles) {
        this.eval = eval;
        // this.particles = particles;
    }

    public abstract Vector3[] Eval(Particle[] state, float dt);

}

public class ForceField : Force {

    public ForceField(EvalFunction eval) : base(eval) {}

    public override Vector3[] Eval(Particle[] state, float dt) {

        var result = new Vector3[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            result[i] = eval(i, state, dt);
        }

        return result;

    }

}

public class UnaryForce : Force {

    // particle index
    int p;

    public UnaryForce(int particle, EvalFunction eval) : base(eval) {
        p = particle;
    }

    public override Vector3[] Eval(Particle[] state, float dt) {

        var result = new Vector3[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            result[i] = Vector3.zero;
        }

        result[p] = eval(p, state, dt);

        return result;

    }

}

public class BinaryForce : Force {

    // particle indices
    int p1, p2;

    public BinaryForce(int p1, int p2, EvalFunction eval) : base(eval) {
        this.p1 = p1;
        this.p2 = p2;
    }

    public override Vector3[] Eval(Particle[] state, float dt) {

        var result = new Vector3[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            result[i] = Vector3.zero;
        }

        result[p1] =  eval(p1, state, dt);
        result[p2] =  eval(p2, state, dt);

        return result;

    }

}