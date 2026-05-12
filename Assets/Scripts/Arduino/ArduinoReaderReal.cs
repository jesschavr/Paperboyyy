using UnityEngine;
using System.IO.Ports;
using System.Globalization;

public class ArduinoReader : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/cu.usbmodem141021";
    public int baudRate = 9600;
    public bool autoUseFirstAvailablePort = true;

    [Header("Read Value")]
    [Range(-1f, 1f)]
    public float potentiometerInput = 0f;

    private SerialPort serialPort;

    void Start()
    {
        string selectedPort = portName;
        string[] availablePorts = SerialPort.GetPortNames();

        if (availablePorts.Length == 0)
        {
            Debug.LogWarning("No serial ports found. Arduino will be skipped.");
            return;
        }

        bool portExists = false;
        foreach (string port in availablePorts)
        {
            if (port == portName)
            {
                portExists = true;
                break;
            }
        }

        if (!portExists)
        {
            if (autoUseFirstAvailablePort)
            {
                selectedPort = availablePorts[0];
                Debug.LogWarning("Configured port not found. Using first available port: " + selectedPort);
            }
            else
            {
                Debug.LogWarning("Configured port not found: " + portName);
                Debug.LogWarning("Available ports: " + string.Join(", ", availablePorts));
                return;
            }
        }

        try
        {
            serialPort = new SerialPort(selectedPort, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();
            Debug.Log("Serial port opened correctly: " + selectedPort);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Could not open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            string data = serialPort.ReadLine().Trim();

            if (float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out float posicion))
            {
                // Arduino sends 0.00 to 2.00
                // Unity converts it to -1 to 1
                potentiometerInput = Mathf.Clamp(1f - posicion, -1f, 1f);

                Debug.Log("Arduino: " + data + " | Input: " + potentiometerInput);
            }
        }
        catch
        {
            // Ignore timeout / incomplete line errors
        }
    }

    void OnApplicationQuit()
    {
        ClosePort();
    }

    void OnDisable()
    {
        ClosePort();
    }

    void ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}