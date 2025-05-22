/*  CombineChildrenIntoNewMesh.cs
 *  Put this file anywhere under an “Editor” folder.
 *  Right-click a Mesh Filter (or use Tools ▸ Glasses ▸ Combine …)
 *  to merge all child meshes into one Mesh asset,
 *  keeping every sub-mesh and all materials.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CombineChildrenIntoNewMesh : MonoBehaviour
{
    // Context-menu entry (Inspector ► Mesh Filter header)
    [MenuItem("CONTEXT/MeshFilter/Combine Children Into New Mesh")]
    private static void CombineContext(MenuCommand cmd)
    {
        Combine(((MeshFilter)cmd.context).transform);
    }

    // Fallback top-bar entry (Tools ► Glasses ► …)
    [MenuItem("Tools/Glasses/Combine Selected Children (make asset)")]
    private static void CombineFromTools()
    {
        if (Selection.activeTransform == null) return;
        var mf = Selection.activeTransform.GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogWarning("Select an object that has a Mesh Filter.");
            return;
        }
        Combine(mf.transform);
    }

    // ────────────────────────────────────────────────────────────
    private static void Combine(Transform root)
    {
        // 1  Gather parts
        MeshFilter[]  filters   = root.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();

        if (filters.Length < 2)
        {
            Debug.LogWarning("Nothing to combine — this prefab already has a single mesh.");
            return;
        }

        // 2  Build CombineInstance array
        var combines = new CombineInstance[filters.Length];
        for (int i = 0; i < filters.Length; i++)
        {
            combines[i].mesh      = filters[i].sharedMesh;
            combines[i].transform = filters[i].transform.localToWorldMatrix;
        }

        // 3  Create merged mesh (keep sub-meshes, enable 32-bit indices)
        var mergedMesh = new Mesh
        {
            name        = root.name + "_Combined",
            indexFormat = IndexFormat.UInt32
        };
        mergedMesh.CombineMeshes(combines, mergeSubMeshes: false, useMatrices: true);

        // 4  Save mesh asset beside the prefab
        string path = $"Assets/{mergedMesh.name}.asset";
        AssetDatabase.CreateAsset(mergedMesh, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();

        // 5  Ensure root has MeshFilter / MeshRenderer, then assign
        MeshFilter   rootMF = root.GetComponent<MeshFilter>()   ?? root.gameObject.AddComponent<MeshFilter>();
        MeshRenderer rootMR = root.GetComponent<MeshRenderer>() ?? root.gameObject.AddComponent<MeshRenderer>();

        rootMF.sharedMesh = mergedMesh;
        rootMR.sharedMaterials = CollectAllMaterials(filters);

        // 6  Delete original children (leave the root)
        foreach (MeshFilter f in filters)
            if (f.transform != root)
                Object.DestroyImmediate(f.gameObject);

        Debug.Log($"Combined {filters.Length} meshes → {mergedMesh.name}  " +
                  $"({mergedMesh.vertexCount} verts, {mergedMesh.subMeshCount} sub-meshes).",
                  mergedMesh);
    }

    // Helper: gather unique materials in original order
    // ───── helper: collect materials 1-for-1 with sub-meshes ─────
    private static Material[] CollectAllMaterials(MeshFilter[] filters)
    {
    var ordered = new List<Material>();

    foreach (MeshFilter f in filters)
    {
        var mr = f.GetComponent<MeshRenderer>();
        if (mr == null) continue;

        // keep duplicates and order exactly as CombineMeshes sees them
        ordered.AddRange(mr.sharedMaterials);
    }
    return ordered.ToArray();   // length now == mergedMesh.subMeshCount
    }

}
