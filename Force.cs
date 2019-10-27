using UnityEngine;
using System.Collections.Generic;

public class Force {

    public delegate Vector3 EvalFunction(ParticleState state, float dt);

    public EvalFunction eval;

    public Force(EvalFunction eval) {
        this.eval = eval;
    }

}