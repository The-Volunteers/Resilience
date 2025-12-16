using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;
    
    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Gravity Parameters")]
    [SerializeField] private float gravityMultiplier = 1f;
    
    [Header("Interaction Parameters")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactionCooldown = 2f;
    private Transform objectHeld;
    private Vector3 objectHeldCenter = Vector3.zero;
    private Transform clue;
    private Item interactiveItem;
    private bool interactionTimerStart = true;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private ClueDetector clueDetector;
    //[SerializeField] private RaycastManager raycastManager;
    //[SerializeField] private ObjectPlacer objectPlacer;

    private Vector3 currentMovement;
    private float verticalRotation;

    private bool isObservingAnItem;
    public bool IsObservingAnItem
    {
        get { return isObservingAnItem; }
        set {  isObservingAnItem = value; }
    }

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isObservingAnItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleInteraction();      
    }

    private Vector3 CalculateWolrdDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWolrdDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleGravity();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        if (IsObservingAnItem)
        {
            ApplyHorizontalRotationToHeldObject(mouseXRotation);
            ApplyVerticalRotationToHeldObject(mouseYRotation);
            if (clue != null)
            {
                clueDetector.DetectAClue(clue);
            }

        }
        else
        {
            ApplyHorizontalRotation(mouseXRotation);
            ApplyVerticalRotation(mouseYRotation);
        }
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleInteraction()
    {
        CheckInteractableObjects();

        InteractionTimerManager();

        if (interactionCooldown > 0f)
        {
            return;
        }

        if (playerInputHandler.InteractionTriggered)
        {
            if (IsObservingAnItem)
            {
                EquipeItem();
                return;
            }

            if (objectHeld != null)
            {
                Transform depositBoxtransform = RaycastManager.Instance.RayCastFormTheCenterOfTheScreen(interactionDistance, interactableLayer);
                if(depositBoxtransform != null && depositBoxtransform.CompareTag("DepositBox"))
                {
                    CheckIfDepositBox(depositBoxtransform, objectHeld);
                    return;
                }
                DropItem();
                return;
            }

            Transform transform = RaycastManager.Instance.RayCastFormTheCenterOfTheScreen(interactionDistance, interactableLayer);
            if (transform == null) { return; }
            if (transform.TryGetComponent<Interactable>(out Interactable interactable))
            {
                interactable.Interact();               
            }
            else
            {
                Debug.Log($"{transform.gameObject.name} is not interactable");
            }

            if (transform.TryGetComponent<Item>(out Item item))
            {
                CheckIfItsAnItem(transform, item);
            }
            else
            {
                Debug.Log($"{transform.gameObject.name} is not an object");
            }

        }
    }

    private void CheckInteractableObjects()
    {
        if(isObservingAnItem) { return; }
        Transform transform = RaycastManager.Instance.RayCastFormTheCenterOfTheScreen(interactionDistance, interactableLayer);
        if (transform == null) { return; }
        if (transform.CompareTag("Entity")) { return; }
        //Item item = transform.GetComponent<Item>();
        //if (item == null) { return; }
        if(GameManager.Instance.littleShake == null) { return; }
        //if (item.IsShakeEffectIsPlaying) {  return; }
        if (GameManager.Instance.ItemEffectisPlaying) {  return; }
        if(transform.TryGetComponent<Item>(out Item item))
        {
            if(item.CheckIfIsObjectToFind())
            {
                DoShakeEffect(transform);
            }
        }
        if (transform.CompareTag("DepositBox")) 
        {
            DoShakeEffect(transform);
        }
    }

    private void DoShakeEffect(Transform transform)
    {
        GameManager.Instance.littleShake(transform, 0.1f, 1f, 10, 90, true); //item.IsShakeEffectIsPlaying
    }

    private void CheckIfDepositBox(Transform binTransform, Transform objectHeld)
    {
        if (binTransform.TryGetComponent<DepositBox>(out DepositBox bin))
        {
            bin.itemTransform = objectHeld;
            bin.Interact();
        }
    }

    private void EquipeItem()
    {
        GameManager.Instance.StopObservingItem.Invoke(objectHeld);
        GameManager.Instance.IsGamePaused = false;
        interactionCooldown = 1f;
        interactionTimerStart = true;
        if (!interactiveItem.HasTheClueBeenfound) { return; }
        interactiveItem.CanBeObserved = false;
    }

    private void DropItem()
    {
        // object is dropped
        GameManager.Instance.DropItem.Invoke(objectHeld);       
    }

    public void ResetInteractedObjectsValues()
    {
        interactionCooldown = 1f;
        interactionTimerStart = true;
        objectHeld = null;
        clue = null;
        interactiveItem = null;
        objectHeldCenter = Vector3.zero;
    }

    private void CheckIfItsAnItem(Transform transform, Item item)
    {
        if(GameManager.Instance.ObjectIndexToFind != item.IndexOrder) { return; }
        // objectHeld, objectHeldCenter and clue must be reset when the object is dropped
        objectHeld = transform;
        objectHeldCenter = GetCenterOfTheObjectHeld(objectHeld);
        interactionCooldown = 1f;
        interactionTimerStart = true;
        clue = clueDetector.GetTheClueTransform(objectHeld, item);
        interactiveItem = item;
        Debug.Log("Interacting with an object");     
    }
    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0f, rotationAmount, 0f);
    }
    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void ApplyHorizontalRotationToHeldObject(float rotationAmount)
    {
        Vector3 CameraUp = Camera.main.transform.up;
        objectHeld.RotateAround(objectHeldCenter, CameraUp, rotationAmount);
    }
    private void ApplyVerticalRotationToHeldObject(float rotationAmount)
    {
        Vector3 CameraRight = Camera.main.transform.right;
        objectHeld.RotateAround(objectHeldCenter, CameraRight, rotationAmount);
    }

    private void InteractionTimerManager()
    {
        if (interactionTimerStart)
        {
            interactionCooldown -= Time.unscaledDeltaTime;
        }

        if(interactionCooldown < 0f)
        {
            interactionCooldown = 0f;
            interactionTimerStart = false;
        }
    }

    private Vector3 GetCenterOfTheObjectHeld(Transform objectHeld)
    {
        if(objectHeld.TryGetComponent<Renderer>(out Renderer renderer))
        {
            return renderer.bounds.center;
        }
        else
        {
            MeshRenderer meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
            Debug.LogWarning($"{transform.gameObject.name} doesn't have a renderer so a new one has been added but might not be at the right mesh render or might be empty !");
            return meshRenderer.bounds.center;
        }
    }

    public void ActivateOrDeActivateCharacterController(bool myBool)
    {
        if (!myBool)
        {
            characterController.enabled = false;
            return;
        }

        characterController.enabled = true;
    }
}
