using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SphereTest : MonoBehaviour
{
    public Material _sphereMaterial;

    private void Start()
    {
        Debug.Log("Running");
        List<Vector3> sphereCenters = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector4> uvs = new List<Vector4>();
        
        var points = new List<Vector3> { new Vector3(0, 0.5f, 0),
                                         new Vector3(1, -0.5f, 0),
                                         new Vector3(-1, -0.5f, 0) };

        for (int i = 0; i < points.Count; ++i)
        {
            sphereCenters.Add(points[i]);
            indices.Add(sphereCenters.Count - 1);
            colors.Add(Color.blue);

            // the normal stores the previous position of the point for animation
            // the points can animate between the previous and current position by adjusting the "_Tween" property of the material
            normals.Add(new Vector3(1, 0, 0));

            float radius = 0.5f;

            // UVs encode the following into the XYZW components
            // index for brushing (int) --- each point is assigned an index into a lookup table for brushing
            // current sphere size (float) --- the size of the point
            // is filtered (bool) --- whether the point is filtered out of the visualisation (i.e. hidden)
            // previous radius (float) --- the previous size of the radius. The shader can animated between the current and previous size by adjusting the _TweenSize property

            uvs.Add(new Vector4(i, radius, 0, radius));
        }

        createMesh(sphereCenters.ToArray(), 
                   indices.ToArray(), 
                   colors.ToArray(), 
                   normals.ToArray(), 
                   uvs.ToArray(), 
                   MeshTopology.Points, 
                   _sphereMaterial);
    }

    private static GameObject createMesh(Vector3[] vertices, int[] indices, Color[] colours, Vector3[] normals, Vector4[] uvs, MeshTopology meshTopology, Material material)
    {
        GameObject meshObject = new GameObject();

        MeshTopology mtp = meshTopology;
        // if (mtp == MeshTopology.Lines) mtp = MeshTopology.LineStrip;
        // Create the mesh
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.SetIndices(indices, mtp, 0);
        mesh.normals = normals;
        mesh.colors = colours;
        mesh.SetUVs(0, uvs.ToList());

        mesh.RecalculateBounds();

        if (normals == null || normals.Length == 0)
        {
            mesh.RecalculateNormals();
        }

        // Assign to GameObject
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = material;
        mesh.RecalculateBounds();

        return meshObject;
    }

}
