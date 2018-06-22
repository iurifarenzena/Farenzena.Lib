using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.EventAggregator
{
    public interface IAsyncEventSubscriber<in TEvent>
    {
        Task HandleEventAsync(TEvent e);
    }
}
