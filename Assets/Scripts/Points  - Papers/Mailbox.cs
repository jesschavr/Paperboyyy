using UnityEngine;

public class Mailbox : MonoBehaviour
{
    [Header("Indicator")]
    [SerializeField] GameObject activeIndicator; // arrow/highlight above mailbox
    [SerializeField] float indicatorBobSpeed = 2f;
    [SerializeField] float indicatorBobHeight = 0.2f;

    [Header("Settings")]
    [SerializeField] float activationChance = 0.5f; // 50% chance of needing paper

    bool isActive = false;
    bool delivered = false;
    Vector3 indicatorStartPos;

    void Start()
    {
        // Randomly decide if this mailbox needs a paper
        isActive = Random.value < activationChance;

        // Show or hide indicator based on active state
        if (activeIndicator != null)
        {
            activeIndicator.SetActive(isActive);
            indicatorStartPos = activeIndicator.transform.localPosition;
        }
    }

    void Update()
    {
        // Animate the indicator bobbing up and down
        if (isActive && !delivered && activeIndicator != null)
        {
            float newY = indicatorStartPos.y + 
                Mathf.Sin(Time.time * indicatorBobSpeed) * indicatorBobHeight;

            activeIndicator.transform.localPosition = new Vector3(
                indicatorStartPos.x,
                newY,
                indicatorStartPos.z
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only accept paper if active and not already delivered
        if (other.CompareTag("Paper") && isActive && !delivered)
        {
            delivered = true;
            isActive = false;

            // Hide the indicator
            if (activeIndicator != null)
                activeIndicator.SetActive(false);

            // Destroy the paper
            Destroy(other.gameObject);

            // Tell the score system
            PaperManager.Instance.DeliverPaper();

            Debug.Log("Paper delivered successfully!");
        }
        else if (other.CompareTag("Paper") && !isActive)
        {
            // Wrong mailbox — destroy paper but no point
            Destroy(other.gameObject);
            Debug.Log("This mailbox didnt need a paper!");
        }
    }
}