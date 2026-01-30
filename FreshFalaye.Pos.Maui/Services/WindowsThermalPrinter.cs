#if WINDOWS

using FreshFalaye.Pos.Maui.Helpers;

namespace FreshFalaye.Pos.Maui.Services
{
    public class WindowsThermalPrinter
    {
        /// <summary>
        /// Sends RAW ESC/POS bytes to Windows printer
        /// </summary>
        public bool Print(byte[] data, string printerName)
        {
            if (data == null || data.Length == 0)
                return false;

            if (string.IsNullOrWhiteSpace(printerName))
                return false;

            return RawPrinterHelper.SendRawBytes(
                printerName,
                data
            );
        }

        /// <summary>
        /// Convenience method for text receipts
        /// </summary>
        public bool PrintText(string text, string printerName)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return RawPrinterHelper.SendText(
                printerName,
                text
            );
        }

        /// <summary>
        /// Prints a built-in test receipt
        /// </summary>
        public bool PrintTest(string printerName)
        {
            return RawPrinterHelper.PrintTestReceipt(printerName);
        }
    }
}

#endif
