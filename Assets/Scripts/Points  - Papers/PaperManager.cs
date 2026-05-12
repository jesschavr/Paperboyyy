using UnityEngine;
using System;

public class PaperManager : MonoBehaviour
{
    public static PaperManager Instance { get; private set; }

    [Header("Paper Settings")]
    [SerializeField] int startingPapers = 1;
    [SerializeField] int maxPapers = 4;

    public int CurrentPapers => currentPapers;
    public int MaxPapers => maxPapers;
    public int DeliveredPapers => deliveredPapers;

    public event Action<int> OnPaperCountChanged;

    int currentPapers;
    int deliveredPapers;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentPapers = Mathf.Clamp(startingPapers, 0, maxPapers);
        deliveredPapers = 0;

        NotifyPaperCountChanged();
    }

    public bool HasPapers()
    {
        return currentPapers > 0;
    }

    public bool HasSpace()
    {
        return currentPapers < maxPapers;
    }

    public bool CollectPaper()
    {
        if (currentPapers >= maxPapers)
        {
            Debug.Log("Paper inventory is full");
            return false;
        }

        currentPapers++;
        Debug.Log("Collected paper. Total: " + currentPapers);
        NotifyPaperCountChanged();
        return true;
    }

    public bool ThrowPaper()
    {
        if (currentPapers <= 0)
        {
            Debug.Log("No papers left");
            return false;
        }

        currentPapers--;
        Debug.Log("Threw paper. Remaining: " + currentPapers);
        NotifyPaperCountChanged();
        return true;
    }

    public void DeliverPaper()
    {
        deliveredPapers++;
        Debug.Log("Paper delivered. Total delivered: " + deliveredPapers);
    }

    void NotifyPaperCountChanged()
    {
        OnPaperCountChanged?.Invoke(currentPapers);
    }
}