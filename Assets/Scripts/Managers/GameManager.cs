using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isGamePaused = false;
    public bool IsGamePaused
    {
        get => IsGamePaused = isGamePaused;
        set 
        {
            isGamePaused = value;
            if (isGamePaused)
            {
                PauseGame();
                Debug.Log("The game is paused");
            }
            else
            {
                UnPauseGame();
                Debug.Log("The game is not paused");
            }
        }
    }

    public bool ItemEffectisPlaying {  get; set; } = false;
    public int ObjectIndexToFind {  get; private set; } = 0;
    public bool IsplayerOntheRoof { get; private set; }
    public bool HasTriedToGetOut { get; set; } = false;

    [Header("Manager References")]
    [SerializeField] private ViewManager viewManager;

    [Header("Player References")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private Transform playerItemObserverPosition;
    [SerializeField] private Transform playerEquipedItemPosition;
    [SerializeField] private Entity entity;
    [SerializeField] private Transform playerRoofPosition;
    [SerializeField] private Transform playerHomePosition;

    [Header("Objects References")]
    [SerializeField] private GameObject ladder;

    [Header("Scripts References")]
    [SerializeField] private RippleEffectController rippleEffectController;

    [Header("Unity Events")]
    public UnityEvent<Transform> ObserveItem;
    public UnityEvent<Transform> StopObservingItem;
    public UnityEvent<Transform> DropItem;
    public UnityEvent<Transform> ThrowAwayItem;
    public UnityEvent<Transform> ForceAdvanceStory;
    public UnityEvent<string> NpcInteraction;
    //public UnityEvent<string> DisplayDialogue;
    public UnityEvent GoingToTheRoof;
    public UnityEvent GoingBackHome;
    public UnityEvent FoundClueEffect;
    public UnityEvent OpenExitDoor;
    public UnityEvent EndGame;

    public delegate void ShakeEffect(Transform transform, float strenght, float duration, int vibrato, float randomness, bool fadeOut); //bool isEffectPlaying
    public ShakeEffect littleShake;

    public delegate void AfficherMessage(string message);

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ObserveItem.AddListener(ActivateObserveItemMode);
        StopObservingItem.AddListener(EquipeItem);
        DropItem.AddListener(DeactivateItemPlacementMode);
        ThrowAwayItem.AddListener(DestroyItem);
        ForceAdvanceStory.AddListener(Advance);
        GoingToTheRoof.AddListener(GoToTheRoofLocation);
        GoingBackHome.AddListener(GoToHomeLocation);
        FoundClueEffect.AddListener(rippleEffectController.TriggerRipple);
        IsplayerOntheRoof = false;       
        OpenExitDoor.AddListener(viewManager.DisplayEndPortal);
        EndGame.AddListener(viewManager.GoTpEndPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivateObserveItemMode(Transform transform)
    {
        playerController.IsObservingAnItem = true;
        Debug.Log("Observable Mode Activated !!");
        transform.parent = null;
        viewManager.MoveObjectInWorld(transform, playerItemObserverPosition.position);
        transform.parent = playerItemObserverPosition;
        transform.localPosition = Vector3.zero;
    }

    private void EquipeItem(Transform transform)
    {
        playerController.IsObservingAnItem = false;
        Debug.Log("Observable Mode Deactivated !!");
        transform.parent = null;
        viewManager.MoveObjectInWorld(transform, playerEquipedItemPosition.position);
        transform.parent = playerEquipedItemPosition;
        transform.localPosition = Vector3.zero;
        ActivateItemPlacementMode(transform);
        
        // Timer peut être ajouter une coroutine...
        entity.SendRemarkWhenHoldingObject();
    }

    private void ActivateItemPlacementMode(Transform transform)
    {
        objectPlacer.PreviewObject = transform.gameObject;
        objectPlacer.IsInPlacementMode = true;
    }
    private void DeactivateItemPlacementMode(Transform transform)
    {
        if (!objectPlacer.ValidDropState) { return; }

        transform.parent = null;
        Transform previewItem = objectPlacer.ExitPlacementMode();
        viewManager.DropItemToPlacementLocaltion(transform, previewItem);
        Collider itemCollider = transform.gameObject.GetComponent<Collider>();
        itemCollider.enabled = true;
        transform.gameObject.layer = LayerMask.NameToLayer("Interactable");
        //transform.gameObject.AddComponent<Rigidbody>();
        objectPlacer.IsInPlacementMode = false;
        Destroy(objectPlacer.PreviewObject);
        playerController.ResetInteractedObjectsValues();
       
        // Advance Story...
        if(transform.TryGetComponent<Item>(out Item item))
        {
            if (item.HasTheClueBeenfound)
            {
                entity.AdvanceStoryEntity();
                viewManager.MoveObjectInWorld(entity.transform, entity.actualEntityStoryLocation.entityLocation);
                ObjectIndexToFind++;
            }
        }
        // make a visual effect...
        //viewManager.MoveObjectInWorld(entity.transform, entity.actualEntityStoryLocation.entityLocation.position);
    }

    private void DestroyItem(Transform transform)
    {
        Destroy(transform.gameObject);
        objectPlacer.IsInPlacementMode = false;
        Destroy(objectPlacer.PreviewObject);
        playerController.ResetInteractedObjectsValues();
        // Advance Story...
        if (transform.TryGetComponent<Item>(out Item item))
        {
            if (item.HasTheClueBeenfound)
            {
                entity.AdvanceStoryEntity();
                ActivateLadder();
                viewManager.MoveObjectInWorld(entity.transform, entity.actualEntityStoryLocation.entityLocation);
                ObjectIndexToFind++;
            }
        }
        // make a visual effect...
    }

    private void Advance(Transform transform)
    {
        
        entity.AdvanceStoryEntity();
        ActivateLadder();
        viewManager.MoveObjectInWorld(entity.transform, entity.actualEntityStoryLocation.entityLocation);
        ObjectIndexToFind++;
    }

    private void GoToTheRoofLocation()
    {
        //IsplayerOntheRoof = true;
        playerController.ActivateOrDeActivateCharacterController(false);
        viewManager.MoveObjectInWorld(playerController.transform, playerRoofPosition);
        playerController.ActivateOrDeActivateCharacterController(true);
        Debug.Log($"Teleportation to playerRoofPosition, {playerRoofPosition.position} ! IsplayerOntheRoof is {IsplayerOntheRoof}");
    }
    private void GoToHomeLocation()
    {
        //IsplayerOntheRoof = false;
        playerController.ActivateOrDeActivateCharacterController(false);
        viewManager.MoveObjectInWorld(playerController.transform, playerHomePosition);
        playerController.ActivateOrDeActivateCharacterController(true);
        Debug.Log($"Teleportation to playerHomePosition, {playerHomePosition.position} ! IsplayerOntheRoof is {IsplayerOntheRoof}");
    }

    private void ActivateLadder()
    {
        if(entity.actualEntityStoryLocation.index >= 4)
        {
            ladder.SetActive(true);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
    private void UnPauseGame()
    {
        Time.timeScale = 1f;
    }

}
