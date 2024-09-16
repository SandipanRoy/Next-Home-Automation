#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>
#include "esp_mac.h"

class Connection_ID_Based_Authentication
{
  private:
  int connectionID;
  boolean isDefaultKeyVerified;
  boolean isPasswordVerified;

  public:

  Connection_ID_Based_Authentication()
  {
    
  }
  Connection_ID_Based_Authentication(int connectionID)
  {
    this->connectionID = connectionID;
    isDefaultKeyVerified = false;
    isPasswordVerified = false;
  }

  int getConnectionID()
  {
    return this->connectionID;
  }
  
  void setDefaultKeyVerificationStatus(boolean keyStatus)
  {
    isDefaultKeyVerified = keyStatus;
  }

  void setPasswordVerificationStatus(boolean passwordStatus)
  {
    isPasswordVerified = passwordStatus;
  }

  boolean isConnectionIDAuthenticated()
  {
    if(this->isDefaultKeyVerified && this->isPasswordVerified)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
};



/*For using Bluetooth low energy (BLE) devices,
 * in this case ESP32 in BLE mode, we need to 
 * define Service UUID and Characterictics UUID to identify 
 * a particular service in current device.
 */
#define SERVICE_UUID        "5e45c898-17a3-4104-8726-63fea9e06bb6"
#define CHARACTERISTIC_UUID "11148f5a-152f-4c0d-a9b0-7273e7bb1e88"

/*Using these keys for initial connection handshake*/
#define DEFAULT_KEY "0209b107-0444-4a20-88f0-91d63cd32a24"
#define ECHO_KEY "a6d11766-105c-4391-b116-602858589bc0"

#define DEVICE_NAME "Next Smart Home" //This name will show up in BLE mode advertisement
#define DEVICE_TYPE "8 Switches"
#define MAX_NO_CLIENTS 3

/*Defining some important variables globally*/
BLEServer *server = NULL;
BLECharacteristic *characteristic = NULL;
bool isConnected = false;


uint8_t mac[6]; //Test Variable for adaptor(ESP32) MAC Address
String PASSWORD = "1234"; //Test

Connection_ID_Based_Authentication currentConnectedClients[3];
int connectedConnectionIDArrayCounter = 0;

void removeClientByConnectionId(Connection_ID_Based_Authentication currentConnectedClientsArray[] ,int connectionID)
{
  for(int i=0; i<MAX_NO_CLIENTS; i++)
  {
    if(currentConnectedClientsArray[i].getConnectionID() == connectionID)
    {
      for(int j=i+1; j<MAX_NO_CLIENTS; j++)
      {
        currentConnectedClientsArray[i++] = currentConnectedClientsArray[j];
      }
      connectedConnectionIDArrayCounter -=1;
      break;
    }
  }
}



boolean isConnectionIDAuthenticated(Connection_ID_Based_Authentication currentConnectedClientsArray[],int connectionID, int connectedConnectionIDArrayCounter)
{
  for (int i = 0; i < connectedConnectionIDArrayCounter; i++)
  {
    if(currentConnectedClientsArray[i].getConnectionID() == connectionID && currentConnectedClientsArray[i].isConnectionIDAuthenticated())
    {
      return true;
    }
  }

  return false;
}


/************Callback method for incoming data*****************/
class BLE_Characteristic_Callback:public BLECharacteristicCallbacks
{
  void onWrite(BLECharacteristic *characteristic, esp_ble_gatts_cb_param_t *param)
  {
    
    std::string command = characteristic->getValue();

    /*Get MAC address of client (Sender of message)
    NOTE: From Bluetooth 4.2, devices will show random MAC Address*/

    
    if (command == DEFAULT_KEY)
    {
      characteristic -> setValue(ECHO_KEY);
      characteristic->notify();
    }

    else if(command == "TYPE")
    {
      characteristic -> setValue(DEVICE_TYPE);
      characteristic->notify();
    }

    else if(command == "MAC_ADDRESS")
    {
      esp_efuse_mac_get_default(mac);
      
      // Convert MAC address to string
      char MAC_as_string[8];
      sprintf(MAC_as_string, "%02X:%02X:%02X:%02X:%02X:%02X", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);

      //Send the MAC address as string and notify the connected device
      characteristic -> setValue(MAC_as_string);
      characteristic->notify();
    }


    else if(command.substr(0,8).compare("PASSWORD") == 0)
    {
      if(command.substr(9).compare(PASSWORD.c_str())== 0 )
      {
        characteristic -> setValue("PASSWORD_VERIFIED");
        characteristic->notify();
      }
    else
     {
        characteristic -> setValue("INVALID_PASSWORD");
        characteristic->notify();
     }
      
   }

    else if(command == "STATE_CHANGE")
     {
        if(!isConnectionIDAuthenticated(currentConnectedClients, param->connect.conn_id, connectedConnectionIDArrayCounter))
        {
          characteristic -> setValue("ACCESS_DENIED");
          characteristic->notify();
        }
        else
        {
          for (int i=0; i<command.length();i++)
          {
          Serial.print(command[i]);
          }
        }
        
     }

    else
    {
      for (int i=0; i<command.length();i++)
      {
      Serial.print(command[i]);
      }
    }

  }

};


/*****************Callback Method for Client connect and disconnect*****************/
/*On both connection and disconnection, ESP32 adaptor automatically
 * stops advertising. So, we are making explicit function call, to
 * restart Bluetooth advertisement (Enable for Bluetooth discovery.
 */
class BLE_Server_Callback:public BLEServerCallbacks
{
  
  void onConnect(BLEServer *server, esp_ble_gatts_cb_param_t *param)
  {
    
    isConnected = true; 
    server -> startAdvertising();

    Serial.println(param->connect.conn_id);
    if(connectedConnectionIDArrayCounter < 3)
    {
      currentConnectedClients[connectedConnectionIDArrayCounter++] = Connection_ID_Based_Authentication(param->connect.conn_id);
    }
  }

  void onDisconnect(BLEServer *server, esp_ble_gatts_cb_param_t *param)
  {
    isConnected = false;
    server -> startAdvertising();
    removeClientByConnectionId(currentConnectedClients, param->disconnect.conn_id);
  }
};



void setup() {
  
  Serial.begin(115200);

  

  
  /******Initialize ESP32 in BLE Server Mode*****/

  /****Create BLE Server instance****/
  BLEDevice::init(DEVICE_NAME);
  server = BLEDevice::createServer();
  server->setCallbacks(new BLE_Server_Callback());

  /***Start service using above defined SERVICE_UUID***/
  BLEService *service = server->createService(SERVICE_UUID);

  /***Create Characteristic above defined CHARACTERISTIC_UUID***/
  characteristic = service->createCharacteristic(
                                         CHARACTERISTIC_UUID,
                                         BLECharacteristic::PROPERTY_READ |
                                         BLECharacteristic::PROPERTY_WRITE |
                                         BLECharacteristic::PROPERTY_NOTIFY
                                       );
  characteristic->setCallbacks(new BLE_Characteristic_Callback());   //Add callback class (For incoming data read/write)
  characteristic->addDescriptor(new BLE2902());  //Add Descriptor for notification (Here Characteristic value change notifications)
  service->start();  //Start BLE Service

  /*****Prepare to start BLE Advertising (Enable for Bluetooth Discovery)*****/
  BLEAdvertising *advertiser = BLEDevice::getAdvertising();
  advertiser->addServiceUUID(SERVICE_UUID);
  advertiser->setScanResponse(true);
  advertiser->setMinPreferred(0x06);  // to address iPhone connections issue
  advertiser->setMinPreferred(0x12);
  BLEDevice::startAdvertising();  
}

void loop() {

}
