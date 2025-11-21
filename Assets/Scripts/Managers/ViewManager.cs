using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveObjectInWorld(Transform objectToMove, Vector3 endPosition)
    {
        objectToMove.position = endPosition;
        objectToMove.localRotation = Quaternion.identity;
    }
}
