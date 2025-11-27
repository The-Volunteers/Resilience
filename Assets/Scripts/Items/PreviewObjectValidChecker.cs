using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObjectValidChecker : MonoBehaviour
{
    public bool IsValid { get; private set; } = true;
    private LayerMask collisionLayers;

    public void SetCollisionLayers(params string[] layerNames)
    {
        collisionLayers = LayerMask.GetMask(layerNames);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, collisionLayers))
        {
            IsValid = false;  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, collisionLayers))
        {
            IsValid = true;  
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
