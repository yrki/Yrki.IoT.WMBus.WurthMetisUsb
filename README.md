# Yrki.IoT.WMBus.WurthMetisUsb
A small Library in C# for reading WMBus-data from the Würth Metis II Usb-stick.
https://www.we-online.com/en/components/products/USB_RADIO_STICK_METERING

## Usage
```csharp

// Set up the port and baudrate
var port = "COM3"; // The port to use.
var baudRate = 9600; // The baudrate to use.

// Create a new instance
var metisII = new MetisII();
metisII.MessageReceived += (sender, message) =>
{
    // Do something with the message
    Console.WriteLine($"Message received: {BitConverter.ToString(message)}");
};

// Initialize the usb module if this is the first time using it. This will take a few seconds.
metisII.IntializeModule(port, baudRate);

// Start the reader
Task.Run(() => metisII.ListenToMessages(port, baudRate, new CancellationToken()));
```
## Work in progress
This library is still a work in progress.

## License
This project is allowed to use for personal use only. If you want to use it for commercial use or in public sector, please contact me at: thomas@yrki.no
