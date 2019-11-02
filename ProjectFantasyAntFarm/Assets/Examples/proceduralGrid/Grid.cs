using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    private struct Tri 
    {
        public int[] edgePoints;

        public Tri(int vertA, int vertB, int vertC) 
        {
            edgePoints = new int[3];
            edgePoints[0] = vertA;
            edgePoints[1] = vertB;
            edgePoints[2] = vertC;
        }
    }

    public int xSize;
    public int ySize;

    [SerializeField] private MeshFilter meshFilter;

    private Mesh mesh;
    private Vector3[] verts;
    private void Generate() 
    {
        mesh.name = "Procedural Grid";
        verts = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++) 
        {
            for (int x = 0; x <= xSize;x++, i++) 
            {
                verts[i] = new Vector3(x, y);
            }
        }

        Tri triangle1 = new Tri(0, xSize + 1, 1);
        mesh.vertices = verts;
        mesh.triangles = triangle1.edgePoints;
    }

    private void OnDrawGizmos() 
    {
        if (verts == null) 
        {
            return; 
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < verts.Length; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.1f);
        }
    }

    private void Awake()
    {
        if (meshFilter != null)
        {
            mesh = meshFilter.mesh;
            Generate();
        }
    }

}
