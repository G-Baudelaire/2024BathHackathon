float inputA0, inputA1, inputA2, inputA3;   // Input magnitudes on the pins
const byte PIN_IDENTIFIERS[] = {0x00, 0x01, 0x02, 0x03}; // Identifiers for pins 
const byte PIN_NUMBERS[] = {A0, A1, A2, A3}; // Pin numbers corresponding to the identifiers (left, right, up, down)

void setup() {
  // Initialize serial communication at 9600 baud rate
  Serial.begin(9600);
}

float getInputMagnitude(int readPin) {
  // Get the input magnitude on a given pin
  return abs(5.0 - (analogRead(readPin) * (5.0 / 1023.0)));
}

void sendDataOverSerial(int pinNumber, float inputMagnitude) {
  for (int pinIndex = 0; pinIndex < sizeof(PIN_NUMBERS); pinIndex++) {
    if (PIN_NUMBERS[pinIndex] == pinNumber) {
      byte pinIdentifier = PIN_IDENTIFIERS[pinIndex];
      byte *b = (byte *)&inputMagnitude;
      
      Serial.write(pinIdentifier); // Identifier for pin
      
      for (int i = 0; i < sizeof(float); i++) {
        Serial.write(b[i]); // Data
      }
      
      break; // Exit loop after writing data for the specified pin
    }
  }
}

void loop() {
  // Convert the analog reading to abs diff between 5.0V and input V.
  inputA0 = getInputMagnitude(A0);
  inputA1 = getInputMagnitude(A1);
  inputA2 = getInputMagnitude(A2);
  inputA3 = getInputMagnitude(A3);

  if (inputA0 > 0.05) {
    sendDataOverSerial(A0, inputA0);
  } 
  if (inputA1 > 0.05) {
    sendDataOverSerial(A1, inputA1);
  }
  if (inputA2 > 0.05) {
    sendDataOverSerial(A2, inputA2);
  } 
  if (inputA3 > 0.05) {
    sendDataOverSerial(A3, inputA3);
  }

  // Add a delay to prevent flooding the Serial Monitor with data
  delay(100); // Adjust delay as needed
}
