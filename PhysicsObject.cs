using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    public bool immovable = false;
    
    public float mass = 1;

    public float e = 0.5f;

    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public List<Force> forces = new List<Force>();

    // Start is called before the first frame update
    protected virtual void Start() {
        
    }

    // Update is called once per frame
    protected virtual void Update() {
        
    }

    protected virtual void ApplyForces() {

        foreach (var f in forces) {
            f.Apply();
        }

    }

    protected virtual void DetectCollisions() {
        
        // rigid body collision

    }
    
}
