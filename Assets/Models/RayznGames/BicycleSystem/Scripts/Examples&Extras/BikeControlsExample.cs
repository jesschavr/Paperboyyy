using rayzngames;
using UnityEngine;

namespace rayzngames
{
    public class BikeControlsExample : MonoBehaviour
    {
        BicycleVehicle bicycle;
        public bool controllingBike;

        void Awake()
        {
            bicycle = GetComponent<BicycleVehicle>();
        }

        void Update()
        {
            bicycle.verticalInput = Input.GetAxis("Vertical");
            bicycle.horizontalInput = Input.GetAxis("Horizontal");
            BrakingInput();

            bicycle.InControl(controllingBike);

            if (controllingBike)
            {
                bicycle.ConstrainRotation(bicycle.OnGround());
            }
            else
            {
                bicycle.ConstrainRotation(false);
            }
        }

        void BrakingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                bicycle.braking = true;

            if (Input.GetKeyUp(KeyCode.Space))
                bicycle.braking = false;
        }
    }
}