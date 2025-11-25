using UnityEngine;

public class PortalCameraDebug : MonoBehaviour
{
    [Header("Références")]
    public Camera portalCamera;
    public Material portalMaterial;
    public GameObject portalQuad; // Le mesh qui affiche le portail

    [Header("Debug Info")]
    public bool showDebugLogs = true;
    public string renderTextureStatus = "";
    public string cameraStatus = "";
    public string materialStatus = "";

    [Header("Actions de Test")]
    [Tooltip("Cliquer pour forcer un rendu de la caméra")]
    public bool forceRender = false;

    [Tooltip("Cliquer pour changer vers Unlit/Texture")]
    public bool testUnlitTexture = false;

    [Tooltip("Cliquer pour afficher du rouge (test matériau)")]
    public bool testRedColor = false;

    [Tooltip("Cliquer pour recréer la RenderTexture")]
    public bool recreateRenderTexture = false;

    private RenderTexture renderTexture;

    void Start()
    {
        if (portalCamera == null)
        {
            Debug.LogError("Portal Camera non assignée !");
            return;
        }

        if (portalMaterial == null)
        {
            Debug.LogError("Portal Material non assigné !");
            return;
        }

        SetupRenderTexture();
    }

    void SetupRenderTexture()
    {
        // Nettoyer l'ancienne RenderTexture si elle existe
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        // Créer la RenderTexture avec des paramètres explicites
        renderTexture = new RenderTexture(
            Screen.width,
            Screen.height,
            24,
            RenderTextureFormat.ARGB32
        );

        renderTexture.Create();

        // Configuration de la caméra portail
        portalCamera.targetTexture = renderTexture;
        portalCamera.enabled = true;
        portalCamera.clearFlags = CameraClearFlags.Skybox;
        portalCamera.cullingMask = -1; // Afficher tout
        portalCamera.depth = -1; // Rendre avant la caméra principale

        // Assigner au matériau
        portalMaterial.mainTexture = renderTexture;

        // Force un rendu immédiat
        portalCamera.Render();

        if (showDebugLogs)
        {
            Debug.Log("=== PORTAL CAMERA DEBUG ===");
            Debug.Log($"RenderTexture créée: {renderTexture.width}x{renderTexture.height}");
            Debug.Log($"Camera enabled: {portalCamera.enabled}");
            Debug.Log($"Camera culling mask: {portalCamera.cullingMask}");
            Debug.Log($"Camera clear flags: {portalCamera.clearFlags}");
            Debug.Log($"Material shader: {portalMaterial.shader.name}");
            Debug.Log($"Camera position: {portalCamera.transform.position}");
            Debug.Log($"Camera rotation: {portalCamera.transform.rotation.eulerAngles}");
        }

        UpdateDebugStatus();
    }

    void Update()
    {
        // Vérifier si la RenderTexture est toujours valide
        if (renderTexture != null && !renderTexture.IsCreated())
        {
            Debug.LogWarning("RenderTexture perdue, recréation...");
            SetupRenderTexture();
        }

        UpdateDebugStatus();
        HandleInspectorButtons();
    }

    void HandleInspectorButtons()
    {
        if (forceRender)
        {
            forceRender = false;
            if (portalCamera != null)
            {
                portalCamera.Render();
                Debug.Log("Rendu forcé de la caméra portail");
            }
        }

        if (testUnlitTexture)
        {
            testUnlitTexture = false;
            if (portalMaterial != null)
            {
                portalMaterial.shader = Shader.Find("Unlit/Texture");
                portalMaterial.mainTexture = renderTexture;
                Debug.Log("Shader changé vers Unlit/Texture");
            }
        }

        if (testRedColor)
        {
            testRedColor = false;
            if (portalMaterial != null)
            {
                portalMaterial.shader = Shader.Find("Unlit/Color");
                portalMaterial.color = Color.red;
                Debug.Log("Test rouge appliqué - si vous voyez du rouge, le matériau fonctionne !");
            }
        }

        if (recreateRenderTexture)
        {
            recreateRenderTexture = false;
            Debug.Log("Recréation de la RenderTexture...");
            SetupRenderTexture();
        }
    }

    void UpdateDebugStatus()
    {
        if (renderTexture != null)
        {
            renderTextureStatus = $"RT: {renderTexture.width}x{renderTexture.height}, Created: {renderTexture.IsCreated()}";
        }
        else
        {
            renderTextureStatus = "RT: NULL";
        }

        if (portalCamera != null)
        {
            cameraStatus = $"Enabled: {portalCamera.enabled}, Culling: {portalCamera.cullingMask}, Pos: {portalCamera.transform.position}";
        }
        else
        {
            cameraStatus = "Camera: NULL";
        }

        if (portalMaterial != null)
        {
            materialStatus = $"Shader: {portalMaterial.shader.name}, Texture: {(portalMaterial.mainTexture != null ? "Assigned" : "NULL")}";
        }
        else
        {
            materialStatus = "Material: NULL";
        }
    }

    void OnDestroy()
    {
        // Nettoyer la RenderTexture
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
            // Dessiner la direction de vue de la caméra portail
            Gizmos.color = Color.green;
            Gizmos.DrawRay(portalCamera.transform.position, portalCamera.transform.forward * 5f);
            Gizmos.DrawWireSphere(portalCamera.transform.position, 0.3f);
        }
    }
}