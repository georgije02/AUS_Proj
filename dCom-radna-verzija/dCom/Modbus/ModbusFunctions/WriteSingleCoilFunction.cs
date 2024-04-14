﻿using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
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
                int startIndex = 9; // Start index of data payload

                // Loop through each 16-bit value in the response
                for (int i = 0; i < dataLength / 2; i++)
                {
                    // Calculate the index of the current 16-bit value
                    int dataIndex = startIndex + i * 2;

                    // Extract the high and low bytes of the 16-bit value
                    byte highByte = response[dataIndex];
                    byte lowByte = response[dataIndex + 1];

                    // Combine the high and low bytes to form the 16-bit value
                    ushort value = (ushort)((highByte << 8) | lowByte);

                    // Create a tuple representing the point type and address
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.DIGITAL_OUTPUT, (ushort)(modbus.OutputAddress + i));

                    // Add the tuple-value pair to the dictionary
                    ret.Add(tuple, value);
                }
            }

            return ret;
        }
    }
}