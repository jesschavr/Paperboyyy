using UnityEngine;

public class ActiveMailbox : MonoBehaviour
{
    [Header("Indicator")]
    [SerializeField] GameObject activeIndicator;
    [SerializeField] float indicatorBobSpeed = 2f;
    [SerializeField] float indicatorBobHeight = 0.2f;

    public bool alreadyUsed { get; private set; } = false;

    private Vector3 indicatorStartPos;

    void Start()
    {
        if (activeIndicator != null)
        {
            activeIndicator.SetActive(true);
            indicatorStartPos = activeIndicator.transform.localPosition;
        }
    }

    void Update()
    {
        if (!alreadyUsed && activeIndicator != null)
        {
            float newY = indicatorStartPos.y + Mathf.Sin(Time.time * indicatorBobSpeed) * indicatorBobHeight;

            activeIndicator.transform.localPosition = new Vector3(
                indicatorStartPos.x,
                newY,
                indicatorStartPos.z
            );
        }
    }

    public void MarkAsDelivered()
    {
        alreadyUsed = true;

        if (activeIndicator != null)
        {
            activeIndicator.SetActive(false);
        }
    }
}
