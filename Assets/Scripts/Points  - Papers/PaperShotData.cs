using UnityEngine;

public class PaperShotData : MonoBehaviour
{
    public float Accuracy { get; private set; } = 1f;

    public void SetAccuracy(float accuracy)
    {
        Accuracy = Mathf.Clamp01(accuracy);
    }
}