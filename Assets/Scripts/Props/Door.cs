using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    private bool isDoorOpened = false;
    private BoxCollider collider;
    private Coroutine currentCoroutine;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float animationDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        float targetAngle = isDoorOpened ? closeAngle : openAngle;
        if(targetAngle > 0f)
        {
            collider.isTrigger = true;
        }
        currentCoroutine = StartCoroutine(OpenOrCloseDoor(targetAngle));
        isDoorOpened = !isDoorOpened;
        if (!isDoorOpened)
        {
            collider.isTrigger = false;
        }
    }

    IEnumerator OpenOrCloseDoor(float targetAngle)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        while (elapsedTime < animationDuration)
        {
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = targetRotation;
        currentCoroutine = null;
    }
}
