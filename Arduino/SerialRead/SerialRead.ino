int incomingByte = 0;

int RedLED = 9;
int YellowLED = 10;
int GreenLED = 11;
 
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(RedLED, OUTPUT);
  pinMode(YellowLED, OUTPUT);
  pinMode(GreenLED, OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  delay(10);
  if(Serial.available() > 0) {
    incomingByte = Serial.read();
      Serial.println(incomingByte);
       switch (incomingByte) {
        case 48: // Fail
          digitalWrite(RedLED, HIGH);
          digitalWrite(GreenLED, LOW); 
          digitalWrite(YellowLED, LOW);
          break;
        case 49: // Pass
          digitalWrite(GreenLED, HIGH);
          digitalWrite(RedLED, LOW); 
          digitalWrite(YellowLED, LOW);
          break;
        case 50: // Running
          digitalWrite(YellowLED, HIGH);
          digitalWrite(RedLED, LOW); 
          digitalWrite(GreenLED, LOW);
          break;
        default: 
          digitalWrite(RedLED, LOW);
          digitalWrite(GreenLED, LOW); 
          digitalWrite(YellowLED, LOW);
        break;
      }
  }
}
