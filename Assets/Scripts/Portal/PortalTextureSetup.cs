using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    [SerializeField] private Camera portalCamera;
    [SerializeField] private Material portalMaterial;
    [SerializeField] private MeshRenderer portalMeshRenderer;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool testRedMaterial = false;

    private RenderTexture renderTexture;

    // Start is called before the first frame update
    void Start()
    {
        //if(portalCamera.targetTexture != null)
        //{
        //    portalCamera.targetTexture.Release();
        //}
        //portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        //portalCamera.enabled = true;
        //portalCamera.cullingMask = -1;
        //portalCamera.Render();
        //portalMaterial.SetTexture("_MainTex", portalCamera.targetTexture);
        //portalMaterial.mainTexture = portalCamera.targetTexture;


        SetupPortal();
    }

    void SetupPortal()
    {
        // Vérifications de sécurité
        if (portalCamera == null)
        {
            Debug.LogError(" Portal Camera non assignée !");
            return;
        }

        if (portalMaterial == null)
        {
            Debug.LogError(" Portal Material non assigné !");
            return;
        }

        // IMPORTANT: Vérifier si le mesh renderer est assigné
        if (portalMeshRenderer == null)
        {
            Debug.LogWarning(" Portal Mesh Renderer non assigné. Tentative de recherche automatique...");
            portalMeshRenderer = GetComponent<MeshRenderer>();

            if (portalMeshRenderer == null)
            {
                Debug.LogError(" Aucun MeshRenderer trouvé ! Assignez le portalMeshRenderer dans l'Inspector.");
                return;
            }
        }

        // Vérifier que le material est bien sur le mesh
        if (portalMeshRenderer.sharedMaterial != portalMaterial)
        {
            Debug.LogWarning(" Le matériau du MeshRenderer ne correspond pas. Application du portalMaterial...");
            portalMeshRenderer.material = portalMaterial;
        }

        // Nettoyer l'ancienne RenderTexture si elle existe
        if (portalCamera.targetTexture != null)
        {
            portalCamera.targetTexture.Release();
        }

        // Créer la RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        // Configurer la caméra
        portalCamera.targetTexture = renderTexture;
        portalCamera.enabled = true;
        portalCamera.cullingMask = -1; // Tout afficher
        portalCamera.clearFlags = CameraClearFlags.Skybox;
        portalCamera.depth = -1;

        // Forcer un rendu
        portalCamera.Render();

        // Assigner la texture au matériau
        portalMaterial.mainTexture = renderTexture;

        if (showDebugLogs)
        {
            Debug.Log("=== PORTAL SETUP COMPLETE ===");
            Debug.Log($" RenderTexture: {renderTexture.width}x{renderTexture.height}");
            Debug.Log($" Camera Position: {portalCamera.transform.position}");
            Debug.Log($" Camera Forward: {portalCamera.transform.forward}");
            Debug.Log($" Material Shader: {portalMaterial.shader.name}");
            Debug.Log($" MeshRenderer: {portalMeshRenderer.name}");
            Debug.Log($" MeshRenderer Enabled: {portalMeshRenderer.enabled}");
            Debug.Log($" GameObject Active: {portalMeshRenderer.gameObject.activeInHierarchy}");

            // Vérifier ce que voit la caméra
            int visibleObjects = 0;
            foreach (Renderer r in FindObjectsOfType<Renderer>())
            {
                if (IsVisibleFrom(r, portalCamera))
                {
                    visibleObjects++;
                }
            }
            Debug.Log($" Objets visibles par la portal camera: {visibleObjects}");
        }
    }

    void Update()
    {
        // Test rouge en mode play
        if (testRedMaterial)
        {
            testRedMaterial = false;
            TestRedColor();
        }
    }

    void TestRedColor()
    {
        if (portalMeshRenderer != null)
        {
            Material testMat = new Material(Shader.Find("Unlit/Color"));
            testMat.color = Color.red;
            portalMeshRenderer.material = testMat;
            Debug.Log(" Test rouge appliqué. Si vous ne voyez PAS de rouge, le problème vient du mesh ou de sa visibilité !");
        }
    }

    // Méthode pour vérifier si un objet est visible par une caméra
    bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    void OnDrawGizmos()
    {
        if (portalCamera != null)
        {
            // Dessiner la caméra portail
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(portalCamera.transform.position, 0.5f);
            Gizmos.DrawRay(portalCamera.transform.position, portalCamera.transform.forward * 3f);
        }

        if (portalMeshRenderer != null)
        {
            // Dessiner le portail mesh
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(portalMeshRenderer.bounds.center, portalMeshRenderer.bounds.size);
        }
    }

}
