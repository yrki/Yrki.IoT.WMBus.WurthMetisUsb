using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Yrki.IoT.WMBus.IMSTim871A
{
    public class IMSTim871A
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
            serialPort.Close();
        }

        public Task ListenToMessages(string port, int baudrate, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("Listening to messages");
            var serialPort = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
            serialPort.Open();

            var charactersLeft = 0;
            List<byte> message = new List<byte>();

            while(true)
            {
                System.Diagnostics.Debug.WriteLine("Here");
                var value = serialPort.ReadByte();
                System.Diagnostics.Debug.WriteLine(value.ToString("X2"));
                // var buffer = new byte[serialPort.BytesToRead];
                // if(buffer.Length > 1)
                // {
                //     serialPort.Read(buffer, 0, buffer.Length);
                //     Console.WriteLine(BitConverter.ToString(buffer));

                    // for (int i = 0; i < buffer.Length; i++)
                    // {
                    //     if(charactersLeft <= 0 && (i < buffer.Length - 1) && buffer[i] == 0xFF && buffer[i+1] == 0x03)
                    //     {
                    //         charactersLeft = buffer[i+2];

                    //         if(message.Count > 0)
                    //         {
                    //             MessageReceived?.Invoke(this, message.ToArray());
                    //             message = new List<byte>();
                    //         }
                    //     }

                    //     message.Add(buffer[i]);
                    //     charactersLeft--;
                    // }
                // }
                // Thread.Sleep(100);

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
