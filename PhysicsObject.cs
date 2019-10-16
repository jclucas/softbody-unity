using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnMouseDown() {
        // start registering
        Debug.Log("Touched");
    }

    void OnMouseUp() {
        // stop registering
        Debug.Log("Untouched");
    }

}
