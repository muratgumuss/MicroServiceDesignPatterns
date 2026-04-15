using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class StockNotReservedEvent
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}
