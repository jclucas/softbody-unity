using UnityEngine;
using System.Collections.Generic;

public class BinaryForce : Force {

    // new public List<Particle[]> particles;

    Particle a, b;

    public BinaryForce(EvalFunction eval, Particle a, Particle b) : base(eval) {
        this.a = a;
        this.b = b;
    }

    new public void Apply() {
        a.state = a.state.Integrate(this, Time.deltaTime);
    }


}