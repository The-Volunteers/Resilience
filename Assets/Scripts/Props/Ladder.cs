using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, Interactable
{  
    public void Interact()
    {
        if (GameManager.Instance.IsplayerOntheRoof)
        {
            GameManager.Instance.GoingBackHome.Invoke();
        }
        else
        {
            GameManager.Instance.GoingToTheRoof.Invoke();
        }
    }
}
