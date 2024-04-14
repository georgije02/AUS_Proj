using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters modbus = (ModbusWriteCommandParameters)CommandParameters;
            byte[] ret = new byte[12];

            // TransactionId (2 bytes)
            ret[0] = (byte)(modbus.TransactionId >> 8);  // High byte
            ret[1] = (byte)modbus.TransactionId;        // Low byte

            // ProtocolId (2 bytes)
            ret[2] = (byte)(modbus.ProtocolId >> 8);    // High byte
            ret[3] = (byte)modbus.ProtocolId;          // Low byte

            // Length (2 bytes)
            ret[4] = (byte)(modbus.Length >> 8);        // High byte
            ret[5] = (byte)modbus.Length;              // Low byte

            // UnitId (1 byte)
            ret[6] = (byte)modbus.UnitId;

            // FunctionCode (1 byte)
            ret[7] = (byte)modbus.FunctionCode;

            // OutputAddress (2 bytes)
            ret[8] = (byte)(modbus.OutputAddress >> 8);  // High byte
            ret[9] = (byte)modbus.OutputAddress;        // Low byte

            // Value (2 bytes)
            ret[10] = (byte)(modbus.Value >> 8);     // High byte
            ret[11] = (byte)modbus.Value;           // Low byte

            return ret;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusWriteCommandParameters modbus = (ModbusWriteCommandParameters)CommandParameters;

            if (response.Length <= 9)
            {
                Console.WriteLine("Not valid message!");
            }
            else
            {
                int dataLength = response[8]; // Length of data payload

                // Ensure that the response length is valid
                if (response.Length < 10 + dataLength)
                {
                    Console.WriteLine("Invalid message length!");
                }
                else
                {
                    // Extract the high and low bytes of the value
                    byte highByte = response[10];
                    byte lowByte = response[9];

                    // Combine the high and low bytes to form the 16-bit value
                    ushort value = (ushort)((highByte << 8) | lowByte);

                    // Create a tuple representing the point type and address
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.ANALOG_OUTPUT, modbus.OutputAddress);

                    // Add the tuple-value pair to the dictionary
                    ret.Add(tuple, value);
                }
            }

            return ret;
        }
    }
}