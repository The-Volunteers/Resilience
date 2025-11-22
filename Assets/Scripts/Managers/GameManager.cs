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
    
    [Header("Manager References")]
    [SerializeField] private ViewManager viewManager;

    [Header("Player References")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private Transform playerItemObserverPosition;
    [SerializeField] private Transform playerEquipedItemPosition;

    [Header("Unity Events")]
    public UnityEvent<Transform> ObserveItem;
    public UnityEvent<Transform> StopObservingItem;
    public UnityEvent<Transform> DropItem;
    

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
    }

    private void EquipeItem(Transform transform)
    {
        playerController.IsObservingAnItem = false;
        Debug.Log("Observable Mode Deactivated !!");
        transform.parent = null;
        viewManager.MoveObjectInWorld(transform, playerEquipedItemPosition.position);
        transform.parent = playerEquipedItemPosition;
        ActivateItemPlacementMode(transform);
    }

    private void ActivateItemPlacementMode(Transform transform)
    {
        objectPlacer.PreviewObject = transform.gameObject;
        objectPlacer.IsInPlacementMode = true;
    }
    private void DeactivateItemPlacementMode(Transform transform)
    {
        objectPlacer.IsInPlacementMode = false;
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
