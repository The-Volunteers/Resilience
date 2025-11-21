using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
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
}
