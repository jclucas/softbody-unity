using UnityEngine;

/// <summary>
/// A force that can be evaluated based on the current state of the system.
/// </summary>
public abstract class Force {

    /// <summary>
    /// Force evaluation function.
    /// </summary>
    /// <param name="particle">Index of particle being evaluated.</param>
    /// <param name="state">Array of all particles in the system.</param>
    /// <param name="dt">Evaluation time step.</param>
    /// <returns></returns>
    public delegate Vector3 EvalFunction(int particle, Particle[] state, float dt);

    /// <summary>
    /// Function for evaluating this force.
    /// </summary>
    internal EvalFunction eval;

    /// <summary>
    /// Constructor for a force.
    /// </summary>
    /// <param name="eval">Evaluation function.</param>
    public Force(EvalFunction eval) {
        this.eval = eval;
    }

    /// <summary>
    /// Evaluates a force on a system.
    /// </summary>
    /// <param name="state">Current state of the system.</param>
    /// <param name="dt">Evaluation time step.</param>
    /// <returns></returns>
    public abstract Vector3[] Eval(Particle[] state, float dt);

}

/// <summary>
/// A force that applies to all particles in the state.
/// </summary>
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

/// <summary>
/// A force that applies to a single particle.
/// </summary>
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

/// <summary>
/// An equal and opposite force applied to a pair of particles.
/// </summary>
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