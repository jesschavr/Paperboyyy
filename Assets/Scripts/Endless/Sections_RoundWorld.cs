using UnityEngine;

public class SectionA_RoundWorld : MonoBehaviour
{
    // How far ahead the section starts rising up from below
    [SerializeField] float riseStartDistance = 100f;
    
    // Over how many units the rise completes
    [SerializeField] float riseRange = 150f;
    
    // How far below ground sections sit when far away
    [SerializeField] float hiddenY = -100f;

    Transform playerTransform;

    void Start()
    {
        // Singular - finds ONE object with Player tag
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // How far ahead of the player this section is
        float distanceToPlayer = transform.position.z - playerTransform.position.z;

        // Calculate how much to lerp (0 = hidden below, 1 = fully up)
        float lerpPercentage = 1.0f - ((distanceToPlayer - riseStartDistance) / riseRange);

        // Clamp between 0 and 1 so it never goes below hidden or above ground
        lerpPercentage = Mathf.Clamp01(lerpPercentage);

        // Move the section between hiddenY and 0 based on lerpPercentage
        transform.position = Vector3.Lerp(
            new Vector3(transform.position.x, hiddenY, transform.position.z),
            new Vector3(transform.position.x, 0, transform.position.z),
            lerpPercentage
        );
    }
}