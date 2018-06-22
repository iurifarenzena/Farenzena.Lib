using System;
using System.Collections.Generic;
using System.Text;

namespace Farenzena.Lib.EventAggregator
{
    public interface IEventSubscriber<in TEvent>
    {
        void HandleEvent(TEvent e);
    }
}
