using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : Object {

    // PUBLIC PROPERTIES

    public float mass = 1;

    public float internalK = 50;

    public float collisionK = 1000;

    public float damping = 1;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public List<Force> forces = new List<Force>();

    // GEOMETRY

    private Mesh mesh;

    // private
    public List<Particle> particles = new List<Particle>();

    // Start is called before the first frame update
    void Start() {
        
        mesh = GetComponent<MeshFilter>().mesh;

        // set static values
        Particle.k = this.internalK;
        Particle.damping = this.damping;

        // construct dictionary of same points
        var points = new Dictionary<Vector3, List<int>>();

        for (var i = 0; i < mesh.vertexCount; i++) {

            if (!points.ContainsKey(mesh.vertices[i])) {
                points.Add(mesh.vertices[i], new List<int>());
            } 

            points[mesh.vertices[i]].Add(i);

        }

        // temporary map of vertex -> particle
        var map = new Dictionary<Vector3, Particle>();

        // add vertices
        foreach (var p in points) {
            var newParticle = new Particle(p.Key, mass, p.Value);
            particles.Add(newParticle);
            map.Add(p.Key, newParticle);
        }

        // add global forces
        forces.Add(new Force((state, dt) => state.mass * gravity, particles));

        // add edges
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            var a = map[mesh.vertices[mesh.triangles[i]]];
            var b = map[mesh.vertices[mesh.triangles[i + 1]]];
            var c = map[mesh.vertices[mesh.triangles[i + 2]]];
            // a.AddEdge(ref b);
            // a.AddEdge(ref c);
            // b.AddEdge(ref a);
            // b.AddEdge(ref c);
            // c.AddEdge(ref a);
            // c.AddEdge(ref b);
            forces.Add(new BinaryForce((ParticleState state, ParticleState other, float dt) => {
                var d = GetDisplacement(state.position, other.position, 2.0f);
                return GetSpringForce(d, state.velocity, kInternal, damping);
            }, a, b));
        }

        Debug.Log("All done :)");

    }

    // Update is called once per frame
    internal void Update() {
        
        var vertices = mesh.vertices;

        // calculate forces
        // for (var i = 0; i < mesh.vertexCount; i++) {
        //     accel[i] = gravity + getEdgeForce(i) / mass;
        // }

        // foreach (var p in particles) {
        //     p.force = gravity * mass;
        //     // foreach (var n in p.neighbors) {
        //     //     p.force += GetSpringForce(GetDisplacement(p.GetPosition(), n.Key.GetPosition(), n.Value),
        //     //             p.state.velocity, internalK, damping);
        //     // }
        // }

        foreach (var f in forces) {
            f.Apply();
        }

        foreach (var p in particles) {
            p.UpdateMesh(ref vertices);
        }

        mesh.vertices = vertices;

    }

    // UTILITY FUNCTIONS

    // private Vector3 DetectCollisions(int i) {

    //     // world space transform
    //     var world = transform.TransformPoint(mesh.vertices[i]);

    //     // CHEATING just don't let it go below the floor
    //     if (world.y < 0) {

    //         // back up to before collision
    //         var surface = transform.InverseTransformPoint(new Vector3(world.x, 0, world.z));
    //         var delta = mesh.vertices[i] - surface;

    //         // calculate spring force
    //         return getSpringForce(delta, velocity[i], collisionK, damping);

    //     } else {
    //         return Vector3.zero;
    //     }

    // }

    // private Vector3 getEdgeForce(int i) {

    //     Vector3 force = Vector3.zero;

    //     foreach (int n in edges.GetNeighbors(i)) {

    //         var F = getSpringForce(edges.GetDisplacement(i, n), velocity[n] - velocity[i], internalK, damping) / mass;
    //         if (Vector3.Magnitude(F) > 0.01) {
                
    //             Debug.Log("Vertex " + i + " applying a force of " +  F + " to " + n);
    //             force += F;
    //         }
    //     }

    //     return force;

    // }

    // k: spring constant
    // c: damping constant
    private static Vector3 GetSpringForce(Vector3 d, Vector3 v, float k, float c) {
        return (d * k * -1) - (c * v);
    }

    public Vector3 GetDisplacement(Vector3 a, Vector3 b, float expected) {
        var actual = a - b;
        var displacement = Vector3.Magnitude(actual) - expected;
        return displacement * (actual / Vector3.Magnitude(actual));
    }

}
