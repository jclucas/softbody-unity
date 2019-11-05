using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

    public static Particle[] Step(this Particle[] state, Force f, float dt) {

        var deriv = new ParticleState[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            deriv[i] = new ParticleState();
            // step forward with current derivatives
            deriv[i].position = state[i].position + state[i].velocity * dt;
            deriv[i].velocity = state[i].velocity + state[i].force / state[i].mass * dt;
            // calculate
            deriv[i].force = f.eval(i, state, dt);
            state[i].SetState(deriv[i]);
        }
        
        return state;

    }

    public static Particle[] IntegrateMidpoint(this Particle[] state, Force f, float dt) {
        var k1 = state.Step(f, dt / 2);
        return state.Step(f, dt);
    }

    public static Particle[] Integrate(this Particle[] state, Force f, float dt) {
        
        var k1 = state.Step(f, 0);
        var k2 = k1.Step(f, dt/2);
        var k3 = k2.Step(f, dt/2);
        var k4 = k3.Step(f, dt);

        for (int p = 0; p < state.Length; p++) {
            Debug.Log("Position [p]: " + state[p].position);
            state[p].position = ((k1[p].position + (k2[p].position + k3[p].position) * 2 + k4[p].position) * (1f/6f));
            state[p].velocity = ((k1[p].velocity + (k2[p].velocity + k3[p].velocity) * 2 + k4[p].velocity) * (1f/6f));
            state[p].force = ((k1[p].force + (k2[p].force + k3[p].force) * 2 + k4[p].force) * (1f/6f));
        }

        return state;

    }

}