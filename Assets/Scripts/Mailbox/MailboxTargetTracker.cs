using UnityEngine;

public class MailboxTargetTracker : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform aimReference;

    [Header("Distance Limits")]
    [SerializeField] float minThrowDistance = 4f;
    [SerializeField] float maxThrowDistance = 18f;

    [Header("Search")]
    [SerializeField] float maxSideAngle = 85f;

    [Header("Debug")]
    [SerializeField] bool debugDraw = true;

    public MailboxAimPoint CurrentMailbox { get; private set; }
    public Transform CurrentTarget => CurrentMailbox != null ? CurrentMailbox.ThrowTarget : null;
    public float CurrentDistance { get; private set; }

    void Update()
    {
        CurrentMailbox = FindBestMailbox();

        if (CurrentTarget != null)
        {
            CurrentDistance = Vector3.Distance(GetReference().position, CurrentTarget.position);

            if (debugDraw)
            {
                Debug.DrawLine(
                    GetReference().position,
                    CurrentTarget.position,
                    Color.green
                );
            }
        }
        else
        {
            CurrentDistance = -1f;
        }
    }

    MailboxAimPoint FindBestMailbox()
    {
        MailboxAimPoint[] mailboxes = FindObjectsByType<MailboxAimPoint>(FindObjectsSortMode.None);

        MailboxAimPoint bestMailbox = null;
        float bestScore = float.MaxValue;

        Transform reference = GetReference();
        Vector3 origin = reference.position;
        Vector3 forward = reference.forward;

        foreach (MailboxAimPoint mailbox in mailboxes)
        {
            if (mailbox == null || mailbox.ThrowTarget == null)
                continue;

            if (!mailbox.gameObject.activeInHierarchy)
                continue;

            Vector3 toTarget = mailbox.ThrowTarget.position - origin;
            float distance = toTarget.magnitude;

            // Only valid inside this distance window
            if (distance < minThrowDistance || distance > maxThrowDistance)
                continue;

            float angle = Vector3.Angle(forward, toTarget);

            if (angle > maxSideAngle)
                continue;

            float score = distance + angle * 0.25f;

            if (score < bestScore)
            {
                bestScore = score;
                bestMailbox = mailbox;
            }
        }

        return bestMailbox;
    }

    Transform GetReference()
    {
        return aimReference != null ? aimReference : transform;
    }
}