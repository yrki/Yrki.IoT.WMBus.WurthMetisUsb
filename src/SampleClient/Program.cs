using Yrki.IoT.WMBus.IMSTim871A;

// Set up the port and baudrate
var port = "COM6"; // The port to use.
var baudRate = 576; // The baudrate to use.

// Create a new instance
var metisII = new IMSTim871A();
metisII.MessageReceived += (sender, message) =>
{
    // Do something with the message
    Console.WriteLine($"Message received: {BitConverter.ToString(message)}");
};

// Initialize the usb module if this is the first time using it. This will take a few seconds.
metisII.IntializeModule(port, baudRate);

// Start the reader
Task.Run(() => metisII.ListenToMessages(port, baudRate, new CancellationToken()));

Console.WriteLine("Waiting for messages. Press any key to exit.");
Console.Read();