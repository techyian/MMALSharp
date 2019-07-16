using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMALSharp.Callbacks
{
    public class PortCallbackMonitor : IObserver<ObservableCallback>
    {
        public void OnNext(ObservableCallback value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
