using UnityEngine;

public class PaperPickup : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 90f; // spins so its visible

    void Update()
    {
        // Rotate so it catches the player's eye
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if its the player that touched it
        if (other.CompareTag("Player"))
        {
            PaperManager.Instance.CollectPaper();
            Destroy(gameObject); // pickup disappears
        }
    }
}