using UnityEngine;

public class PaperPickup : MonoBehaviour
{
    [SerializeField] Transform visual;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] Collider pickupTrigger;

    bool collected = false;

    void Update()
    {
        if (visual != null)
        {
            visual.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        Transform playerRoot = other.transform.root;

        if (!playerRoot.CompareTag("Player"))
            return;

        if (PaperManager.Instance == null)
            return;

        bool success = PaperManager.Instance.CollectPaper();

        if (!success)
            return;

        collected = true;

        if (pickupTrigger != null)
            pickupTrigger.enabled = false;

        Destroy(gameObject);
    }
}