using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    // PUBLIC PROPERTIES

    public float mass = 1;

    public float internalK = 50;

    public float collisionK = 1000;

    public float damping = 1;

    public float temperature = 300;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public static float gasConstant = 8.314f;

    public List<Force> forces = new List<Force>();

    // GEOMETRY

    private Mesh mesh;

    private float volume;

    // PARTICLES

    private List<Particle> particles = new List<Particle>();

    private Dictionary<int, Particle> vertexMap = new Dictionary<int, Particle>();

    // the floor
    private Plane floor = new Plane(Vector3.up, new Vector3(0, -3, 0));

    // Start is called before the first frame update
    void Start() {
        
        mesh = GetComponent<MeshFilter>().mesh;
        
        volume = CalculateVolume(mesh.bounds);

        // set static values
        Particle.k = this.internalK;
        Particle.damping = this.damping;

        // TEMP set collision bounds
        // floor = new Plane(Vector3.up, transform.InverseTransformVector(Vector3.zero));

        // construct dictionary of same points
        var points = new Dictionary<Vector3, List<int>>();

        for (var i = 0; i < mesh.vertexCount; i++) {

            if (!points.ContainsKey(mesh.vertices[i])) {
                points.Add(mesh.vertices[i], new List<int>());
            } 

            points[mesh.vertices[i]].Add(i);

        }

        // temporary map of vertex -> particle
        // TODO try to remove this ?
        var map = new Dictionary<Vector3, Particle>();

        // add vertices
        foreach (var p in points) {
            var newParticle = new Particle(p.Key, mass, p.Value);
            particles.Add(newParticle);
            map.Add(p.Key, newParticle);
            foreach (var i in p.Value) {
                vertexMap.Add(i, newParticle);
            }
        }

        // add global forces
        forces.Add(new Force((p, state, dt) => p.mass * gravity, particles));
        forces.Add(new Force((p, state, dt) => {
            if (p.CollidesPlane(floor)) {
                return p.GetImpulsePlane(Vector3.up, 1) / dt;
            }
            return Vector3.zero;
        }, particles));

        // add edges
        forces.Add(new Force((p, state, dt) => {
            Vector3 f = Vector3.zero;
            foreach (var other in p.neighbors) {
                var d = GetDisplacement(state[p].position, state[other.Key].position, other.Value);
                f += GetSpringForce(d, state[p].velocity, internalK, damping);
            }
            return f;
        }, particles));

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            var a = map[mesh.vertices[mesh.triangles[i]]];
            var b = map[mesh.vertices[mesh.triangles[i + 1]]];
            var c = map[mesh.vertices[mesh.triangles[i + 2]]];
            a.AddEdge(ref b);
            a.AddEdge(ref c);
            b.AddEdge(ref a);
            b.AddEdge(ref c);
            c.AddEdge(ref a);
            c.AddEdge(ref b);
        }

    }

    // Update is called once per frame
    internal void Update() {
        
        var vertices = mesh.vertices;

        // apply all forces
        foreach (var f in forces) {
            f.Apply();
        }

        // update volume
        var bounds = new Bounds();

        foreach (var p in particles) {
            bounds.Encapsulate(p.state.position);
        }

        volume = CalculateVolume(bounds);

        // get pressure
        var pressure = CalculatePressure();

        // apply to each triangle
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            var a = vertexMap[mesh.triangles[i]];
            var b = vertexMap[mesh.triangles[i + 1]];
            var c = vertexMap[mesh.triangles[i + 2]];
            var n = Vector3.Cross(b.state.position - a.state.position, c.state.position - a.state.position);
            var area = Vector3.Magnitude(n) / 2;
            var p = pressure * n / 2;
            a.force += p;
            b.force += p;
            c.force += p;
        }
        

        // update geometry
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

    public float CalculateVolume(Bounds b) {
        return (b.max.x - b.min.x) * (b.max.y - b.min.y) * (b.max.z - b.min.z);
    }

    public float CalculatePressure() {
        return gasConstant * temperature / volume;
    }

}
