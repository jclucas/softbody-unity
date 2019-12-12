using UnityEngine;

public abstract class CollisionObject : MonoBehaviour {

    public abstract bool Collides(Vector3 point);

    public abstract Vector3 GetCollisionAmount(Vector3 point);

}