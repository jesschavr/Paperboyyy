using UnityEngine;

public class PaperThrow : MonoBehaviour
{
    [Header("Paper Prefab")]
    [SerializeField] GameObject paperPrefab;
    [SerializeField] Transform throwOrigin;

    [Header("Throw Settings")]
    [SerializeField] float throwForce = 8f;
    [SerializeField] float minAngle = -25f;
    [SerializeField] float maxAngle = 60f;

    [Header("Horizontal Aim")]
    [SerializeField] float minHorizontalAngle = -90f;
    [SerializeField] float maxHorizontalAngle = 90f;
    [SerializeField] float mouseSensitivity = 2f;

    [Header("Trajectory Line")]
    [SerializeField] LineRenderer trajectoryLine;
    [SerializeField] int trajectoryPoints = 20;
    [SerializeField] float timeBetweenPoints = 0.05f;

    float currentVerticalAngle = 10f;
    float currentHorizontalAngle = 0f;
    bool isAiming = false;

    void Update()
    {
        /*
         * MODO ANTERIOR / PRUEBA:
         * Este bloque permite apuntar manteniendo F y lanzar al soltar F.
         * Lo dejamos activo por si quieres probar el lanzamiento sin el medidor.
         * No tiene nada específico de Leap Motion, así que no se pierde funcionalidad.
         */

        if (Input.GetKeyDown(KeyCode.F))
        {
            BeginAim();
        }

        if (isAiming)
        {
            UpdateAimWithMouse();
            ShowTrajectory();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            ThrowPreparedPaper(true);
        }

        /*
         * Si más adelante se vuelve a usar Leap Motion, aquí podría conectarse
         * el gesto de la mano para modificar currentVerticalAngle,
         * currentHorizontalAngle o para llamar a ThrowPreparedPaper().
         */
    }

    public void BeginAim()
    {
        isAiming = true;

        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }
    }

    public void StopAim()
    {
        isAiming = false;

        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }
    }

    void UpdateAimWithMouse()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        currentVerticalAngle += mouseY * mouseSensitivity;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minAngle, maxAngle);

        float mouseX = Input.GetAxis("Mouse X");
        currentHorizontalAngle += mouseX * mouseSensitivity;
        currentHorizontalAngle = Mathf.Clamp(currentHorizontalAngle, minHorizontalAngle, maxHorizontalAngle);
    }

    void ShowTrajectory()
    {
        if (trajectoryLine == null || throwOrigin == null)
            return;

        Vector3 velocity = CalculateThrowVelocity();
        trajectoryLine.positionCount = trajectoryPoints;

        Vector3 currentPosition = throwOrigin.position;
        Vector3 currentVelocity = velocity;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            trajectoryLine.SetPosition(i, currentPosition);

            currentPosition += currentVelocity * timeBetweenPoints;
            currentVelocity += Physics.gravity * timeBetweenPoints;

            if (Physics.Raycast(
                currentPosition,
                currentVelocity.normalized,
                out RaycastHit hit,
                currentVelocity.magnitude * timeBetweenPoints))
            {
                trajectoryLine.positionCount = i + 1;
                trajectoryLine.SetPosition(i, hit.point);
                break;
            }
        }
    }

    Vector3 CalculateThrowVelocity()
    {
        float verticalRad = currentVerticalAngle * Mathf.Deg2Rad;

        Vector3 flatDirection = Quaternion.AngleAxis(currentHorizontalAngle, Vector3.up) * transform.forward;
        flatDirection.y = 0f;
        flatDirection.Normalize();

        Vector3 direction =
            flatDirection * Mathf.Cos(verticalRad) +
            Vector3.up * Mathf.Sin(verticalRad);

        return direction.normalized * throwForce;
    }

    public void ThrowPreparedPaper(bool consumePaper)
    {
        if (consumePaper)
        {
            if (!PaperManager.Instance.HasPapers())
            {
                Debug.Log("No papers left!");
                StopAim();
                return;
            }

            PaperManager.Instance.ThrowPaper();
        }

        Vector3 throwVelocity = CalculateThrowVelocity();

        GameObject paper = Instantiate(
            paperPrefab,
            throwOrigin.position,
            Quaternion.LookRotation(throwVelocity)
        );

        Rigidbody rb = paper.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(throwVelocity, ForceMode.Impulse);
        }

        Destroy(paper, 3f);

        StopAim();
    }
}