using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, Interactable
{  
    public void Interact()
    {
        //if (GameManager.Instance.IsplayerOntheRoof)
        //{
        //    Debug.Log($"Interaction's triggered {GameManager.Instance.IsplayerOntheRoof.ToString()} is {GameManager.Instance.IsplayerOntheRoof}");
        //    GameManager.Instance.GoingBackHome.Invoke();
        //}
        //else
        //{
        //    Debug.Log($"Interaction's triggered {GameManager.Instance.IsplayerOntheRoof.ToString()} is {GameManager.Instance.IsplayerOntheRoof}");
        //    GameManager.Instance.GoingToTheRoof.Invoke();
        //}


        GameManager.Instance.GoingToTheRoof.Invoke();
    }
}
