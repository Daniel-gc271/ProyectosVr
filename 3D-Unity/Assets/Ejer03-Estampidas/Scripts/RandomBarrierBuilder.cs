using UnityEngine;

public class RandomBarrierBuilder : MonoBehaviour
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
            // 1. Seleccionar variante aleatoria
            int randomIndex = Random.Range(0, obstacleVariants.Length);
            GameObject selectedVariant = obstacleVariants[randomIndex];

            // 2. Definir los ángulos en grados (Vector3)
            Vector3 randomRotationVector = new Vector3(0f, Random.Range(-45f, 45f), 0f);

            // 3. Pasar el Vector3 convertido mediante Quaternion.Euler en el Instantiate
            Instantiate(selectedVariant, point.position,/*point.rotation*/ Quaternion.Euler(randomRotationVector), point);
        }
    }
}