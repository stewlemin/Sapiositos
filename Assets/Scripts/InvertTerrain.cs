using UnityEngine;
using UnityEditor;

public static class TerrainCeilingBuilder
{
    const float Clearance = 30f;  // distance from floor base to the ceiling's flat top
    const int Step = 2;            // 1 = full detail, 2 = half, 4 = quarter (lighter meshes)

    [MenuItem("Tools/Build Cave Ceiling From Selected Terrains")]
    static void Build()
    {
        foreach (var t in Selection.GetFiltered<Terrain>(SelectionMode.Deep))
        {
            var data = t.terrainData;
            int res = data.heightmapResolution;
            float[,] h = data.GetHeights(0, 0, res, res);
            Vector3 size = data.size;
            Vector3 origin = t.transform.position;

            int n = (res - 1) / Step + 1;
            var verts = new Vector3[n * n];
            var uvs = new Vector2[n * n];

            for (int zi = 0; zi < n; zi++)
                for (int xi = 0; xi < n; xi++)
                {
                    int x = Mathf.Min(xi * Step, res - 1);
                    int z = Mathf.Min(zi * Step, res - 1);
                    float px = x / (float)(res - 1);
                    float pz = z / (float)(res - 1);
                    // mirrored: flat parts at the top, hills hang down
                    verts[zi * n + xi] = new Vector3(
                        px * size.x,
                        (2f * size.y + Clearance) - h[z, x] * size.y,
                        pz * size.z);
                    uvs[zi * n + xi] = new Vector2(px, pz);
                }

            var tris = new int[(n - 1) * (n - 1) * 6];
            int k = 0;
            for (int zi = 0; zi < n - 1; zi++)
                for (int xi = 0; xi < n - 1; xi++)
                {
                    int i00 = zi * n + xi, i10 = i00 + 1;
                    int i01 = i00 + n,     i11 = i01 + 1;
                    // reversed winding so the faces point DOWN
                    tris[k++] = i00; tris[k++] = i10; tris[k++] = i01;
                    tris[k++] = i10; tris[k++] = i11; tris[k++] = i01;
                }

            var mesh = new Mesh { name = t.name + "_Ceiling", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + mesh.name + ".asset");
            AssetDatabase.CreateAsset(mesh, path);

            var go = new GameObject(t.name + "_Ceiling");
            Undo.RegisterCreatedObjectUndo(go, "Build Ceiling");
            go.transform.position = origin;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial =
                new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
        }
        AssetDatabase.SaveAssets();
    }
}