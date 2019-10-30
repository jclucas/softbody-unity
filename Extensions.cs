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
        
        for (var i = 0; i < state.Length; i++) {
            deriv[i] = new ParticleState(state[i].particle);
            deriv[i].force = f.eval(state[i].particle, state, dt);
            deriv[i].velocity = state[i].velocity + deriv[i].force / deriv[i].particle.mass;
            deriv[i].position = state[i].position + deriv[i].velocity;
        }
        
        return deriv;

    }

    public static ParticleState[] IntegrateEuler(this ParticleState[] state, Force f, float dt) {

        var newState = new ParticleState[state.Length];
        
        for (var i = 0; i < state.Length; i++) {
            newState[i] = new ParticleState(state[i].particle);
            newState[i].force = f.eval(state[i].particle, state, dt);
            newState[i].velocity = state[i].velocity + newState[i].force / newState[i].particle.mass * dt;
            newState[i].position = state[i].position + newState[i].velocity * dt;
        }
        
        return newState;
    }

    public static ParticleState[] IntegrateMidpoint(this ParticleState[] state, Force f, float dt) {
        // var k1 = this.IntegrateEuler(f, dt/2);
        // return k1.IntegrateEuler(f, dt/2);v
        var dx = state.IntegrateEuler(f, dt/2);
        return dx.IntegrateEuler(f, dt);

    }

    public static ParticleState[] Integrate(this ParticleState[] state, Force f, float dt) {
        var k1 = state.IntegrateEuler(f, dt);
        var k2 = k1.IntegrateEuler(f, dt/2);
        var k3 = k2.IntegrateEuler(f, dt/2);
        var k4 = k3.IntegrateEuler(f, dt);
        // return (k1 + k2 * 2 + k3 * 2 + k4) * (1/6);
        return k4;
    }

}