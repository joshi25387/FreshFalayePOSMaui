using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshFalaye.Pos.Shared.Abstractions
{
    public interface IReceiptPrinter
    {
        Task PrintAsync(long saleId);
    }
}
