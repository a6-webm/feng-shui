using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Quad))]
public class NoFurnitureCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Quad quad = GetComponent<Quad>();
        if (quad == null) { return; }

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];
        for (int i = 0; i < 4; i++) {
            vertices[i] = quad.points()[i] + new Vector3(0, -1, 0) - transform.position;
        }
        for (int i = 0; i < 4; i++) {
            vertices[4 + i] = quad.points()[i] + new Vector3(0, 1, 0) - transform.position;
        }
        mesh.vertices = vertices;
        // mesh.uv = new Vector2[0];
        mesh.triangles = new int[] {1, 2, 4, 
                                    2, 3, 4, 
                                    1, 2, 5, 
                                    2, 5, 6, 
                                    2, 3, 6, 
                                    3, 6, 7, 
                                    3, 4, 7, 
                                    4, 7, 0, 
                                    1, 4, 0, 
                                    1, 5, 0, 
                                    5, 6, 0, 
                                    6, 7, 0};

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = false;
        meshCollider.sharedMesh = mesh;
        // Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        // rigidbody.isKinematic = true;
        // rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }
}
