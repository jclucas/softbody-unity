using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : PhysicsObject {

    private bool clicked = false;
    Vector3 screenPoint;
    Vector3 offset;

    void OnMouseDown() {
        // start registering
        clicked = true;
        accel = new Vector3(0, 0, 0);
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset =  transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,screenPoint.z));
    }

    void OnMouseDrag() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void OnMouseUp() {
        // stop registering
        clicked = false;
        accel = gravity;
    }

    // Update is called once per frame
    void Update() {

        // add the user input force
        if (clicked) {
            velocity = new Vector3(0, 0, 0);
            momentum = new Vector3(0, 0, 0);
        }

        base.Update();
        
    }

}
