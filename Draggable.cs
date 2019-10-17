using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : PhysicsObject {

    private bool clicked = false;

    // location in camera space of the object
    Vector3 screenPoint;

    // location of click relative to object
    Vector3 offset;

    void OnMouseDown() {

        // start registering
        clicked = true;
        accel = new Vector3(0, 0, 0);

        // get initial object location
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset =  transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    
    }

    void OnMouseDrag() {
        screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint) + offset;
    }

    void OnMouseUp() {

        // stop registering
        clicked = false;
        accel = gravity;

        // calculate velocity of fling

    }

    // Update is called once per frame
    void Update() {

        // add the user input force
        if (clicked) {
            // equal and opposite to current momentum
            momentum = new Vector3(0, 0, 0);
        }

        base.Update();
        
    }

}
