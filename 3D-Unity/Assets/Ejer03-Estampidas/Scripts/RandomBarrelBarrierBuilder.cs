using UnityEngine;

public class RandomBarrelBarrierBuilder : MonoBehaviour
{
    [Header("Puntos de Anclaje")]
    [SerializeField] private Transform[] spawnPositions; // Posiciones hijas en la barrera

    [Header("Pool de Elementos")]
    [SerializeField] private GameObject[] obstacleVariants; // Lista de barriles o bloques a combinar

    private void Awake()
    {
        BuildBarrier();
    }

    private void BuildBarrier()
    {
        if (obstacleVariants.Length == 0 || spawnPositions.Length == 0) return;

        foreach (Transform point in spawnPositions)
        {
            // Seleccionar una variante aleatoria para cada punto
            int randomIndex = Random.Range(0, obstacleVariants.Length);
            GameObject selectedVariant = obstacleVariants[randomIndex];

            // Instanciar como hijo del punto correspondiente
            Instantiate(selectedVariant, point.position, point.rotation, point);
        }
    }
}