using UnityEngine;

public class PortalClipping : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Camera portalCamera;
    [SerializeField] private Transform portalPlane; // Le transform du mesh du portail

    [Header("Configuration du Plan")]
    [Tooltip("Choisir quel axe local du portail représente la normale du plan")]
    [SerializeField] private PortalNormalAxis normalAxis = PortalNormalAxis.Forward;

    [Header("Options")]
    [Tooltip("Offset pour éviter le z-fighting (valeurs négatives = plus agressif)")]
    [SerializeField] private float clipPlaneOffset = 0.07f;

    [Tooltip("Inverser la direction du plan de clipping")]
    [SerializeField] private bool invertClipDirection = false;

    [Header("Debug")]
    [SerializeField] private bool enableClipping = true;
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showGizmos = true;

    public enum PortalNormalAxis
    {
        Forward,    // Z+ (défaut)
        Back,       // Z-
        Up,         // Y+
        Down,       // Y-
        Right,      // X+
        Left        // X-
    }

    void Start()
    {
        if (portalCamera == null)
        {
            portalCamera = GetComponent<Camera>();
        }

        if (showDebugInfo && portalPlane != null)
        {
            Debug.Log($"=== PORTAL ORIENTATION INFO ===");
            Debug.Log($"Portal Rotation: {portalPlane.rotation.eulerAngles}");
            Debug.Log($"Portal Forward: {portalPlane.forward}");
            Debug.Log($"Portal Up: {portalPlane.up}");
            Debug.Log($"Portal Right: {portalPlane.right}");
        }
    }

    void LateUpdate()
    {
        if (!enableClipping)
        {
            // Désactiver le clipping
            if (portalCamera != null)
            {
                portalCamera.ResetProjectionMatrix();
            }
            return;
        }

        if (portalCamera == null || portalPlane == null)
            return;

        ApplyObliqueProjection();
    }

    Vector3 GetPortalNormal()
    {
        Vector3 normal = Vector3.forward;

        switch (normalAxis)
        {
            case PortalNormalAxis.Forward:
                normal = portalPlane.forward;
                break;
            case PortalNormalAxis.Back:
                normal = -portalPlane.forward;
                break;
            case PortalNormalAxis.Up:
                normal = portalPlane.up;
                break;
            case PortalNormalAxis.Down:
                normal = -portalPlane.up;
                break;
            case PortalNormalAxis.Right:
                normal = portalPlane.right;
                break;
            case PortalNormalAxis.Left:
                normal = -portalPlane.right;
                break;
        }

        if (invertClipDirection)
        {
            normal = -normal;
        }

        return normal;
    }

    void ApplyObliqueProjection()
    {
        // Obtenir la normale du plan du portail
        Vector3 normal = GetPortalNormal();

        // Position du plan avec offset
        Vector3 planePosition = portalPlane.position + normal * clipPlaneOffset;

        // Transformer en espace caméra
        Vector3 cameraSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(planePosition);
        Vector3 cameraSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(normal).normalized;

        if (showDebugInfo)
        {
            Debug.Log($"=== PORTAL CLIPPING DEBUG ===");
            Debug.Log($"Normal Axis: {normalAxis}");
            Debug.Log($"World Normal: {normal}");
            Debug.Log($"Plane World Pos: {planePosition}");
            Debug.Log($"Camera Space Pos: {cameraSpacePos}");
            Debug.Log($"Camera Space Normal: {cameraSpaceNormal}");
            Debug.Log($"Camera Space Z: {cameraSpacePos.z} (devrait être négatif)");
            Debug.Log($"Distance camera->plan: {Vector3.Distance(portalCamera.transform.position, planePosition):F2}");
        }

        // Si le plan est derrière la caméra (Z positif en espace caméra), ne pas clipper
        if (cameraSpacePos.z > -0.1f) // Petit threshold pour éviter les problèmes
        {
            if (showDebugInfo)
            {
                Debug.LogWarning(" Le plan de clipping est trop proche ou derrière la caméra!");
            }
            portalCamera.ResetProjectionMatrix();
            return;
        }

        // Créer le plan de clipping en espace caméra
        Vector4 clipPlane = new Vector4(
            cameraSpaceNormal.x,
            cameraSpaceNormal.y,
            cameraSpaceNormal.z,
            -Vector3.Dot(cameraSpacePos, cameraSpaceNormal)
        );

        if (showDebugInfo)
        {
            Debug.Log($"Clip Plane: {clipPlane}");
        }

        // Calculer la matrice de projection oblique
        Matrix4x4 projection = portalCamera.CalculateObliqueMatrix(clipPlane);
        portalCamera.projectionMatrix = projection;
    }

    void OnDisable()
    {
        // Réinitialiser la matrice de projection
        if (portalCamera != null)
        {
            portalCamera.ResetProjectionMatrix();
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || portalPlane == null) return;

        Vector3 normal = GetPortalNormal();
        Vector3 planePos = portalPlane.position + normal * clipPlaneOffset;

        // Dessiner tous les axes du portail pour debug
        Gizmos.color = Color.red;
        Gizmos.DrawRay(portalPlane.position, portalPlane.right * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(portalPlane.position, portalPlane.up * 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(portalPlane.position, portalPlane.forward * 0.5f);

        // Dessiner la normale du plan de clipping (celle utilisée)
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(planePos, normal * 2f);

        // Dessiner la normale inversée pour comparaison
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(planePos, -normal * 1f);

        // Dessiner un carré représentant le plan de clipping
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        DrawPlaneGizmo(planePos, normal);

        // Dessiner une sphère au centre du plan
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(planePos, 0.1f);

        // Dessiner la position de la caméra portail
        if (portalCamera != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(portalCamera.transform.position, 0.15f);
            Gizmos.DrawLine(portalCamera.transform.position, planePos);

            // Dessiner la direction de vue de la caméra
            Gizmos.color = Color.green;
            Gizmos.DrawRay(portalCamera.transform.position, portalCamera.transform.forward * 1.5f);
        }
    }

    void DrawPlaneGizmo(Vector3 position, Vector3 normal)
    {
        // Créer un système de coordonnées local pour le plan
        Vector3 right, up;

        if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.99f)
        {
            right = Vector3.Cross(normal, Vector3.up).normalized;
            up = Vector3.Cross(right, normal).normalized;
        }
        else
        {
            right = Vector3.Cross(normal, Vector3.forward).normalized;
            up = Vector3.Cross(right, normal).normalized;
        }

        float size = 1.5f;
        right *= size;
        up *= size;

        Vector3 topLeft = position - right + up;
        Vector3 topRight = position + right + up;
        Vector3 bottomRight = position + right - up;
        Vector3 bottomLeft = position - right - up;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Dessiner une croix au centre
        Gizmos.DrawLine(position - right * 0.3f, position + right * 0.3f);
        Gizmos.DrawLine(position - up * 0.3f, position + up * 0.3f);
    }
}