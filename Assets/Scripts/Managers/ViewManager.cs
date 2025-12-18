using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private UIManager UIManager;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject portalColider;
    [SerializeField] private GameObject endPortal;
    [SerializeField] private Transform house;
    [SerializeField] private List<GameObject> ObjectsToClean;
    // Start is called before the first frame update
    void Start()
    {
        //GameManager.Instance.DisplayDialogue.AddListener(UIManager.ShowMessage);
        GameManager.Instance.NpcInteraction.AddListener(UIManager.ShowMessage);
        initializeListOfObjectsToClean();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveObjectInWorld(Transform objectToMove, Vector3 endPosition)
    {
        objectToMove.position = endPosition;
        objectToMove.localRotation = Quaternion.identity;
    }
    public void MoveObjectInWorld(Transform objectToMove, Transform endPosition)
    {
        objectToMove.position = endPosition.position;
        objectToMove.localRotation = endPosition.localRotation;
    }

    public void DropItemToPlacementLocaltion(Transform ItemToMove, Transform placementLocation)
    {
        ItemToMove.position = placementLocation.position;
        ItemToMove.localRotation = placementLocation.localRotation;
    }

    private void initializeListOfObjectsToClean()
    {
        foreach(Transform child in house)
        {
            if(child.tag == "ObjectToClean")
            {
                ObjectsToClean.Add(child.gameObject);
            }
        }
    } 

    public void CleaningHouse()
    {
        if (GameManager.Instance.IsHouseClean) return;
        if(ObjectsToClean.Count <= 0) return;
        for (int i = 0; i < ObjectsToClean.Count; i++)
        {
            ObjectsToClean[i].gameObject.SetActive(false);
        }
        GameManager.Instance.IsHouseClean = true;
    }

    public void DisplayEndPortal()
    {
        portal.SetActive(false);
        portalColider.SetActive(false);
        endPortal.SetActive(true);
    }
    public void GoTpEndPanel()
    {
        UIManager.ActivateEndPanel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
