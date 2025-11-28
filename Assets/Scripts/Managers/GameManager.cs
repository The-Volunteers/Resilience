using System.Collections;
using System.Collections.Generic;
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
    
    [Header("Manager References")]
    [SerializeField] private ViewManager viewManager;

    [Header("Player References")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private Transform playerItemObserverPosition;
    [SerializeField] private Transform playerEquipedItemPosition;
    [SerializeField] private Entity entity;

    [Header("Unity Events")]
    public UnityEvent<Transform> ObserveItem;
    public UnityEvent<Transform> StopObservingItem;
    public UnityEvent<Transform> DropItem;
    public UnityEvent<Transform> ThrowAwayItem;
    public UnityEvent<string> NpcInteraction;

    public delegate void ShakeEffect(Transform transform, float strenght, float duration, int vibrato, float randomness, bool fadeOut); //bool isEffectPlaying
    public ShakeEffect littleShake;

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
        entity.AdvanceStoryEntity();
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
        entity.AdvanceStoryEntity();
        // make a visual effect...
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
