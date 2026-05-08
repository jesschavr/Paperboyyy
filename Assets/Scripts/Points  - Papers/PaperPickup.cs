using UnityEngine;

public class PaperPickup : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 90f; // spins so its visible
    [SerializeField] float floatSpeed = 1.5f; // speed of up/down movement
    [SerializeField] float floatHeight = 0.03f; // height of movement

    [SerializeField] AudioClip pickupSound;
    [SerializeField] float pickupVolume = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotate so it catches the player's eye
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Very subtle floating movement
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if its the player that touched it
        if (other.CompareTag("Player"))
        {
            PaperManager.Instance.CollectPaper();

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);
            }

            Destroy(gameObject); // pickup disappears
        }
    }
}