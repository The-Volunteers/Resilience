using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform playerCamera;
    //public Transform portal;
    public Transform mirrorCenter;

    private Vector3 centerPoint;

    [Header("Options")]
    [Tooltip("Inverser uniquement la position sur l'axe Y")]
    public bool mirrorPositionY = true;

    [Tooltip("Inverser uniquement la rotation sur l'axe Y")]
    public bool mirrorRotationY = true;

    [Header("Limites de Rotation")]
    [Tooltip("Limiter l'angle de rotation de la caméra miroir")]
    public bool limitRotationAngle = true;

    [Tooltip("Angle maximum de rotation en degrés (par rapport au point central)")]
    [Range(0f, 180f)]
    public float maxRotationAngle = 90f;

    private void Start()
    {
        centerPoint = mirrorCenter.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 PlayerOffsetFromPortal = playerCamera.position - portal.position;
        //transform.position = portal.position + PlayerOffsetFromPortal;

        //float angularOffsetRotation = Quaternion.Angle(playerCamera.rotation, portal.rotation);

        //Quaternion portalRotationDifference = Quaternion.AngleAxis(angularOffsetRotation, Vector3.up);
        //Vector3 newCameraDirection = portalRotationDifference * playerCamera.forward;
        //transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);

    }

    private void LateUpdate()
    {
        if (playerCamera == null) return;



        centerPoint = mirrorCenter != null ? mirrorCenter.position : Vector3.zero;

        //Vector3 mirroredPosition = CalculateMirroredPosition();
        //transform.position = mirroredPosition;



        Quaternion mirroredRotation = CalculateMirroredRotation();
        transform.rotation = mirroredRotation;
    }

    private Vector3 CalculateMirroredPosition()
    {
        Vector3 mainPos = playerCamera.position;
        Vector3 offset = mainPos - centerPoint;

        if (mirrorPositionY)
        {
            // Inverser la position sur l'axe Y
            offset.y = -offset.y;
        }
        offset.z = -offset.z;

        return centerPoint + offset;
    }

    private Quaternion CalculateMirroredRotation()
    {

        if (!mirrorRotationY)
        {
            // Ne pas inverser la rotation Y - copier directement la rotation de la main caméra
            Quaternion baseRotation = playerCamera.rotation;

            if (limitRotationAngle)
            {
                // Limiter l'angle de rotation par rapport à la direction vers le centre
                return LimitRotationAngle(baseRotation);
            }

            return baseRotation;
        }


        // Refléter la direction de vue par rapport au plan du miroir (axe Y)
        Vector3 mainForward = playerCamera.forward;
        Vector3 reflectedForward = new Vector3(mainForward.x, mainForward.y, -mainForward.z);

        // Refléter aussi le up vector pour maintenir l'orientation correcte
        Vector3 mainUp = playerCamera.up;
        Vector3 reflectedUp = new Vector3(mainUp.x, mainUp.y, -mainUp.z);

        // Créer la rotation miroir
        Quaternion mirroredRotation = Quaternion.LookRotation(reflectedForward, reflectedUp);

        if (limitRotationAngle)
        {
            return LimitRotationAngle(mirroredRotation);
        }

        return mirroredRotation;
    }

    private Quaternion LimitRotationAngle(Quaternion desiredRotation)
    {
        // Calculer la direction vers le point central depuis la caméra miroir
        Vector3 directionToCenter = (centerPoint - transform.position).normalized;

        // Direction de vue actuelle
        Vector3 currentForward = desiredRotation * Vector3.forward;

        // Calculer l'angle entre la direction de vue et la direction vers le centre
        float angle = Vector3.Angle(currentForward, directionToCenter);

        // Si l'angle dépasse la limite, corriger la rotation
        if (angle > maxRotationAngle)
        {
            // Calculer la rotation qui regarde vers le centre
            Quaternion lookAtCenter = Quaternion.LookRotation(directionToCenter, Vector3.up);

            // Interpoler entre regarder le centre et la rotation désirée
            // pour rester à l'angle maximum
            float t = maxRotationAngle / angle;
            return Quaternion.Slerp(lookAtCenter, desiredRotation, t);
        }

        return desiredRotation;
    }
}
