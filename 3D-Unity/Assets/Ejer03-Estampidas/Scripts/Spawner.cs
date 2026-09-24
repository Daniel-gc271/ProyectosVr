using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Configuración del Prefab")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private string spawnerTag = "Spawner";

    [Header("Configuración del Tiempo")]
    [SerializeField] private float startDelay = 0f;     // Cuánto tarda en salir el primer objeto
    [SerializeField] private float spawnInterval = 3f;  // Cada cuántos segundos se repite

    private Transform spawnPoint;

    private void Start()
    {
        // Intentar encontrar el objeto Spawner en la escena por su Tag
        GameObject spawnerObject = GameObject.FindWithTag(spawnerTag);

        if (spawnerObject != null && prefabToSpawn != null)
        {
            spawnPoint = spawnerObject.transform;

            // Llama a la función "SpawnObject" repetidamente
            InvokeRepeating(nameof(SpawnObject), startDelay, spawnInterval);
        }
        else
        {
            Debug.LogWarning("Falta asignar el Prefab o no se encontró ningún objeto con el Tag: " + spawnerTag);
        }
    }

    private void SpawnObject()
    {
        // Instanciar el prefab en la posición y rotación del spawner
        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("BarrilVa");
    }
}
