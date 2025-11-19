using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName;

    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string interaction = "Interaction";
    [SerializeField] private string sprint = "Sprint";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction interactionAction;
    private InputAction sprintAction;

    public Vector2 MovementInput {  get; private set; }
    public Vector2 RotationInput {  get; private set; }
    public bool InteractionTriggered {  get; private set; }
    public bool SprintTriggered {  get; private set; }

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);
        movementAction = playerControls.FindAction(movement);
        rotationAction = playerControls.FindAction(rotation);
        interactionAction = playerControls.FindAction(interaction);
        sprintAction = playerControls.FindAction(sprint);

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        interactionAction.performed += inputInfo => InteractionTriggered = true;
        interactionAction.canceled += inputInfo => InteractionTriggered = false;

        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
