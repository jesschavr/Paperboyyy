using UnityEngine;

public class PaperThrow : MonoBehaviour
{
    [Header("Paper Prefab")]
    [SerializeField] GameObject paperPrefab;
    [SerializeField] Transform throwOrigin;

    [Header("Targeting")]
    [SerializeField] MailboxTargetTracker targetTracker;
    [SerializeField] float flightTime = 0.8f;
    [SerializeField] float maxMissRadius = 1.2f;

    [Header("Trajectory Line")]
    [SerializeField] LineRenderer trajectoryLine;
    [SerializeField] int trajectoryPoints = 20;
    [SerializeField] float timeBetweenPoints = 0.05f;

    bool isAiming = false;

    void Update()
    {
        if (isAiming)
        {
            ShowTrajectory();
        }
    }

    public void BeginAim()
    {
        isAiming = true;

        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }
    }

    public void StopAim()
    {
        isAiming = false;

        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }
    }

    void ShowTrajectory()
    {
        if (trajectoryLine == null || throwOrigin == null)
            return;

        if (!TryCalculateThrowVelocityToPoint(GetIdealTargetPoint(), out Vector3 velocity))
        {
            trajectoryLine.positionCount = 0;
            return;
        }

        trajectoryLine.positionCount = trajectoryPoints;

        Vector3 currentPosition = throwOrigin.position;
        Vector3 currentVelocity = velocity;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            trajectoryLine.SetPosition(i, currentPosition);

            currentPosition += currentVelocity * timeBetweenPoints;
            currentVelocity += Physics.gravity * timeBetweenPoints;

            if (Physics.Raycast(
                currentPosition,
                currentVelocity.normalized,
                out RaycastHit hit,
                currentVelocity.magnitude * timeBetweenPoints))
            {
                trajectoryLine.positionCount = i + 1;
                trajectoryLine.SetPosition(i, hit.point);
                break;
            }
        }
    }

    Vector3 GetIdealTargetPoint()
    {
        if (targetTracker == null || targetTracker.CurrentTarget == null)
            return Vector3.zero;

        return targetTracker.CurrentTarget.position;
    }

    Vector3 GetAdjustedTargetPoint(float accuracy)
    {
        Vector3 targetPoint = GetIdealTargetPoint();

        float missRadius = Mathf.Lerp(maxMissRadius, 0f, Mathf.Clamp01(accuracy));
        Vector2 offset = Random.insideUnitCircle * missRadius;

        targetPoint += transform.right * offset.x;
        targetPoint += Vector3.up * offset.y;

        return targetPoint;
    }

    bool TryCalculateThrowVelocityToPoint(Vector3 targetPoint, out Vector3 velocity)
    {
        velocity = Vector3.zero;

        if (throwOrigin == null || targetTracker == null || targetTracker.CurrentTarget == null)
            return false;

        Vector3 start = throwOrigin.position;
        Vector3 displacement = targetPoint - start;

        float time = Mathf.Max(0.1f, flightTime);

        Vector3 horizontalVelocity = new Vector3(displacement.x, 0f, displacement.z) / time;
        float verticalVelocity = (displacement.y - 0.5f * Physics.gravity.y * time * time) / time;

        velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        return true;
    }

    public void ThrowPreparedPaper(bool consumePaper)
    {
        ThrowPreparedPaper(consumePaper, 1f);
    }

    public void ThrowPreparedPaper(bool consumePaper, float accuracy)
    {
        if (consumePaper)
        {
            if (!PaperManager.Instance.HasPapers())
            {
                Debug.Log("No papers left!");
                StopAim();
                return;
            }

            PaperManager.Instance.ThrowPaper();
        }

        Vector3 adjustedTarget = GetAdjustedTargetPoint(accuracy);

        if (!TryCalculateThrowVelocityToPoint(adjustedTarget, out Vector3 throwVelocity))
        {
            Debug.Log("No mailbox target found.");
            StopAim();
            return;
        }

        GameObject paper = Instantiate(
            paperPrefab,
            throwOrigin.position,
            Quaternion.LookRotation(throwVelocity)
        );

PaperShotData shotData = paper.GetComponent<PaperShotData>();

if (shotData == null)
{
    shotData = paper.AddComponent<PaperShotData>();
}

shotData.SetAccuracy(accuracy);

        Rigidbody rb = paper.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(throwVelocity, ForceMode.VelocityChange);
        }

        Destroy(paper, 3f);

        StopAim();
    }
    public bool CanAimAtCurrentTarget()
{
    return throwOrigin != null &&
           targetTracker != null &&
           targetTracker.CurrentTarget != null;
}

}