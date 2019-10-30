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


    public static Dictionary<Particle, ParticleState> Derivative(this Dictionary<Particle, ParticleState> state, Force f, float dt) {

        var deriv = new Dictionary<Particle, ParticleState>();
        
        foreach (var p in state.Keys) {
            deriv.Add(p, new ParticleState());
            deriv[p].force = f.eval(p, state, dt);
            deriv[p].velocity = state[p].velocity + deriv[p].force / p.mass;
            deriv[p].position = state[p].position + deriv[p].velocity;
        }
        
        return deriv;

    }

    public static Dictionary<Particle, ParticleState> IntegrateEuler(this Dictionary<Particle, ParticleState> state, Force f, float dt) {

        var deriv = new Dictionary<Particle, ParticleState>();
        
        foreach (var p in state.Keys) {
            deriv.Add(p, new ParticleState());
            deriv[p].force = f.eval(p, state, dt);
            deriv[p].velocity = state[p].velocity + deriv[p].force / p.mass * dt;
            deriv[p].position = state[p].position + deriv[p].velocity * dt;
        }
        
        return deriv;

    }

    public static Dictionary<Particle, ParticleState> IntegrateMidpoint(this Dictionary<Particle, ParticleState> state, Force f, float dt) {
        // var k1 = this.IntegrateEuler(f, dt/2);
        // return k1.IntegrateEuler(f, dt/2);v
        var dx = state.IntegrateEuler(f, dt/2);
        return dx.IntegrateEuler(f, dt);

    }

    public static Dictionary<Particle, ParticleState> Integrate(this Dictionary<Particle, ParticleState> state, Force f, float dt) {
        var k1 = state.IntegrateEuler(f, dt);
        var k2 = k1.IntegrateEuler(f, dt/2);
        var k3 = k2.IntegrateEuler(f, dt/2);
        var k4 = k3.IntegrateEuler(f, dt);
        // return (k1 + k2 * 2 + k3 * 2 + k4) * (1/6);
        return k4;
    }

}