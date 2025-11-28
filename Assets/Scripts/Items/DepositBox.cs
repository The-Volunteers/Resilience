using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositBox : MonoBehaviour, Interactable
{
    public Transform itemTransform {  get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        if (itemTransform == null) { return; }
        GameManager.Instance.ThrowAwayItem.Invoke(itemTransform);
    }
}
