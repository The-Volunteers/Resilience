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
    private Vector3 currentPacementPosition = Vector3.zero;
    private Vector3 outOfScenePosition = new Vector3(0f, -100f, 0f);

    [Header("Raycast Parameters")]
    [SerializeField] private float raycastDistance;
    [SerializeField] private float raycastStartVerticalOffset;
    [SerializeField] private LayerMask itemSurfacePlacerLayer;


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
        }
    }

    private void EnterPlacementMode()
    {
        Debug.Log("Entering Placement Mode !!");
        Quaternion rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        PreviewObject = Instantiate(PreviewObject, outOfScenePosition, rotation);
        PreviewObject.layer = LayerMask.NameToLayer("ProjectedItem");
        Renderer previewobjectRenderer = PreviewObject.GetComponent<Renderer>();
        Material[] materials = previewobjectRenderer.materials;
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i] = previewItemMaterial;
        }
        previewobjectRenderer.materials = materials;

        //GameObject pivot = new GameObject("ItemCopy");
        //pivot.transform.rotation = rotation;
        //pivot.transform.position = outOfScenePosition;
        //GameObject projectedObject = Instantiate(PreviewObject, Vector3.zero, PreviewObject.transform.rotation = Quaternion.identity);
        //projectedObject.transform.position = new Vector3(0f, -100f, 0f);
        //projectedObject.transform.parent = pivot.transform;
        //projectedObjectCopy = pivot;
        //projectedObjectCopy.layer = LayerMask.NameToLayer("ProjectedItem");
        //foreach (Transform child in projectedObjectCopy.transform)
        //{
        //    child.gameObject.layer = LayerMask.NameToLayer("ProjectedItem");
        //}
    }
    public Transform ExitPlacementMode()
    {
        Debug.Log("Placement Mode Deactivated !!");
        return PreviewObject.transform;
    }

    private void UpdateCurrentPlacementPosition()
    {
        currentPacementPosition = RaycastManager.Instance.FindPreviewItemCurrentPosition(raycastDistance, raycastStartVerticalOffset, objectDistanceFromPlayer, itemSurfacePlacerLayer);
        Quaternion rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        PreviewObject.transform.position = currentPacementPosition;
        PreviewObject.transform.rotation = rotation;
        //projectedObjectCopy.transform.position = currentPacementPosition;
        //projectedObjectCopy.transform.rotation = rotation;
    }
}
