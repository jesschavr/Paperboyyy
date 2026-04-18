using UnityEngine;
using Leap;

public class LeapPaperThrow : MonoBehaviour
{
    [Header("Ultraleap")]
    [SerializeField] LeapProvider leapProvider;
    [SerializeField] Chirality throwingHand = Chirality.Right;

    [Header("References")]
    [SerializeField] Transform aimReference;
    [SerializeField] Transform throwOrigin;
    [SerializeField] GameObject paperPrefab;
    [SerializeField] LineRenderer trajectoryLine;

    [Header("Throw Settings")]
    [SerializeField] float throwForce = 8f;
    [SerializeField] float minVerticalAngle = -35f;
    [SerializeField] float maxVerticalAngle = 55f;
    [SerializeField] float minHorizontalAngle = -100f;
    [SerializeField] float maxHorizontalAngle = 100f;
    [SerializeField] float aimSmoothing = 10f;

    [Header("Pinch")]
    [SerializeField] float pinchStartThreshold = 0.80f;
    [SerializeField] float pinchReleaseThreshold = 0.50f;

    [Header("Trajectory")]
    [SerializeField] int trajectoryPoints = 20;
    [SerializeField] float timeBetweenPoints = 0.05f;

    float currentVerticalAngle = 10f;
    float currentHorizontalAngle = 0f;

    bool isAiming = false;
    bool wasPinching = false;

    void Update()
    {
        if (leapProvider == null || throwOrigin == null || paperPrefab == null)
            return;

        Hand hand = leapProvider.CurrentFrame.GetHand(throwingHand);

        if (hand == null)
        {
            StopAiming();
            return;
        }

        UpdateAimFromHand(hand);

        bool isPinchingNow = IsPinchingWithHysteresis(hand.PinchStrength);

        if (isPinchingNow)
        {
            isAiming = true;
            ShowTrajectory();
        }
        else
        {
            if (wasPinching && isAiming)
            {
                TryThrow();
            }

            isAiming = false;
            ClearTrajectory();
        }

        wasPinching = isPinchingNow;
    }

    bool IsPinchingWithHysteresis(float pinchStrength)
    {
        if (!wasPinching)
            return pinchStrength >= pinchStartThreshold;

        return pinchStrength > pinchReleaseThreshold;
    }

    void UpdateAimFromHand(Hand hand)
    {
        Transform reference = aimReference != null ? aimReference : transform;

        Vector3 localDirection = reference.InverseTransformDirection(hand.Direction).normalized;

        float targetHorizontalAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float targetVerticalAngle = Mathf.Asin(localDirection.y) * Mathf.Rad2Deg;

        targetHorizontalAngle = Mathf.Clamp(targetHorizontalAngle, minHorizontalAngle, maxHorizontalAngle);
        targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);

        currentHorizontalAngle = Mathf.Lerp(
            currentHorizontalAngle,
            targetHorizontalAngle,
            aimSmoothing * Time.deltaTime
        );

        currentVerticalAngle = Mathf.Lerp(
            currentVerticalAngle,
            targetVerticalAngle,
            aimSmoothing * Time.deltaTime
        );
    }

    Vector3 CalculateThrowVelocity()
    {
        Transform reference = aimReference != null ? aimReference : transform;

        float verticalRad = currentVerticalAngle * Mathf.Deg2Rad;

        Vector3 flatDirection =
            Quaternion.AngleAxis(currentHorizontalAngle, Vector3.up) * reference.forward;

        flatDirection.y = 0f;
        flatDirection.Normalize();

        Vector3 direction =
            flatDirection * Mathf.Cos(verticalRad) +
            Vector3.up * Mathf.Sin(verticalRad);

        return direction.normalized * throwForce;
    }

    void ShowTrajectory()
    {
        if (trajectoryLine == null)
            return;

        Vector3 velocity = CalculateThrowVelocity();

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

    void TryThrow()
    {
        if (PaperManager.Instance != null && !PaperManager.Instance.HasPapers())
        {
            Debug.Log("No papers left!");
            return;
        }

        if (PaperManager.Instance != null)
            PaperManager.Instance.ThrowPaper();

        Vector3 throwVelocity = CalculateThrowVelocity();

        GameObject paper = Instantiate(
            paperPrefab,
            throwOrigin.position,
            Quaternion.LookRotation(throwVelocity)
        );

        Rigidbody rb = paper.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(throwVelocity, ForceMode.Impulse);

        Destroy(paper, 3f);
    }

    void StopAiming()
    {
        isAiming = false;
        wasPinching = false;
        ClearTrajectory();
    }

    void ClearTrajectory()
    {
        if (trajectoryLine != null)
            trajectoryLine.positionCount = 0;
    }
}
