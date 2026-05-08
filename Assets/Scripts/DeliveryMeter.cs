using UnityEngine;

public class DeliveryMeter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject meterPanel;
    [SerializeField] RectTransform needle;

    [Header("Movement")]
    [SerializeField] float meterWidth = 400f;
    [SerializeField] float needleSpeed = 500f;
    [SerializeField] float activeTime = 5f;

    private bool isActive = false;
    private bool ignoreSpaceThisFrame = false;

    private float timer = 0f;
    private float needlePosition = 0f;
    private int direction = 1;

    // private ActiveMailbox currentMailbox;

    void Start()
    {
        meterPanel.SetActive(false);
    }

    void Update()
    {
        {
        if (!isActive && Input.GetKeyDown(KeyCode.M))
        {
            StartMeter();
        }

        if (!isActive) return;

        timer -= Time.deltaTime;

        needlePosition += direction * needleSpeed * Time.deltaTime;

        float halfWidth = meterWidth / 2f;

        if (needlePosition >= halfWidth)
        {
            needlePosition = halfWidth;
            direction = -1;
        }
        else if (needlePosition <= -halfWidth)
        {
            needlePosition = -halfWidth;
            direction = 1;
        }

        needle.anchoredPosition = new Vector2(needlePosition, needle.anchoredPosition.y);

        if (ignoreSpaceThisFrame)
        {
            ignoreSpaceThisFrame = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
      
{
    if (PaperManager.Instance.ThrowPaper())
    {
        PaperManager.Instance.DeliverPaper();
        CheckScore();
    }

    EndMeter();
}

        if (timer <= 0f)
        {
            EndMeter();
        }
    }
    }

    public bool StartMeter()
    {
        if (isActive) return false;

        if (!PaperManager.Instance.HasPapers())
        {
            Debug.Log("No papers available.");
            return false;
        }

        meterPanel.SetActive(true);

        isActive = true;
        ignoreSpaceThisFrame = true;
        timer = activeTime;

        needlePosition = -meterWidth / 2f;
        direction = 1;

        needle.anchoredPosition = new Vector2(needlePosition, needle.anchoredPosition.y);

        return true;
    }

    void CheckScore()
    {
        float normalizedPosition = Mathf.Abs(needlePosition) / (meterWidth / 2f);

        int points;

        if (normalizedPosition <= 0.15f)
        {
            points = 500;
        }
        else if (normalizedPosition <= 0.55f)
        {
            points = 100;
        }
        else
        {
            points = 10;
        }

        ScoreManager.Instance.AddScore(points);
    }

    void EndMeter()
    {
        isActive = false;
        // currentMailbox = null;
        meterPanel.SetActive(false);
    }
}