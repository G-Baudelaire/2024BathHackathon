using UnityEngine;
using System.IO.Ports;
using System;

public class MoveSprite : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM10", 9600);  // The serial port the arduino is connected over
    byte[] buffer = new byte[sizeof(float) + 1];        // Buffer to store received bytes for reconstructing float
    int bytesRead = 0;                                  // Number of bytes read so far
    public Rigidbody rigidBody;                         // 3D Rigidbody works in 2D and 3D so works for all minigames to use 3D
    public float velocityScale = 3f;                    // Scale to adjust how sensitive the controls are
    private SpriteRenderer spriteRenderer;              // Used to flip the character sprite etc.

    void Start()
    {
        // Initialise connection with Arduino
        stream.Open();
        stream.ReadTimeout = 1;

        // Assign the Rigidbody component of the sprite (so we can move it etc.)
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (stream.IsOpen)
        {
            try
            {
                // Read bytes from the serial port (how it communicates with the Arduino)
                while (bytesRead < buffer.Length)
                {
                    // Arduino write bytes directly to serial port so read those bytes
                    int byteRead = stream.ReadByte();
                    if (byteRead != -1)
                    {
                        buffer[bytesRead] = (byte)byteRead;
                        bytesRead++;
                    }
                }

                // If enough bytes are received for pin number and float data
                if (bytesRead == buffer.Length)
                { 
                    int pinNumber = buffer[0];                              // So we know which button is being pressed
                    float floatValue = BitConverter.ToSingle(buffer, 1);    // How much button is being pressed
                    processArduinoInput(pinNumber, floatValue);             // Convert floats to movement
                    bytesRead = 0;                                          // Reset bytesRead for next data set
                }
            }
            catch (Exception)
            {
                // Handle other exceptions here (just ignore them LOL)
            }
        }
    }

    void processArduinoInput(int pinNumber, float valueSentByArduino)
    {
        // Round the float value to 2 decimal places
        float roundedValue = Mathf.Round(valueSentByArduino * 100f) / 100f;

        // Left, Right, Up, Down movement
        if (pinNumber == 0)
        {
            roundedValue = -roundedValue;
            updateHorizontalVelocity(roundedValue);
        }
        if (pinNumber == 1)
        {
            updateHorizontalVelocity(roundedValue);
        }
        if (pinNumber == 2)
        {
            updateVerticalVelocity(roundedValue);
        }
        if (pinNumber == 3)
        {
            roundedValue = -roundedValue;
            updateVerticalVelocity(roundedValue);
        }

        // For Debugging Inputs
        // Debug.Log("Received input from pin " + pinNumber + ": " + roundedValue);
    }

    void updateHorizontalVelocity(float roundedValue)
    {
        Vector3 newVelocity = new Vector3(roundedValue * velocityScale, rigidBody.velocity.y, rigidBody.velocity.z);

        // If going left, flip X
        if (roundedValue < 0)
        {
            spriteRenderer.flipX = true;
        } else
        {
            spriteRenderer.flipX = false;
        }

        rigidBody.velocity = newVelocity;
    }

    void updateVerticalVelocity(float roundedValue)
    {
        Vector3 newVelocity = new Vector3(rigidBody.velocity.x, roundedValue * velocityScale, rigidBody.velocity.z);

        // If going down, flip Y
        if (roundedValue < 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }

        rigidBody.velocity = newVelocity;
    }
}
