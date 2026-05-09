using UnityEngine;
using System.IO.Ports;
using System.Globalization;

public class ArduinoReader : MonoBehaviour
{
    public string portName = "/dev/cu.usbmodem141021";
    public int baudRate = 9600;

    [Range(-1f, 1f)]
    public float potentiometerInput = 0f;

    private SerialPort serialPort;

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();
            Debug.Log("Puerto serial abierto correctamente");
        }
        catch (System.Exception e)
        {
            Debug.LogError("No se pudo abrir el puerto: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine().Trim();

                float posicion = float.Parse(data, CultureInfo.InvariantCulture);

                // Arduino manda 0.00 a 2.00
                // Unity lo convierte a -1 a 1
                potentiometerInput = Mathf.Clamp(1f - posicion, -1f, 1f);

                Debug.Log("Arduino: " + data + " | Input: " + potentiometerInput);
            }
            catch
            {
            }
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}