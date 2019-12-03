using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

    public static ParticleState[] Step(this Particle[] state, List<Force> f, float dt) {

        var next = new ParticleState[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            next[i] = new ParticleState();
            // step forward with current derivatives
            next[i].position = state[i].position + state[i].velocity * dt;
            next[i].velocity = state[i].velocity + state[i].force / state[i].mass * dt;
            // calculate
            next[i].force = Vector3.zero;
        }

        foreach (var force in f) {
            var result = force.Eval(state, dt);
            for (int i = 0; i < state.Length; i++) {
                next[i].force += result[i];
            }
        }

        for (int i = 0; i < state.Length; i++) {
            state[i].SetState(next[i]);
        }
        
        return next;

    }

    public static ParticleState[] IntegrateMidpoint(this Particle[] state, List<Force> f, float dt) {
        var k1 = state.Step(f, dt / 2);
        return state.Step(f, dt);
    }

    public static Particle[] Integrate(this Particle[] state, List<Force> f, float dt) {
        
        var k1 = state.Step(f, 0);
        var k2 = state.Step(f, dt/2);
        var k3 = state.Step(f, dt/2);
        var k4 = state.Step(f, dt);

        for (int p = 0; p < state.Length; p++) {
            state[p].position = ((k1[p].position + (k2[p].position + k3[p].position) * 2 + k4[p].position) * (1f/6f));
            state[p].velocity = ((k1[p].velocity + (k2[p].velocity + k3[p].velocity) * 2 + k4[p].velocity) * (1f/6f));
            state[p].force = ((k1[p].force + (k2[p].force + k3[p].force) * 2 + k4[p].force) * (1f/6f));
        }

        return state;

    }

}