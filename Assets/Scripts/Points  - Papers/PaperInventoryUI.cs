using System.Collections.Generic;
using UnityEngine;

public class PaperInventoryUI : MonoBehaviour
{
    [SerializeField] Transform iconContainer;
    [SerializeField] GameObject paperIconPrefab;

    readonly List<GameObject> icons = new List<GameObject>();

    void Start()
    {
        if (PaperManager.Instance == null || iconContainer == null || paperIconPrefab == null)
            return;

        BuildIcons(PaperManager.Instance.MaxPapers);

        PaperManager.Instance.OnPaperCountChanged += RefreshIcons;
        RefreshIcons(PaperManager.Instance.CurrentPapers);
    }

    void OnDestroy()
    {
        if (PaperManager.Instance != null)
        {
            PaperManager.Instance.OnPaperCountChanged -= RefreshIcons;
        }
    }

    void BuildIcons(int maxCount)
    {
        for (int i = iconContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(iconContainer.GetChild(i).gameObject);
        }

        icons.Clear();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject icon = Instantiate(paperIconPrefab, iconContainer);
            icon.SetActive(false);
            icons.Add(icon);
        }
    }

    void RefreshIcons(int count)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetActive(i < count);
        }
    }
}