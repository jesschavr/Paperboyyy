using UnityEngine;
using Leap;

public class LeapGrabPaperController : MonoBehaviour
{
    [Header("Ultraleap")]
    [SerializeField] LeapProvider leapProvider;
    [SerializeField] Chirality throwingHand = Chirality.Right;

    [Header("Paper Throw")]
    [SerializeField] PaperThrow paperThrow;

    [Header("Grab Detection")]
    [SerializeField] float closeHandThreshold = 0.75f;
    [SerializeField] float openHandThreshold = 0.40f;

    [Header("Aim Mode")]
    [SerializeField] GameObject accuracyBarRoot;
    [SerializeField] AccuracyBarController accuracyBarController;
    [SerializeField] float slowMotionScale = 0.25f;

    bool handWasClosed = false;
    bool isAiming = false;

    float defaultFixedDeltaTime;

    void Awake()
    {
        defaultFixedDeltaTime = Time.fixedDeltaTime;

        if (accuracyBarRoot != null)
        {
            accuracyBarRoot.SetActive(false);
        }
    }

    void Update()
    {
        if (leapProvider == null || paperThrow == null)
            return;

        Hand hand = leapProvider.CurrentFrame.GetHand(throwingHand);

        bool handIsClosed = false;

        if (hand != null)
        {
            handIsClosed = IsHandClosed(hand.GrabStrength);
        }

        if (!isAiming && handIsClosed)
        {
            BeginAimMode();
        }

        if (isAiming)
        {
            if (!paperThrow.CanAimAtCurrentTarget())
            {
                ForceCancelAimMode();
            }
            else if (!handIsClosed && handWasClosed)
            {
                ReleaseThrow();
            }
            }

        handWasClosed = handIsClosed;
    }

    bool IsHandClosed(float grabStrength)
    {
        if (!handWasClosed)
            return grabStrength >= closeHandThreshold;

        return grabStrength > openHandThreshold;
    }

void BeginAimMode()
{
    if (!paperThrow.CanAimAtCurrentTarget())
    {
        Debug.Log("No valid mailbox in throw range.");
        return;
    }

    isAiming = true;

    paperThrow.BeginAim();
    SetSlowMotion(true);

    if (accuracyBarRoot != null)
    {
        accuracyBarRoot.SetActive(true);
    }

    if (accuracyBarController != null)
    {
        accuracyBarController.BeginBar();
    }
}
    void ReleaseThrow()
    {
        isAiming = false;

        float accuracy = 1f;
        AccuracyBarController.AccuracyZone zone = AccuracyBarController.AccuracyZone.Green;

        if (accuracyBarController != null)
        {
            accuracy = accuracyBarController.CurrentAccuracy;
            zone = accuracyBarController.CurrentZone;
            accuracyBarController.StopBar();
        }

        if (accuracyBarRoot != null)
        {
            accuracyBarRoot.SetActive(false);
        }

        SetSlowMotion(false);

        switch (zone)
        {
            case AccuracyBarController.AccuracyZone.Green:
                Debug.Log("GREEN ZONE -> Great throw");
                break;

            case AccuracyBarController.AccuracyZone.Yellow:
                Debug.Log("YELLOW ZONE -> Medium throw");
                break;

            case AccuracyBarController.AccuracyZone.Red:
                Debug.Log("RED ZONE -> Bad throw");
                break;
        }

        paperThrow.ThrowPreparedPaper(true, accuracy);
    }

    void SetSlowMotion(bool active)
    {
        if (active)
        {
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
        }
    }

    void OnDisable()
    {
        SetSlowMotion(false);
    }

    void OnDestroy()
    {
        SetSlowMotion(false);
    }
    void ForceCancelAimMode()
{
    isAiming = false;

    if (accuracyBarController != null)
    {
        accuracyBarController.StopBar();
    }

    if (accuracyBarRoot != null)
    {
        accuracyBarRoot.SetActive(false);
    }

    SetSlowMotion(false);
    paperThrow.StopAim();

    Debug.Log("Aim cancelled: mailbox is out of range.");
}
}
