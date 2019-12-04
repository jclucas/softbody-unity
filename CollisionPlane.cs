using UnityEngine;

public class CollisionPlane : MonoBehaviour {

    public Vector3 normal { 
        get => plane.normal;
    }

    private Plane plane;

    public void Start() {
        plane = new Plane(transform.TransformDirection(Vector3.up), transform.position);
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + plane.normal);
    }

    public bool Collides(Vector3 point) {
        return (!plane.GetSide(point));
    }

    public Vector3 GetCollisionAmount(Vector3 point) {
        return plane.ClosestPointOnPlane(point) - point;
    }

}