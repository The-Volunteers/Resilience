using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Interactable
{

    [SerializeField] private bool canBeAssembled;
    [SerializeField] private bool canBeObserved = true;

    private bool isHeld = false;
    private Collider col;
    public bool CanBeAssembled { get { return canBeAssembled; } }
    public bool CanBeObserved { get { return canBeObserved; } }
    public bool IsHeld
    {
        get { return isHeld; }
        private set 
        {
            isHeld = value;
            if (isHeld)
            {
                col.enabled = false;
                gameObject.layer = LayerMask.NameToLayer("Item");
            }
            else
            {
                col.enabled = true;
                gameObject.layer = LayerMask.NameToLayer("Iteractable");
            }
        }
    }

    private void Start()
    {
        col = transform.GetComponent<Collider>();
    }

    public void Interact()
    {
        IsHeld = true;

        if (canBeObserved)
        {
            LookAt();
            return;
        }
    }

    private void LookAt()
    {
        GameManager.Instance.ObserveItem.Invoke(transform);
        GameManager.Instance.IsGamePaused = true;
    }

    private void Dispose()
    {
        IsHeld = false;
    }
}
