using UnityEditor;
using UnityEngine;
using System.Linq;
using Parabox.CSG; // pb_CSG

public static class MeshBooleanUnion
{
    [MenuItem("Tools/Mesh/Boolean/Union Selected")]
    private static void UnionSelectedMeshes()
    {
        // Get all selected MeshFilters (scene objects only, no prefabs)
        MeshFilter[] filters = Selection.GetFiltered<MeshFilter>(
            SelectionMode.Editable | SelectionMode.ExcludePrefab
        );

        if (filters.Length < 2)
        {
            Debug.LogWarning("Select at least TWO GameObjects with MeshFilters to do a Boolean union.");
            return;
        }

        // Initial union of the first two meshes
        GameObject goA = filters[0].gameObject;
        GameObject goB = filters[1].gameObject;

        Model model = CSG.Union(goA, goB);

        // Temporary working object to union with the rest
        GameObject temp = new GameObject("CSG_Temp_Working");
        var tempMF = temp.AddComponent<MeshFilter>();
        var tempMR = temp.AddComponent<MeshRenderer>();

        tempMF.sharedMesh = model.mesh;
        tempMR.sharedMaterials = model.materials.ToArray();

        temp.transform.position = Vector3.zero;
        temp.transform.rotation = Quaternion.identity;
        temp.transform.localScale = Vector3.one;

        // Union with the remaining selected meshes
        for (int i = 2; i < filters.Length; i++)
        {
            GameObject nextGO = filters[i].gameObject;
            model = CSG.Union(temp, nextGO);

            tempMF.sharedMesh = model.mesh;
            tempMR.sharedMaterials = model.materials.ToArray();
        }

        // Create final result GameObject
        GameObject result = new GameObject("UnionResult");
        var resultMF = result.AddComponent<MeshFilter>();
        var resultMR = result.AddComponent<MeshRenderer>();

        resultMF.sharedMesh = model.mesh;
        resultMR.sharedMaterials = model.materials.ToArray();

        result.transform.position = Vector3.zero;
        result.transform.rotation = Quaternion.identity;
        result.transform.localScale = Vector3.one;

        // Center the pivot to the mesh bounds center (and keep geometry in place)
        CenterPivotOnMesh(result);

        // Optionally disable originals
        foreach (var f in filters)
        {
            f.gameObject.SetActive(false);
        }

        // Ask where to save the mesh asset
        string path = EditorUtility.SaveFilePanelInProject(
            "Save union mesh",
            result.name,
            "asset",
            "Choose where to save the generated mesh asset."
        );

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(resultMF.sharedMesh, path);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(resultMF.sharedMesh);
        }

        // Clean up temp
        Object.DestroyImmediate(temp);

        // Focus the result
        Selection.activeGameObject = result;
        EditorGUIUtility.PingObject(result);
    }

    /// <summary>
    /// Moves mesh vertices so that its local bounds center is at (0,0,0),
    /// and shifts the transform so the mesh stays in the same world position.
    /// Result: pivot is at the center of the mesh.
    /// </summary>
    private static void CenterPivotOnMesh(GameObject go)
    {
        var mf = go.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("CenterPivotOnMesh: GameObject has no MeshFilter/mesh.");
            return;
        }

        Mesh mesh = mf.sharedMesh;

        Vector3 center = mesh.bounds.center;  // local-space center
        Vector3[] verts = mesh.vertices;

        // Recenter vertices around (0,0,0)
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] -= center;
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // Shift transform so world geometry stays in place
        Vector3 worldOffset = go.transform.TransformVector(center);
        go.transform.position += worldOffset;
    }
}
