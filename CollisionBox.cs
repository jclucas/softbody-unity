using UnityEngine;

public class CollisionBox : CollisionObject {

    private BoxCollider box;

    public void Start() {
        box = GetComponent<BoxCollider>();
    }

    public override bool Collides(Vector3 point) {
        return box.bounds.Contains(point);
    }

    public override Vector3 GetCollisionAmount(Vector3 point) {
        return   ClosestPointOnBounds(point) - point;
    }

    public Vector3 ClosestPointOnBounds(Vector3 position) {

        // inside -- not implemented by unity
        if (box.bounds.Contains(position)) {

            // get position within bounding box
            var inside = box.transform.InverseTransformPoint(position);
            inside.x /= box.size.x;
            inside.y /= box.size.y;
            inside.z /= box.size.z;

            // find closest dimension to bounds
            var maxDim = Mathf.Max(inside.x, inside.y, inside.z);
            var minDim = Mathf.Min(inside.x, inside.y, inside.z);
            var dim = Mathf.Max(maxDim, Mathf.Abs(minDim));

            // get bounds in world space
            var boundsMax = box.transform.TransformPoint(box.center + box.size / 2);
            var boundsMin = box.transform.TransformPoint(box.center - box.size / 2);

            // replace closest dimension with bounds
            if (dim == inside.x) {
                
                if (inside.x < 0) {
                    return new Vector3 (boundsMin.x, position.y, position.z);
                } else {
                    return new Vector3 (boundsMax.x, position.y, position.z);
                }

            } else if (dim == inside.y) {
                
                if (inside.y < 0) {
                    return new Vector3(position.x, boundsMin.y, position.z);
                } else {
                    return new Vector3(position.x, boundsMax.y, position.z);
                }

            } else {
                
                if (inside.z < 0) {
                    return new Vector3(position.x, position.y, boundsMin.z);
                } else {
                    return new Vector3(position.x, position.y, boundsMax.z);
                }

            }

        } else {
            return box.ClosestPointOnBounds(position);
        }

    }

}