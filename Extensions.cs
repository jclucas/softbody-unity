using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

    // // f(x + dx) = f(x) + f'(x) * dx
    // public static Vector3 IntegrateEuler(this Vector3 x, Vector3 dxdt, float dt) {
    //     return x + dxdt * dt;
    // }

    // public static Vector3 IntegrateMidpoint(this Vector3 x, Vector3 dxdt, float dt) {
    //     // var mid = x + dt / 2 * dxdt;
    //     // return x + dt * (mid - x) / (dt / 2);
    //     var dx = dxdt * dt;
    //     return x + (x + dx / 2) * dt;
    // }

    // public delegate Vector3 Eval(Vector3 x, float dt);

    // public static Vector3 Integrate(this Vector3 x, Eval dxdt, float dt) {
    //     var k1 = dxdt(x, dt);
    //     var k2 = dxdt(x + k1, dt / 2);
    //     var k3 = dxdt(x + k2, dt / 2);
    //     var k4 = dxdt(x + k3, dt);
    //     return x + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
    // }


    public static ParticleState[] Derivative(this ParticleState[] state, Force f, float dt) {

        var deriv = new ParticleState[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            deriv[i].force = f.eval(i, state, dt);
            deriv[i].velocity = state[i].velocity + deriv[i].force / state[i].mass;
            deriv[i].position = state[i].position + deriv[i].velocity;
        }
        
        return deriv;

    }

    public static ParticleState[] Step(this ParticleState[] state, Force f, float dt) {

        var deriv = new ParticleState[state.Length];
        
        for (int i = 0; i < state.Length; i++) {
            deriv[i] = new ParticleState(state[i].mass);
            // step forward with current derivatives
            deriv[i].position = state[i].position + state[i].velocity * dt;
            deriv[i].velocity = state[i].velocity + state[i].force / state[i].mass * dt;
            // calculate
            deriv[i].force = f.eval(i, state, dt);
        }
        
        return deriv;

    }

    public static ParticleState[] IntegrateMidpoint(this ParticleState[] state, Force f, float dt) {
        var k1 = state.Step(f, dt / 2);
        return state.Step(f, dt);
    }

    // public static ParticleState[] IntegrateMidpoint(this ParticleState[] state, Force f, float dt) {
    //     // var k1 = this.IntegrateEuler(f, dt/2);
    //     // return k1.IntegrateEuler(f, dt/2);
    //     var dx = state.IntegrateEuler(f, dt/2);
    //     var integral = dx.IntegrateEuler(f, dt);
        
    //     foreach (var p in state.Keys) {
    //         Debug.Log("midpoint = " + dx[p].force);
    //         Debug.Log("integral = " + integral[p].force);
    //     }
    //     return integral;
    // }

    public static ParticleState[] Integrate(this ParticleState[] state, Force f, float dt) {
        var k1 = state.Step(f, 0);
        var k2 = k1.Step(f, dt/2);
        var k3 = k2.Step(f, dt/2);
        var k4 = k3.Step(f, dt);

        var deriv = new ParticleState[state.Length];
        for (int p = 0; p < state.Length; p++) {
            deriv[p] = (k1[p] + (k2[p] + k3[p]) * 2 + k4[p]);
            deriv[p] *= (1f/6f);
            deriv[p] *= dt;
            Debug.Log("k1 = " + k1[p].position);
            Debug.Log("k2 = " + k2[p].position);
            Debug.Log("k3 = " + k3[p].position);
            Debug.Log("k4 = " + k4[p].position);
            Debug.Log("result = " + deriv[p].position);
            state[p] += deriv[p];
        }
        // return state;
        return k4;
    }

}