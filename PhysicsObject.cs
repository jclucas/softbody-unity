using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    // PARAMETERS

    public float mass = 1;

    public float k = 50;

    public float damping = 1;

    public float e = 0.5f;

    // constant for all physics objects
    public static Vector3 gravity = new Vector3(0, -9.81f, 0);

    public List<Force> forces = new List<Force>();

    // GEOMETRY

    private Mesh mesh;

    // PARTICLES

    private Particle[] particles;

    private Dictionary<int, Particle> vertexMap = new Dictionary<int, Particle>();

    // the floor
    private Plane floor;

    // Start is called before the first frame update
    void Start() {
        
        mesh = GetComponent<MeshFilter>().mesh;

        // set static values
        Particle.k = this.k;
        Particle.damping = this.damping;

        // set collision bounds
        floor = new Plane(Vector3.up, transform.InverseTransformPoint(Vector3.zero));

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
        var particleList = new List<Particle>();
        
        foreach (var p in points) {
            var newParticle = new Particle(p.Key, mass, p.Value);
            particleList.Add(newParticle);
            map.Add(p.Key, newParticle);
            foreach (var i in p.Value) {
                vertexMap.Add(i, newParticle);
            }
        }

        particles = particleList.ToArray();

        // add global forces
        forces.Add(new Force((p, state, dt) => state[p].mass * gravity, particles));

        // add edges
        forces.Add(new Force((p, state, dt) => {
            Vector3 f = Vector3.zero;
            foreach (var other in state[p].neighbors) {
                var d = GetDisplacement(state[p].position, state[other.Key].position, other.Value);
                f += GetSpringForce(d, state[p].velocity, k, damping);
            }
            return f;
        }, particles));

        // connect all particles for cube
        for (int i = 0; i < particles.Length; i++) {
            for (int j = 0; j < particles.Length; j++) {
                if (i != j) {
                    particles[i].AddEdge(j, ref particles[j]);
                }
            }
        }

    }

    // Update is called once per frame
    internal void Update() {
        
        var vertices = mesh.vertices;

        // apply all forces
        foreach (var f in forces) {
            f.Apply();
        }

        // collision detection
        foreach (var p in particles) {
            if (p.CollidesPlane(floor)) {
                var impulse = p.GetImpulsePlane(Vector3.up, e);
                p.MoveToPlane(floor);
                p.velocity += impulse;
            } 
        }

        // update geometry
        foreach (var p in particles) {
            p.UpdateMesh(ref vertices);
        }

        mesh.vertices = vertices;

    }

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
