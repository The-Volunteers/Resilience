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
    // Start is called before the first frame update
    void Start()
    {
        //GameManager.Instance.DisplayDialogue.AddListener(UIManager.ShowMessage);
        GameManager.Instance.NpcInteraction.AddListener(UIManager.ShowMessage);
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
