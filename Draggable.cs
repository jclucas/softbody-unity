using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : Object {

    void OnMouseDown() {
        // start registering
        Debug.Log("Touched");
    }

    void OnMouseUp() {
        // stop registering
        Debug.Log("Untouched");
    }

    // Update is called once per frame
    void Update() {
        
    }

}
