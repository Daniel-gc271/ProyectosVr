using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    public Transform objetivo; // Aquí irá el muñeco
    public Vector3 desfasaje = new Vector3(0, 5, -10); // Distancia detrás del muñeco

    void LateUpdate()
    {
        if (objetivo != null)
        {
            // Actualiza la posición de la cámara sumando el desfasaje
            transform.position = objetivo.position + desfasaje;
            // Hace que la cámara siempre mire al muñeco
            transform.LookAt(objetivo);
        }
    }
}
