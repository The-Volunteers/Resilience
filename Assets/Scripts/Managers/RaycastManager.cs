using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public static RaycastManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Transform RayCastFormTheCenterOfTheScreen(float rayLenght, LayerMask layerMask)
    {
        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLenght, layerMask))
        {
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.Log("Did not Hit");
        }

        return hit.transform;
    }

    public Transform LinearRay(Vector3 rayPositionPoint, float rayLenght, LayerMask layerMask)
    {
        RaycastHit hit;

        if (Physics.Raycast(rayPositionPoint, rayPositionPoint += Vector3.forward, out hit, rayLenght, layerMask))
        {
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.Log("Did not Hit");
        }

        return hit.transform;
    }

    public Vector3 FindPreviewItemCurrentPosition(float rayDistance, float startVerticalOffset, float objectDistance, LayerMask layermask)
    {
        Vector3 cameraForward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        cameraForward.Normalize();

        Vector3 StartPosition = Camera.main.transform.position + (cameraForward * objectDistance);
        StartPosition.y += startVerticalOffset;

        RaycastHit hit;
        Debug.DrawRay(StartPosition, Vector3.down, Color.yellow, 3f);
        if (Physics.Raycast(StartPosition, Vector3.down, out hit, rayDistance, layermask))
        {
            return hit.point;
        }
        else
        {
            return new Vector3(0f,-100f, 0f);
        }
    }
}
