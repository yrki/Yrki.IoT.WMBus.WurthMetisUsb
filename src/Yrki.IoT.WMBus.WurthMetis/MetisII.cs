using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Yrki.IoT.WMBus.WurthMetis
{
    public class MetisII
    {
        public event EventHandler<byte[]>? MessageReceived;

        /// <summary>
        /// Setting up the USB stick with preferred parameters. Should be done once for each stick.
        /// Is setting up the the module to receive mode for C1 and T1-mode.
        /// Is setting up the the module to include RSSI in the output.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baudrate"></param>
        public void IntializeModule(string port, int baudrate)
        {
            var serialPort = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
            serialPort.Open();

            //Send factory CMD_FACTORYRESET_REQ
            serialPort.Write(new byte[] { 0xFF, 0x11, 0x00, 0xEE }, 0, 4);
            Thread.Sleep(500);

            // Send CMD_RESET_REQ
            serialPort.Write(new byte[] { 0xFF, 0x05, 0x00, 0xFA }, 0, 4);
            Thread.Sleep(500);

            // // Set the UART_CMD_OUT_ENABLE = 1, using CMD_SET_REQ: 0xFF0903050101F0
            serialPort.Write(new byte[] { 0xFF, 0x09, 0x03, 0x05, 0x01, 0x01, 0xF0 }, 0, 7);
            Thread.Sleep(500);

            // Set the RSSI_Enable = 1, using CMD_SET_REQ: 0xFF0903450101B0
            serialPort.Write(new byte[] { 0xFF, 0x09, 0x03, 0x45, 0x01, 0x01, 0xB0 }, 0, 7);
            Thread.Sleep(500);

            // // Set the UART baud rate to 115200 baud, using CMD_SETUARTSPEED_REQ: 0xFF100107E9
            // serialPort.Write(new byte[] { 0xFF, 0x10, 0x01, 0x07, 0xE9 }, 0, 5);
            // Thread.Sleep(1000);
            // serialPort.Close();
            // serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
            // serialPort.Open();

            // Set Mode_Preselect to the default receive mode you want to achieve.
            // for C2_T2_other (RX only mode) use CMD_SET_REQ: 0xFF0903460109BB
            serialPort.Write(new byte[] { 0xFF, 0x09, 0x03, 0x46, 0x01, 0x09, 0xBB }, 0, 7);
            Thread.Sleep(500);

            // Perform a CMD_RESET_REQ: 0xFF0500FA
            serialPort.Write(new byte[] { 0xFF, 0x05, 0x00, 0xFA }, 0, 4);
            Thread.Sleep(500);
        }
        
        public Task ListenToMessages(string port, int baudrate, CancellationToken cancellationToken)
        {
            var serialPort = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
            serialPort.Open();

            var charactersLeft = 0;
            List<byte> message = new List<byte>();
            
            while(true)
            {                
                var buffer = new byte[serialPort.BytesToRead];
                if(buffer.Length > 1)
                {
                    serialPort.Read(buffer, 0, buffer.Length);

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if(charactersLeft <= 0 && (i < buffer.Length - 1) && buffer[i] == 0xFF && buffer[i+1] == 0x03)
                        {
                            charactersLeft = buffer[i+2];
            
                            if(message.Count > 0)
                            {
                                MessageReceived?.Invoke(this, message.ToArray());
                                message = new List<byte>();
                            }
                        }
         
                        message.Add(buffer[i]);
                        charactersLeft--;
                    }
                }
                Thread.Sleep(100);

                if(cancellationToken.IsCancellationRequested)
                {
                    serialPort.Close();
                    break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
