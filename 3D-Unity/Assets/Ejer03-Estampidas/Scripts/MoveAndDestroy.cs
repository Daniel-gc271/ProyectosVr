using UnityEngine;

public class MoveAndDestroy : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;

    [Header("Detección")]
    [SerializeField] private string deleterTag = "Deleter";

    private void Update()
    {
        // Incrementa la posición en el eje Z de forma constante e independiente de los FPS
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto con el que chocó tiene el tag correcto
        if (other.CompareTag(deleterTag))
        {
            Destroy(gameObject);
        }
    }
}
