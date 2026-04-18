using UnityEngine;
using TMPro;

public class PaperManager : MonoBehaviour
{
    public static PaperManager Instance;

    [Header("Paper Settings")]
    [SerializeField] int maxPapers = 10;
    int currentPapers = 0;
    int deliveredPapers = 0;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI paperCountText;
    [SerializeField] TextMeshProUGUI deliveredCountText; // optional score text

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void CollectPaper()
    {
        currentPapers = Mathf.Min(currentPapers + 1, maxPapers);
        UpdateUI();
    }

    public bool ThrowPaper()
    {
        if (currentPapers > 0)
        {
            currentPapers--;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void DeliverPaper()
    {
        deliveredPapers++;
        UpdateUI();
        Debug.Log("Total delivered: " + deliveredPapers);
    }

    public bool HasPapers()
    {
        return currentPapers > 0;
    }

    void UpdateUI()
    {
        if (paperCountText != null)
            paperCountText.text = currentPapers + " / " + maxPapers;

        if (deliveredCountText != null)
            deliveredCountText.text = "Delivered: " + deliveredPapers;
    }
}