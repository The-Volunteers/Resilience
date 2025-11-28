using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private bool isInPlacementMode;
    public bool IsInPlacementMode 
    {
        get { return isInPlacementMode; }
        set 
        {
            isInPlacementMode = value;
            if (isInPlacementMode)
            {               
                EnterPlacementMode();
            }
            else
            {
                ExitPlacementMode();
            }
        }
    }

    public GameObject PreviewObject {  get; set; }
    private GameObject projectedObjectCopy;

    [Header("Item Preview Parameters")]
    [SerializeField] private Material previewItemMaterial;
    [SerializeField] private float objectDistanceFromPlayer = 5f;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;
    private Vector3 currentPlacementPosition = Vector3.zero;
    private Vector3 outOfScenePosition = new Vector3(0f, -100f, 0f);
    private PreviewObjectValidChecker previewObjectValidChecker;

    [Header("Raycast Parameters")]
    [SerializeField] private float raycastDistance;
    [SerializeField] private float raycastStartVerticalOffset;
    [SerializeField] private LayerMask itemSurfacePlacerLayer;

    public bool ValidDropState { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInPlacementMode)
        {           
            UpdateCurrentPlacementPosition();

            if (previewObjectValidChecker.IsValid)
            {
                SetValidPreviewState();
            }
            else
            {
                SetInvalidPreviewState();
            }
        }
    }

    private void EnterPlacementMode()
    {
        Debug.Log("Entering Placement Mode !!");
        Quaternion rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        PreviewObject = Instantiate(PreviewObject, outOfScenePosition, rotation);
        PreviewObject.layer = LayerMask.NameToLayer("ProjectedItem");
        Renderer previewObjectRenderer = PreviewObject.GetComponent<Renderer>();
        Material[] materials = previewObjectRenderer.materials;
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i] = previewItemMaterial;
        }
        previewObjectRenderer.materials = materials;

        BoxCollider previewCollider = PreviewObject.GetComponent<BoxCollider>();
        previewCollider.enabled = true;
        previewCollider.isTrigger = true;

        Rigidbody rb = PreviewObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        previewObjectValidChecker = PreviewObject.AddComponent<PreviewObjectValidChecker>();
        previewObjectValidChecker.SetCollisionLayers("Default", "Water", "Item", "Interactable");
        //previewObjectValidChecker.IsValid = true;
    }
    public Transform ExitPlacementMode()
    {
        Debug.Log("Placement Mode Deactivated !!");
        ValidDropState = false;
        return PreviewObject.transform;
    }

    private void UpdateCurrentPlacementPosition()
    {
        currentPlacementPosition = RaycastManager.Instance.FindPreviewItemCurrentPosition(raycastDistance, raycastStartVerticalOffset, objectDistanceFromPlayer, itemSurfacePlacerLayer);
        Quaternion rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        PreviewObject.transform.position = currentPlacementPosition;
        PreviewObject.transform.rotation = rotation;

        if(PreviewObject.transform.position == Vector3.zero)
        {
            ValidDropState = false;
        }

        //projectedObjectCopy.transform.position = currentPacementPosition;
        //projectedObjectCopy.transform.rotation = rotation;
    }

    private void SetValidPreviewState()
    {
        previewItemMaterial.color = validColor;
        if(PreviewObject.transform.position != Vector3.zero && PreviewObject != null)
        {
            ValidDropState = true;
        }
    }
    private void SetInvalidPreviewState()
    {
        previewItemMaterial.color = invalidColor;
        ValidDropState = false;
    }    
}
