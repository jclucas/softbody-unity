﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // PUBLIC PROPERTIES

    public float mass = 1;

    public float internalK = 50;

    public float collisionK = 1000;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public Vector3[] accel;

    public Vector3[] velocity;

    public Vector3[] momentum;

    // GEOMETRY

    private Mesh mesh;

    private EdgeList edges;

    // Start is called before the first frame update
    void Start() {
        
        mesh = GetComponent<MeshFilter>().mesh.WeldVertices();
        edges = new EdgeList(mesh);
        accel = new Vector3[mesh.vertexCount];
        velocity = new Vector3[mesh.vertexCount];
        momentum = new Vector3[mesh.vertexCount];

        for (var i = 0; i < mesh.vertexCount; i++) {
            accel[i] = Vector3.zero; // gravity;
            velocity[i] = Vector3.zero;
            momentum[i] = Vector3.zero;
        }

        // test: apply initial force to vertex 0
        accel[0] = Vector3.up;

    }

    // Update is called once per frame
    internal void Update() {
        
        var vertices = mesh.vertices;

        for (var i = 0; i < mesh.vertexCount; i++) {

            foreach (int n in edges.GetNeighbors(i)) {
                var F = getSpringForce(edges.GetDisplacement(n, i), internalK) / mass;
                if (Vector3.Magnitude(F) > 0.01) {
                    Debug.Log("Vertex " + n + " applying a force of " +  F + " to " + i);
                    accel[i] += F;
                }
            }

        }

        for (var i = 0; i < mesh.vertexCount; i++) {    
            
            // calculate forces
            var F = accel[i] * mass;

            // integrate position and rotation
            vertices[i] = vertices[i].IntegrateMidpoint(velocity[i], Time.deltaTime);

            // collision detection, get penalty force
            var penalty = DetectCollisions(i);

            // update momentum (integrate forces)
            momentum[i] = momentum[i].IntegrateMidpoint(F + penalty, Time.deltaTime);

            // update velocities
            velocity[i] = momentum[i] / mass;
        
        }

        mesh.vertices = vertices;

    }

    // UTILITY FUNCTIONS

    private Vector3 DetectCollisions(int i) {

        // world space transform
        var world = transform.TransformPoint(mesh.vertices[i]);

        // CHEATING just don't let it go below the floor
        if (world.y < 0) {

            // back up to before collision
            var surface = transform.InverseTransformPoint(new Vector3(world.x, 0, world.z));
            var delta = transform.position - surface;

            // calculate spring force
            return getSpringForce(delta, collisionK);

        } else {
            return Vector3.zero;
        }

    }

    private static Vector3 getSpringForce(Vector3 d, float k) {
        return d * k * -1;
    }

}
