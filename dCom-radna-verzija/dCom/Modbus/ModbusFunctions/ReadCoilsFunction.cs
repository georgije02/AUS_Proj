using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters modbus = (ModbusReadCommandParameters)CommandParameters;
            byte[] ret = new byte[12];

            // Pack fields directly into the byte array without intermediate variables
            ret[0] = (byte)(modbus.TransactionId >> 8); // High byte
            ret[1] = (byte)modbus.TransactionId;        // Low byte

            ret[2] = (byte)(modbus.ProtocolId >> 8);
            ret[3] = (byte)modbus.ProtocolId;

            ret[4] = (byte)(modbus.Length >> 8);
            ret[5] = (byte)modbus.Length;

            ret[6] = (byte)modbus.UnitId;
            ret[7] = (byte)modbus.FunctionCode;

            ret[8] = (byte)(modbus.StartAddress >> 8);
            ret[9] = (byte)modbus.StartAddress;

            ret[10] = (byte)(modbus.Quantity >> 8);
            ret[11] = (byte)modbus.Quantity;

            return ret;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters modbus = (ModbusReadCommandParameters)CommandParameters;

            if (response.Length <= 9)
            {
                Console.WriteLine("Not valid message!");
            }
            else
            {
                int startIndex = 9;
                int endIndex = response.Length - 1;
                int byteIndex = 0;

                while (startIndex + byteIndex < endIndex)
                {
                    ushort value = BitConverter.ToUInt16(response, startIndex + byteIndex);
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.DIGITAL_OUTPUT, (ushort)(modbus.StartAddress + byteIndex / 2));
                    ret.Add(tuple, value);
                    byteIndex += 2;
                }
            }

            return ret;
        }
    }
}