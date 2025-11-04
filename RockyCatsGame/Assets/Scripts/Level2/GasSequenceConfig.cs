using UnityEngine;

[System.Serializable]
public class RoundConfig
{
    [Tooltip("Longitud de la secuencia para esta ronda")]
    public int sequenceLength = 4;

    [Tooltip("Intervalo entre emisiones de gas (segundos)")]
    public float stepInterval = 1.0f;
}

[CreateAssetMenu(fileName = "GasSequenceConfig", menuName = "Level2/Gas Sequence Config", order = 0)]
public class GasSequenceConfig : ScriptableObject
{
    [Header("Configuración por Ronda")]
    [Tooltip("Lista de configuraciones para cada ronda. El número de elementos determina el total de rondas.")]
    public RoundConfig[] customRounds = new RoundConfig[8];

    [Header("General")]
    [Tooltip("Número de cráteres en el puzzle (IDs 0..N-1)")]
    public int craterCount = 4;

    [Header("Otros")]
    [Tooltip("Duración de la emisión visual de gas de cada paso")]
    public float gasDuration = 0.5f;

    [Tooltip("Tiempo de espera antes de mostrar el patrón (para leer instrucciones)")]
    public float delayBeforePattern = 2.0f;

    [Tooltip("Semilla opcional para reproducibilidad (0 = aleatoria)")]
    public int randomSeed = 0;

    public int GetSequenceLength(int roundIndex)
    {
        if (customRounds != null && roundIndex >= 0 && roundIndex < customRounds.Length)
        {
            return customRounds[roundIndex].sequenceLength;
        }

        // Valor por defecto si está fuera de rango
        Debug.LogWarning($"[GasSequenceConfig] roundIndex {roundIndex} fuera de rango. Usando valor por defecto.");
        return 4;
    }

    public float GetStepInterval(int roundIndex)
    {
        if (customRounds != null && roundIndex >= 0 && roundIndex < customRounds.Length)
        {
            return customRounds[roundIndex].stepInterval;
        }

        // Valor por defecto si está fuera de rango
        Debug.LogWarning($"[GasSequenceConfig] roundIndex {roundIndex} fuera de rango. Usando valor por defecto.");
        return 1.0f;
    }

    public int GetTotalRounds()
    {
        return (customRounds != null) ? customRounds.Length : 0;
    }
}

