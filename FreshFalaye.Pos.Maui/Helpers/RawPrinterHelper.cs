#if WINDOWS

using System;
using System.Runtime.InteropServices;

namespace FreshFalaye.Pos.Maui.Helpers
{
    public static class RawPrinterHelper
    {
        #region WinSpool Imports

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class DOC_INFO_1
        {
            public string pDocName;
            public string pOutputFile;
            public string pDataType;
        }

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool OpenPrinter(
            string pPrinterName,
            out IntPtr phPrinter,
            IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool StartDocPrinter(
            IntPtr hPrinter,
            int level,
            [In] DOC_INFO_1 di);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool WritePrinter(
            IntPtr hPrinter,
            byte[] pBytes,
            int dwCount,
            out int dwWritten);

        #endregion

        /// <summary>
        /// Sends RAW bytes (ESC/POS) directly to printer
        /// </summary>
        public static bool SendRawBytes(string printerName, byte[] bytes)
        {
            IntPtr hPrinter = IntPtr.Zero;

            var docInfo = new DOC_INFO_1
            {
                pDocName = "POS Receipt",
                pDataType = "RAW"
            };

            try
            {
                if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
                    return false;

                if (!StartDocPrinter(hPrinter, 1, docInfo))
                    return false;

                if (!StartPagePrinter(hPrinter))
                    return false;

                WritePrinter(hPrinter, bytes, bytes.Length, out _);

                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);

                return true;
            }
            finally
            {
                if (hPrinter != IntPtr.Zero)
                    ClosePrinter(hPrinter);
            }
        }

        /// <summary>
        /// Helper to send text as ESC/POS
        /// </summary>
        public static bool SendText(string printerName, string text)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(text);
            return SendRawBytes(printerName, bytes);
        }

        /// <summary>
        /// Sends a simple test receipt
        /// </summary>
        public static bool PrintTestReceipt(string printerName)
        {
            byte[] escpos =
            {
                0x1B, 0x40,             // Initialize
                0x1B, 0x61, 0x01,       // Center
                (byte)'T',(byte)'E',(byte)'S',(byte)'T',(byte)'\n',
                0x1B, 0x61, 0x00,       // Left
                (byte)'I',(byte)'t',(byte)'e',(byte)'m',(byte)' ',(byte)'1',(byte)' ',(byte)'1',(byte)'0',(byte)'0',(byte)'\n',
                (byte)'-',(byte)'-',(byte)'-',(byte)'-',(byte)'-',(byte)'-',(byte)'-',(byte)'-',(byte)'\n',
                (byte)'T',(byte)'O',(byte)'T',(byte)'A',(byte)'L',(byte)' ',(byte)'1',(byte)'0',(byte)'0',(byte)'\n',
                0x1D, 0x56, 0x41        // Cut
            };

            return SendRawBytes(printerName, escpos);
        }
    }
}

#endif
