using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionData;

namespace Receiver.Interfaces
{
    public interface IReceiver : IActor
    {
        Task StartTransaction();

        Task TransmitData(TransactionItems item);

        Task<int> GetFinalCount();
    }
}
