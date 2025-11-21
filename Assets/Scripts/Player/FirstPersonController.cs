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
    private bool interactionTimerStart = true;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private RaycastManager raycastManager;

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
        InteractionTimerManager();

        if (interactionCooldown > 0f)
        {
            return;
        }

        if (playerInputHandler.InteractionTriggered)
        {
            if (IsObservingAnItem)
            {
                GameManager.Instance.StopObservingItem.Invoke(objectHeld);
                GameManager.Instance.IsGamePaused = false;
                interactionCooldown = 2f;
                interactionTimerStart = true;
                return;
            }
            Transform transform = raycastManager.RayCastFormTheCenterOfTheScreen(interactionDistance, interactableLayer);
            if (transform == null) { return; }
            if (transform.TryGetComponent<Interactable>(out Interactable interactable))
            {
                objectHeld = transform;
                interactable.Interact();
                interactionCooldown = 2f;
                interactionTimerStart = true;
                Debug.Log("Interacting with an object");
            }
            else
            {
                Debug.Log($"{transform.gameObject.name} is not interactable");
            }
            
        }
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
        objectHeld.RotateAround(objectHeld.position, Vector3.up, rotationAmount);
    }
    private void ApplyVerticalRotationToHeldObject(float rotationAmount)
    {
        objectHeld.RotateAround(objectHeld.position, Vector3.left, rotationAmount);
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
}
