using UnityEngine;

public class CenterOnCube : MonoBehaviour

{
    [Header("Referencias")]
    public Transform target;           // Asigna aquí el GPS_Cube (su Transform)
    public Transform cameraTransform;  // Asigna aquí la cámara de la CesiumDynamicCamera (o su GameObject)

    [Header("Ajustes de encuadre")]
    public float height = 100f;        // Altura relativa sobre el cubo (mundo Unity)
    public float distance = 200f;      // Distancia hacia atrás para ver el terreno
    public Vector3 extraOffset = Vector3.zero; // Por si quieres un ajuste fino

    public void CenterCamera()
    {
        if (target == null || cameraTransform == null)
        {
            Debug.LogWarning("Faltan referencias: target (GPS_Cube) o cameraTransform (DynamicCamera).");
            return;
        }

        // Offset sencillo: arriba + hacia atrás respecto al cubo
        Vector3 offset = new Vector3(0f, height, -distance) + extraOffset;

        cameraTransform.position = target.position + offset;
        cameraTransform.LookAt(target.position);
    }
}

