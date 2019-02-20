using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoConnectPreview0.Services.Communication
{
    public enum DeviceOperation
    {
        Register = 1
    }
    public class DeviceMessage
    {
        public DeviceOperation Operation { get; set; }

        public object Data { get; set; }
    }
}
