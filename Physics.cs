﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour {

    PhysicsObject[] objects;

    // Start is called before the first frame update
    void Start() {
        
        objects = GetComponentsInChildren<PhysicsObject>();

    }

    // Update is called once per frame
    void Update() {
        
        // // update each physics object
        // foreach (var child in GetComponentsInChildren<PhysicsObject>()) {
        //     // maybe??
        // }

    }

}
