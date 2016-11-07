/*
 *  This sketch demonstrates how to set up a simple HTTP-like server.
 *  The server will set a GPIO pin depending on the request
 *    http://server_ip/gpio/0 will set the GPIO2 low,
 *    http://server_ip/gpio/1 will set the GPIO2 high
 *  server_ip is the IP address of the ESP8266 module, will be 
 *  printed to Serial when the module is connected.
 */

#include <ESP8266WiFi.h>

const char* ssid = "TpLinkMK";
const char* password = "M01z_BTS";

int RedLED = 0;
int GreenLED = 2;

float sinVal;
int toneVal;

String buildStatus;

// Create an instance of the server
// specify the port to listen on as an argument
WiFiServer server(80);

void setup() {
  Serial.begin(115200);
  delay(10);

  // prepare GPIO2
  pinMode(RedLED, OUTPUT);
  pinMode(GreenLED, OUTPUT);
  
  digitalWrite(RedLED, 0);
  digitalWrite(GreenLED, 0);
  
  // Connect to WiFi network
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  
  // Start the server
  server.begin();
  Serial.println("Server started");

  // Print the IP address
  Serial.println(WiFi.localIP());
}

void loop() {
  if(buildStatus == "failure")
  {
    alarm();  
  }else {
    noTone(RedLED);
    digitalWrite(RedLED, 0);
    digitalWrite(GreenLED, 1);
  }
  
  // Check if a client has connected
  WiFiClient client = server.available();
  if (!client) {
    return;
  }
  
  // Wait until the client sends some data
  Serial.println("new client");
  while(!client.available()){
    delay(1);
  }
  
  // Read the first line of the request
  String req = client.readStringUntil('\r');
  Serial.println(req);
  client.flush();
  
  // Match the request
  
  if (req.indexOf("/buildStatus/success") != -1)
    buildStatus = "success";
  else if (req.indexOf("/buildStatus/failure") != -1)
    buildStatus = "failure";
  else {
    Serial.println("invalid request -" + req);
    client.stop();
    return;
  }

  if(buildStatus == "success") {
    // Set GPIO according to the request
    digitalWrite(RedLED, 0);
    digitalWrite(GreenLED, 1);
    noTone(RedLED);
  }else if(buildStatus == "failure") {
    digitalWrite(RedLED, 1);
    digitalWrite(GreenLED, 0);
  }else {
    Serial.println("invalid request -" + req);
  }
  client.flush();

  // Prepare the response
  String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nGPIO is now ";
  s += buildStatus;
  s += "</html>\n";

  // Send the response to the client
  client.print(s);
  delay(1);
  Serial.println("Client disonnected");

    
  // The client will actually be disconnected 
  // when the function returns and 'client' object is detroyed
}

void alarm(){
   //digitalWrite(RedLED, 0);
   delay(100);
   //digitalWrite(RedLED, 1);
   delay(100);
   
   for (int x=0; x<100; x++) {
    // convert degrees to radians then obtain sin value
    sinVal = (sin(x*(3.1412/180)));
    // generate a frequency from the sin value
    toneVal = 2000+(int(sinVal*1000));
    tone(RedLED, toneVal);
  }
}

