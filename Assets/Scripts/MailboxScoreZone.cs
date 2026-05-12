using UnityEngine;

public class MailboxScoreZone : MonoBehaviour
{
    [SerializeField] int greenPoints = 100;
    [SerializeField] int yellowPoints = 50;
    [SerializeField] int redPoints = 10;

    void OnTriggerEnter(Collider other)
    {
        PaperShotData shotData = other.GetComponent<PaperShotData>();

        if (shotData == null)
            return;

        int points = GetPointsFromAccuracy(shotData.Accuracy);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points);
        }

        Debug.Log("Mailbox hit! +" + points + " points");

        Destroy(other.gameObject);
    }

    int GetPointsFromAccuracy(float accuracy)
    {
        if (accuracy >= 0.75f)
            return greenPoints;

        if (accuracy >= 0.35f)
            return yellowPoints;

        return redPoints;
    }
}
