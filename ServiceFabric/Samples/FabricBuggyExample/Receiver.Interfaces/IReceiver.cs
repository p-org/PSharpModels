using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver.Interfaces
{
    public interface IReceiver : IActor
    {
        Task StartTransaction();

        Task TransmitData(string item);

        Task<int> GetCurrentCount();
    }
}
