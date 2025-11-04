using UnityEngine;

[CreateAssetMenu(fileName = "GasSequenceConfig", menuName = "Level2/Gas Sequence Config", order = 0)]
public class GasSequenceConfig : ScriptableObject
{
    [Header("Generación de secuencias")]
    [Tooltip("Número de cráteres en el puzzle (IDs 0..N-1)")]
    public int craterCount = 4;

    [Tooltip("Número total de rondas del puzzle")] public int rounds = 8;

    [Tooltip("Largo de la primera ronda")] public int initialSequenceLength = 4;
    [Tooltip("Incremento de largo por ronda")] public int lengthIncrementPerRound = 1;
    [Tooltip("Máximo largo de la secuencia")] public int maxSequenceLength = 8;

    [Header("Velocidad (más rápido por ronda)")]
    [Tooltip("Intervalo base entre emisiones en la ronda 0")] public float baseStepInterval = 1.2f;
    [Tooltip("Disminución del intervalo por ronda")] public float intervalDecrementPerRound = 0.1f;
    [Tooltip("Intervalo mínimo permitido")] public float minStepInterval = 0.5f;

    [Header("Otros")]
    [Tooltip("Duración de la emisión visual de gas de cada paso")] public float gasDuration = 0.5f;
    [Tooltip("Semilla opcional para reproducibilidad (0 = aleatoria)")] public int randomSeed = 0;

    public int GetSequenceLength(int roundIndex)
    {
        int target = initialSequenceLength + (lengthIncrementPerRound * roundIndex);
        return Mathf.Clamp(target, initialSequenceLength, maxSequenceLength);
    }

    public float GetStepInterval(int roundIndex)
    {
        float t = baseStepInterval - (intervalDecrementPerRound * roundIndex);
        return Mathf.Max(minStepInterval, t);
    }
}

