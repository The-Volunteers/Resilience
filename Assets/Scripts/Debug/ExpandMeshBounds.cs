using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ExpandMeshBounds : MonoBehaviour
{
    [SerializeField] private float boundsExpansion = 5f;

    private MeshRenderer meshRenderer;
    private Bounds originalBounds;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalBounds = meshRenderer.bounds;

        ExpandBounds();
    }

    private void ExpandBounds()
    {
        // Créer de nouvelles bounds plus grandes
        Bounds expandedBounds = originalBounds;
        expandedBounds.Expand(boundsExpansion);

        // Appliquer les nouvelles bounds
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.mesh; // Crée une copie du mesh
            mesh.bounds = expandedBounds;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (meshRenderer != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(meshRenderer.bounds.center, meshRenderer.bounds.size);
        }
    }
}