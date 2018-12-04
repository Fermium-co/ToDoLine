using Bit.ViewModel;
using System;
using System.Collections.Generic;

namespace ToDoLineApp.Implementations
{
    public class ToDoLineExceptionHandler : BitExceptionHandler
    {
        public override void OnExceptionReceived(Exception exp, IDictionary<string, string> properties = null)
        {
#if DEBUG

            System.Diagnostics.Debugger.Break();

#endif

            base.OnExceptionReceived(exp, properties);
        }
    }
}
