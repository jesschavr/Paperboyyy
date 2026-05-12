using UnityEngine;

public class AccuracyBarController : MonoBehaviour
{
    public enum AccuracyZone
    {
        Red,
        Yellow,
        Green
    }

    [Header("UI Reference")]
    [SerializeField] RectTransform pointer;

    [Header("Movement")]
    [SerializeField] float moveRange = 220f;
    [SerializeField] float moveSpeed = 350f;

    [Header("Zones")]
    [SerializeField] float greenThreshold = 0.75f;
    [SerializeField] float yellowThreshold = 0.35f;

    public float CurrentAccuracy { get; private set; } = 1f;
    public AccuracyZone CurrentZone { get; private set; } = AccuracyZone.Green;

    bool isRunning = false;
    float direction = 1f;

    public void BeginBar()
    {
        isRunning = true;
        direction = 1f;

        if (pointer != null)
        {
            Vector2 pos = pointer.anchoredPosition;
            pos.x = -moveRange;
            pointer.anchoredPosition = pos;
        }
    }

    public void StopBar()
    {
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning || pointer == null)
            return;

        Vector2 pos = pointer.anchoredPosition;
        pos.x += direction * moveSpeed * Time.unscaledDeltaTime;

        if (pos.x >= moveRange)
        {
            pos.x = moveRange;
            direction = -1f;
        }
        else if (pos.x <= -moveRange)
        {
            pos.x = -moveRange;
            direction = 1f;
        }

        pointer.anchoredPosition = pos;

        float normalized = Mathf.InverseLerp(-moveRange, moveRange, pos.x);

        CurrentAccuracy = 1f - Mathf.Abs(normalized * 2f - 1f);

        if (CurrentAccuracy >= greenThreshold)
        {
            CurrentZone = AccuracyZone.Green;
        }
        else if (CurrentAccuracy >= yellowThreshold)
        {
            CurrentZone = AccuracyZone.Yellow;
        }
        else
        {
            CurrentZone = AccuracyZone.Red;
        }
    }
}