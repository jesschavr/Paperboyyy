using rayzngames;
using UnityEngine;

namespace rayzngames
{
    public class BikeControlsExample : MonoBehaviour
    {
        BicycleVehicle bicycle;
        public bool controllingBike;

        [Header("Arduino")]
        public ArduinoReader arduinoReader;

        void Awake()
        {
            bicycle = GetComponent<BicycleVehicle>();
        }

        void Update()
        {
            bicycle.verticalInput = Input.GetAxis("Vertical");

            float potInput = 0f;
            if (arduinoReader != null)
            {
                potInput = arduinoReader.potentiometerInput;
            }

            bicycle.horizontalInput = Mathf.Clamp(Input.GetAxis("Horizontal") + potInput, -1f, 1f);

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