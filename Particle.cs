using UnityEngine;
using System.Collections.Generic;

public class Particle {

    // List of associated mesh vertices
    List<Vector3> vertices;

    Vector3 velocity;

    Vector3 momentum;

    // Adjacency list
    Dictionary<Particle, float> neighbors;

}