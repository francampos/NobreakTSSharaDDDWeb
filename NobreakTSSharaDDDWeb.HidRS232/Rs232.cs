using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NobreakTSSharaDDDweb.HidRS232
{
    public class Rs232
    {
        private int mhRS;
        private int miPort;
        private int miTimeout;
        private int miBaudRate;
        private Rs232.DataParity meParity;
        private Rs232.DataStopBit meStopBit;
        private int miDataBit;
        private int miBufferSize;
        private byte[] mabtRxBuf;
        private Rs232.Mode meMode;
        private bool mbWaitOnRead;
        private bool mbWaitOnWrite;
        private bool mbWriteErr;
        private Rs232.OVERLAPPED muOverlapped;
        private Rs232.OVERLAPPED muOverlappedW;
        private Rs232.OVERLAPPED muOverlappedE;
        private byte[] mabtTmpTxBuf;
        private Thread moThreadTx;
        private Thread moThreadRx;
        private int miTmpBytes2Read;
        private Rs232.EventMasks meMask;
        private const int PURGE_RXABORT = 2;
        private const int PURGE_RXCLEAR = 8;
        private const int PURGE_TXABORT = 1;
        private const int PURGE_TXCLEAR = 4;
        private const int GENERIC_READ = -2147483648;
        private const int GENERIC_WRITE = 1073741824;
        private const int OPEN_EXISTING = 3;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int IO_BUFFER_SIZE = 1024;
        private const int FILE_FLAG_OVERLAPPED = 1073741824;
        private const int ERROR_IO_PENDING = 997;
        private const int WAIT_OBJECT_0 = 0;
        private const int ERROR_IO_INCOMPLETE = 996;
        private const int WAIT_TIMEOUT = 258;
        private const int INFINITE = -1;

        public int BaudRate
        {
            get
            {
                return this.miBaudRate;
            }
            set
            {
                this.miBaudRate = value;
            }
        }

        public int BufferSize
        {
            get
            {
                return this.miBufferSize;
            }
            set
            {
                this.miBufferSize = value;
            }
        }

        public int DataBit
        {
            get
            {
                return this.miDataBit;
            }
            set
            {
                this.miDataBit = value;
            }
        }

        public bool Dtr
        {
            set
            {
                if (this.mhRS == -1)
                    return;
                if (value)
                    Rs232.EscapeCommFunction(this.mhRS, 5L);
                else
                    Rs232.EscapeCommFunction(this.mhRS, 6L);
            }
        }

        public virtual byte[] InputStream
        {
            get
            {
                return this.mabtRxBuf;
            }
        }

        public virtual string InputStreamString
        {
            get
            {
                return new ASCIIEncoding().GetString(this.InputStream);
            }
        }

        public bool IsOpen
        {
            get
            {
                return this.mhRS != -1;
            }
        }

        public Rs232.ModemStatusBits ModemStatus
        {
            get
            {
                if (this.mhRS == -1)
                    throw new ApplicationException("Please initialize and open port before using this method");
                int lpModemStatus = 0;
                if (!Rs232.GetCommModemStatus(this.mhRS, ref lpModemStatus))
                    throw new ApplicationException("Unable to get modem status");
                return (Rs232.ModemStatusBits)lpModemStatus;
            }
        }

        public Rs232.DataParity Parity
        {
            get
            {
                return this.meParity;
            }
            set
            {
                this.meParity = value;
            }
        }

        public int Port
        {
            get
            {
                return this.miPort;
            }
            set
            {
                this.miPort = value;
            }
        }

        public bool Rts
        {
            set
            {
                if (this.mhRS == -1)
                    return;
                if (value)
                    Rs232.EscapeCommFunction(this.mhRS, 3L);
                else
                    Rs232.EscapeCommFunction(this.mhRS, 4L);
            }
        }

        public Rs232.DataStopBit StopBit
        {
            get
            {
                return this.meStopBit;
            }
            set
            {
                this.meStopBit = value;
            }
        }

        public virtual int Timeout
        {
            get
            {
                return this.miTimeout;
            }
            set
            {
                this.miTimeout = IntegerType.FromObject(Interaction.IIf(value == 0, (object)500, (object)value));
                this.pSetTimeout();
            }
        }

        public Rs232.Mode WorkingMode
        {
            get
            {
                return this.meMode;
            }
            set
            {
                this.meMode = value;
            }
        }

        public event Rs232.DataReceivedEventHandler DataReceived;

        public event Rs232.TxCompletedEventHandler TxCompleted;

        public event Rs232.CommEventEventHandler CommEvent;

        public Rs232()
        {
            this.mhRS = -1;
            this.miPort = 1;
            this.miTimeout = 70;
            this.miBaudRate = 9600;
            this.meParity = Rs232.DataParity.Parity_None;
            this.meStopBit = (Rs232.DataStopBit)0;
            this.miDataBit = 8;
            this.miBufferSize = 512;
        }

        [DllImport("kernel32.dll")]
        private static extern int BuildCommDCB(string lpDef, ref Rs232.DCB lpDCB);

        [DllImport("kernel32.dll")]
        private static extern int ClearCommError(int hFile, int lpErrors, int l);

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(int hObject);

        [DllImport("kernel32.dll")]
        private static extern int CreateEvent(int lpEventAttributes, int bManualReset, int bInitialState, [MarshalAs(UnmanagedType.LPStr)] string lpName);

        [DllImport("kernel32.dll")]
        private static extern int CreateFile([MarshalAs(UnmanagedType.LPStr)] string lpFileName, int dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll")]
        private static extern bool EscapeCommFunction(int hFile, long ifunc);

        [DllImport("kernel32.dll")]
        private static extern int FormatMessage(int dwFlags, int lpSource, int dwMessageId, int dwLanguageId, [MarshalAs(UnmanagedType.LPStr)] string lpBuffer, int nSize, int Arguments);

        [DllImport("kernel32", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int FormatMessage(int dwFlags, int lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int Arguments);

        [DllImport("kernel32.dll")]
        public static extern bool GetCommModemStatus(int hFile, ref int lpModemStatus);

        [DllImport("kernel32.dll")]
        private static extern int GetCommState(int hCommDev, ref Rs232.DCB lpDCB);

        [DllImport("kernel32.dll")]
        private static extern int GetCommTimeouts(int hFile, ref Rs232.COMMTIMEOUTS lpCommTimeouts);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("kernel32.dll")]
        private static extern int GetOverlappedResult(int hFile, ref Rs232.OVERLAPPED lpOverlapped, ref int lpNumberOfBytesTransferred, int bWait);

        [DllImport("kernel32.dll")]
        private static extern int PurgeComm(int hFile, int dwFlags);

        [DllImport("kernel32.dll")]
        private static extern int ReadFile(int hFile, byte[] Buffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, ref Rs232.OVERLAPPED lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern int SetCommTimeouts(int hFile, ref Rs232.COMMTIMEOUTS lpCommTimeouts);

        [DllImport("kernel32.dll")]
        private static extern int SetCommState(int hCommDev, ref Rs232.DCB lpDCB);

        [DllImport("kernel32.dll")]
        private static extern int SetupComm(int hFile, int dwInQueue, int dwOutQueue);

        [DllImport("kernel32.dll")]
        private static extern int SetCommMask(int hFile, int lpEvtMask);

        [DllImport("kernel32.dll")]
        private static extern int WaitCommEvent(int hFile, ref Rs232.EventMasks Mask, ref Rs232.OVERLAPPED lpOverlap);

        [DllImport("kernel32.dll")]
        private static extern int WaitForSingleObject(int hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll")]
        private static extern int WriteFile(int hFile, byte[] Buffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, ref Rs232.OVERLAPPED lpOverlapped);

        public void _R()
        {
            this.Read(this.miTmpBytes2Read);
        }

        public void _W()
        {
            this.Write(this.mabtTmpTxBuf);
        }

        public void AsyncRead(int Bytes2Read)
        {
            if (this.meMode != Rs232.Mode.Overlapped)
                throw new ApplicationException("Async Methods allowed only when WorkingMode=Overlapped");
            this.miTmpBytes2Read = Bytes2Read;
            this.moThreadTx = new Thread(new ThreadStart(this._R));
            this.moThreadTx.Start();
        }

        public void AsyncWrite(byte[] Buffer)
        {
            if (this.meMode != Rs232.Mode.Overlapped)
                throw new ApplicationException("Async Methods allowed only when WorkingMode=Overlapped");
            if (this.mbWaitOnWrite)
                throw new ApplicationException("Unable to send message because of pending transmission.");
            this.mabtTmpTxBuf = Buffer;
            this.moThreadTx = new Thread(new ThreadStart(this._W));
            this.moThreadTx.Start();
        }

        public void AsyncWrite(string Buffer)
        {
            this.AsyncWrite(new ASCIIEncoding().GetBytes(Buffer));
        }

        public bool CheckLineStatus(Rs232.ModemStatusBits Line)
        {
            return Convert.ToBoolean((int)(this.ModemStatus & Line));
        }

        public void ClearInputBuffer()
        {
            if (this.mhRS == -1)
                return;
            Rs232.PurgeComm(this.mhRS, 8);
        }

        public void Close()
        {
            if (this.mhRS == -1)
                return;
            Rs232.CloseHandle(this.mhRS);
            this.mhRS = -1;
        }

        public void Open()
        {
            int int32 = Convert.ToInt32(RuntimeHelpers.GetObjectValue(Interaction.IIf(this.meMode == Rs232.Mode.Overlapped, (object)1073741824, (object)0)));
            if (this.miPort <= 0)
                throw new ApplicationException("COM Port not defined, use Port property to set it before invoking InitPort");
            try
            {
                this.mhRS = Rs232.CreateFile("COM" + this.miPort.ToString(), -1073741824, 0, 0, 3, int32, 0);
                if (this.mhRS == -1)
                    throw new Rs232.CIOChannelException("Unable to open COM" + this.miPort.ToString());
                int lpErrors = 0;
                int num = Rs232.ClearCommError(this.mhRS, lpErrors, 0);
                num = Rs232.PurgeComm(this.mhRS, 12);
                Rs232.DCB lpDCB = new DCB();
                num = Rs232.GetCommState(this.mhRS, ref lpDCB);
                num = Rs232.BuildCommDCB(string.Format("baud={0} parity={1} data={2} stop={3}", (object)this.miBaudRate, (object)"NOEM".Substring((int)this.meParity, 1), (object)this.miDataBit, (object)this.meStopBit), ref lpDCB);
                if (Rs232.SetCommState(this.mhRS, ref lpDCB) == 0)
                    throw new Rs232.CIOChannelException("Unable to set COM state0" + this.pErr2Text(Rs232.GetLastError()));
                num = Rs232.SetupComm(this.mhRS, this.miBufferSize, this.miBufferSize);
                this.pSetTimeout();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception InnerException = ex;
                throw new Rs232.CIOChannelException(InnerException.Message, InnerException);
            }
        }

        public void Open(int Port, int BaudRate, int DataBit, Rs232.DataParity Parity, Rs232.DataStopBit StopBit, int BufferSize)
        {
            this.Port = Port;
            this.BaudRate = BaudRate;
            this.DataBit = DataBit;
            this.Parity = Parity;
            this.StopBit = StopBit;
            this.BufferSize = BufferSize;
            this.Open();
        }

        private string pErr2Text(int lCode)
        {
            StringBuilder lpBuffer = new StringBuilder(256);
            if (Rs232.FormatMessage(4096, 0, lCode, 0, lpBuffer, 256, 0) > 0)
                return lpBuffer.ToString();
            return "Error not found.";
        }

        private void pHandleOverlappedRead(int Bytes2Read)
        {
            this.muOverlapped.hEvent = Rs232.CreateEvent(0, 1, 0, (string)null);
            if (this.muOverlapped.hEvent == 0)
                throw new ApplicationException("Error creating event for overlapped read.");
            int num = 0;
            if (!this.mbWaitOnRead)
            {
                this.mabtRxBuf = new byte[checked(Bytes2Read - 1 + 1)];
                if (Rs232.ReadFile(this.mhRS, this.mabtRxBuf, Bytes2Read, ref num, ref this.muOverlapped) == 0)
                {
                    int lastError = Rs232.GetLastError();
                    if (lastError != 997)
                        throw new ArgumentException("Overlapped Read Error: " + this.pErr2Text(lastError));
                    this.mbWaitOnRead = true;
                }
                //else if (this.DataReceivedEvent != null)
                //    this.DataReceivedEvent(this, this.mabtRxBuf);
            }
            if (!this.mbWaitOnRead)
                return;
            switch (Rs232.WaitForSingleObject(this.muOverlapped.hEvent, this.miTimeout))
            {
                case 0:
                    if (Rs232.GetOverlappedResult(this.mhRS, ref this.muOverlapped, ref num, 0) == 0)
                    {
                        int lastError = Rs232.GetLastError();
                        if (lastError == 996)
                            throw new ApplicationException("Read operation incomplete");
                        throw new ApplicationException("Read operation error " + lastError.ToString());
                    }
                    //if (this.DataReceivedEvent != null)
                    //    this.DataReceivedEvent(this, this.mabtRxBuf);
                    this.mbWaitOnRead = false;
                    break;
                case 258:
                    throw new Rs232.IOTimeoutException("Timeout error");
                default:
                    throw new ApplicationException("Overlapped read error");
            }
        }

        private bool pHandleOverlappedWrite(byte[] Buffer)
        {
            this.muOverlappedW.hEvent = Rs232.CreateEvent(0, 1, 0, (string)null);
            if (this.muOverlappedW.hEvent == 0)
                throw new ApplicationException("Error creating event for overlapped write.");
            Rs232.PurgeComm(this.mhRS, 12);
            this.mbWaitOnRead = true;
            int num = 0;
            bool flag =false;
            if (Rs232.WriteFile(this.mhRS, Buffer, Buffer.Length, ref num, ref this.muOverlappedW) == 0)
            {
                int lastError = Rs232.GetLastError();
                if (lastError != 997)
                    throw new ArgumentException("Overlapped Read Error: " + this.pErr2Text(lastError));
                if (Rs232.WaitForSingleObject(this.muOverlappedW.hEvent, -1) == 0)
                {
                    if (Rs232.GetOverlappedResult(this.mhRS, ref this.muOverlappedW, ref num, 0) == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        this.mbWaitOnRead = false;
                        //if (this.TxCompletedEvent != null)
                        //    this.TxCompletedEvent(this);
                    }
                }
            }
            else
                flag = false;
            Rs232.CloseHandle(this.muOverlappedW.hEvent);
            return flag;
        }

        private void pSetTimeout()
        {
            if (this.mhRS == -1)
                return;
            Rs232.COMMTIMEOUTS lpCommTimeouts;
            lpCommTimeouts.ReadIntervalTimeout = 0;
            lpCommTimeouts.ReadTotalTimeoutMultiplier = 0;
            lpCommTimeouts.ReadTotalTimeoutConstant = this.miTimeout;
            lpCommTimeouts.WriteTotalTimeoutMultiplier = 10;
            lpCommTimeouts.WriteTotalTimeoutConstant = 100;
            Rs232.SetCommTimeouts(this.mhRS, ref lpCommTimeouts);
        }

        public int Read(int Bytes2Read)
        {
            if (Bytes2Read == 0)
                Bytes2Read = this.miBufferSize;
            if (this.mhRS == -1)
                throw new ApplicationException("Please initialize and open port before using this method");
            int num1 = 0;
            try
            {
                if (this.meMode == Rs232.Mode.Overlapped)
                {
                    this.pHandleOverlappedRead(Bytes2Read);
                }
                else
                {
                    this.mabtRxBuf = new byte[checked(Bytes2Read - 1 + 1)];
                    int mhRs = this.mhRS;
                    byte[] mabtRxBuf = this.mabtRxBuf;
                    int nNumberOfBytesToRead = Bytes2Read;
                    int num2=0;
                    // ISSUE: explicit reference operation
                    // ISSUE: variable of a reference type
                    int lpNumberOfBytesRead = @num2;
                    Rs232.OVERLAPPED overlapped = (Rs232.OVERLAPPED)((object)null ?? Activator.CreateInstance(typeof(Rs232.OVERLAPPED)));
                    // ISSUE: explicit reference operation
                    // ISSUE: variable of a reference type
                    Rs232.OVERLAPPED lpOverlapped = @overlapped;
                    int num3 = Rs232.ReadFile(mhRs, mabtRxBuf, nNumberOfBytesToRead, ref lpNumberOfBytesRead, ref lpOverlapped);
                    if (num3 == 0)
                        throw new ApplicationException("ReadFile error " + num3.ToString());
                    if (num2 < Bytes2Read)
                        throw new Rs232.IOTimeoutException("Timeout error");
                    this.mbWaitOnRead = true;
                    num1 = num2;
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception innerException = ex;
                throw new ApplicationException("Read Error: " + innerException.Message, innerException);
            }
            return num1;
        }

        public void Write(byte[] Buffer)
        {
            if (this.mhRS == -1)
                throw new ApplicationException("Please initialize and open port before using this method");
            try
            {
                if (this.meMode == Rs232.Mode.Overlapped)
                {
                    if (this.pHandleOverlappedWrite(Buffer))
                        throw new ApplicationException("Error in overllapped write");
                }
                else
                {
                    Rs232.PurgeComm(this.mhRS, 12);
                    int mhRs = this.mhRS;
                    byte[] Buffer1 = Buffer;
                    int length = Buffer.Length;
                    int num = 0;
                    // ISSUE: explicit reference operation
                    // ISSUE: variable of a reference type
                    int lpNumberOfBytesWritten = @num;
                    Rs232.OVERLAPPED overlapped = (Rs232.OVERLAPPED)((object)null ?? Activator.CreateInstance(typeof(Rs232.OVERLAPPED)));
                    // ISSUE: explicit reference operation
                    // ISSUE: variable of a reference type
                    Rs232.OVERLAPPED  lpOverlapped = @overlapped;
                    if (Rs232.WriteFile(mhRs, Buffer1, length, ref lpNumberOfBytesWritten, ref lpOverlapped) == 0)
                        throw new ApplicationException("Write Error - Bytes Written " + num.ToString() + " of " + Buffer.Length.ToString());
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                throw;
            }
        }

        public void Write(string Buffer)
        {
            this.Write(new ASCIIEncoding().GetBytes(Buffer));
        }

        public enum DataParity
        {
            Parity_None,
            Pariti_Odd,
            Parity_Even,
            Parity_Mark,
        }

        public enum DataStopBit
        {
            StopBit_1 = 1,
            StopBit_2 = 2,
        }

        private enum PurgeBuffers
        {
            TxAbort = 1,
            RXAbort = 2,
            TxClear = 4,
            RXClear = 8,
        }

        private enum Lines
        {
            SetRts = 3,
            ClearRts = 4,
            SetDtr = 5,
            ClearDtr = 6,
            ResetDev = 7,
            SetBreak = 8,
            ClearBreak = 9,
        }

        [Flags]
        public enum ModemStatusBits
        {
            ClearToSendOn = 16,
            DataSetReadyOn = 32,
            RingIndicatorOn = 64,
            CarrierDetect = 128,
        }

        public enum Mode
        {
            NonOverlapped,
            Overlapped,
        }

        [Flags]
        public enum EventMasks
        {
            RxChar = 1,
            RXFlag = 2,
            TxBufferEmpty = 4,
            ClearToSend = 8,
            DataSetReady = 16,
            ReceiveLine = 32,
            Break = 64,
            StatusError = 128,
            Ring = 256,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DCB
        {
            public int DCBlength;
            public int BaudRate;
            public int Bits1;
            public short wReserved;
            public short XonLim;
            public short XoffLim;
            public byte ByteSize;
            public byte Parity;
            public byte StopBits;
            public byte XonChar;
            public byte XoffChar;
            public byte ErrorChar;
            public byte EofChar;
            public byte EvtChar;
            public short wReserved2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct COMMTIMEOUTS
        {
            public int ReadIntervalTimeout;
            public int ReadTotalTimeoutMultiplier;
            public int ReadTotalTimeoutConstant;
            public int WriteTotalTimeoutMultiplier;
            public int WriteTotalTimeoutConstant;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct COMMCONFIG
        {
            public int dwSize;
            public short wVersion;
            public short wReserved;
            public Rs232.DCB dcbx;
            public int dwProviderSubType;
            public int dwProviderOffset;
            public int dwProviderSize;
            public byte wcProviderData;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OVERLAPPED
        {
            public int Internal;
            public int InternalHigh;
            public int Offset;
            public int OffsetHigh;
            public int hEvent;
        }

        public class CIOChannelException : ApplicationException
        {
            public CIOChannelException(string Message)
              : base(Message)
            {
            }

            public CIOChannelException(string Message, Exception InnerException)
              : base(Message, InnerException)
            {
            }
        }

        public class IOTimeoutException : Rs232.CIOChannelException
        {
            public IOTimeoutException(string Message)
              : base(Message)
            {
            }

            public IOTimeoutException(string Message, Exception InnerException)
              : base(Message, InnerException)
            {
            }
        }

        public delegate void DataReceivedEventHandler(Rs232 Source, byte[] DataBuffer);

        public delegate void TxCompletedEventHandler(Rs232 Source);

        public delegate void CommEventEventHandler(Rs232 Source, Rs232.EventMasks Mask);
    }
}
