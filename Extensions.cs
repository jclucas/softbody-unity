using UnityEngine;

public static class Extensions {

    // f(x + dx) = f(x) + f'(x) * dx
    public static Vector3 IntegrateEuler(this Vector3 value, Vector3 deriv, float step) {
        return value + deriv * step;
    }

    public static Vector3 IntegrateMidpoint(this Vector3 value, Vector3 deriv, float step) {
        var mid = value + step / 2 * deriv;
        return value + step * (mid - value) / (step / 2);
    }

    // private Vector3 Integrate() {
    // }

}