
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterWave : MonoBehaviour
{
    [SerializeField, Min(1)] private int segments = 50;
    [SerializeField, Min(0.01f)] private float size = 100f;



    private Mesh myMesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;

    void Start()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        myMesh = new Mesh();
        myMesh.name = "Water Wave Mesh";

        myMesh.indexFormat = IndexFormat.UInt32;

        int side = segments + 1;

        vertices = new Vector3[side * side];
        uvs = new Vector2[side * side];
        triangles = new int[segments * segments * 6];

        float delta = size / segments;

        // Create vertices and UVs
        for (int i = 0; i <= segments; i++)
        {
            for (int j = 0; j <= segments; j++)
            {
                int index = i * side + j;

                float x = i * delta;
                float z = j * delta;

                vertices[index] = new Vector3(x, 0, z);

                uvs[index] = new Vector2(
                    (float)i / segments,
                    (float)j / segments
                );
            }
        }
        int t = 0;

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < segments; j++)
            {
                int index = i * side + j;

                triangles[t++] = index;
                triangles[t++] = index + 1;
                triangles[t++] = index + side;

                triangles[t++] = index + 1;
                triangles[t++] = index + side + 1;
                triangles[t++] = index + side;
            }
        }

        myMesh.vertices = vertices;
        myMesh.uv = uvs;
        myMesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = myMesh;
    }
    

    void OnDestroy()
    {
        if (myMesh != null)
        {
            Destroy(myMesh);
        }
    }
}
