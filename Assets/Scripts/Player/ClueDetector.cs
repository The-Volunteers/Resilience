using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueDetector : MonoBehaviour
{
    private Transform clueTransform = null;
    //private bool debugEnable;
    private float clueTimer;
    private float clueTimerThreshold = 1.5f;
    private bool isLookingAtTheClue = false;
    //private bool hasTheClueBeenFound = false;
    private Item item = null;

    private void Start()
    {
        clueTimer = clueTimerThreshold;
    }
    private void Update()
    {
        if (isLookingAtTheClue)
        {
            clueTimer -= Time.unscaledDeltaTime;
            //Debug.Log($"The timer is : {clueTimer}");
        }
    }
    public Transform GetTheClueTransform(Transform objectHeld, Item interactedItem)
    {
        Transform clue = null;
        foreach(Transform child in objectHeld)
        {
            if (child.CompareTag("Clue"))
            {
                clue = child;
                break;
            }
        }
        item = interactedItem;
        return clue;
    }

    public void DetectAClue(Transform clue)
    {
        clueTransform = clue;
        //debugEnable = true;
        // need to find the direction from the center of the screen and the clue's local forward direction...
        Vector3 playerEyeDirection = Vector3.Normalize(clue.position - Camera.main.transform.position);
        Vector3 clueLocalForward = -clue.forward;
        float dot = Vector3.Dot(playerEyeDirection, clueLocalForward);
        
        if(dot > 0.9f)
        {
            isLookingAtTheClue = true;
            Debug.Log($"Are you looking at a clue ? {isLookingAtTheClue}");
            if (clueTimer <= 0f && !item.HasTheClueBeenfound)
            {
                Debug.Log("Clue Found !!");
                item.HasTheClueBeenfound = true;
                GameManager.Instance.FoundClueEffect.Invoke();
            }
        }
        else
        {
            isLookingAtTheClue = false;
            Debug.Log($"Are you looking at a clue ? {isLookingAtTheClue}");
            clueTimer = clueTimerThreshold;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (debugEnable)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawRay(Camera.main.transform.position, Vector3.Normalize(clueTransform.position - Camera.main.transform.position) * 1f);
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawRay(clueTransform.position, clueTransform.forward * 1f);
    //    }
    //}
}
