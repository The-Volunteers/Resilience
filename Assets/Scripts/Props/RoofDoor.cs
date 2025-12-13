using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofDoor : MonoBehaviour, Interactable
{
    public void Interact()
    {
        GameManager.Instance.GoingBackHome.Invoke();
    }
}
