using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.InteropServices;

namespace NobreakTSSharaDDDweb.HidRS232
{
    [StandardModule]
    internal sealed class FileIOApiDeclarations
    {
        public const int FILE_FLAG_OVERLAPPED = 1073741824;
        public const short FILE_SHARE_READ = 1;
        public const short FILE_SHARE_WRITE = 2;
        public const int GENERIC_READ = -2147483648;
        public const int GENERIC_WRITE = 1073741824;
        public const int INVALID_HANDLE_VALUE = -1;
        public const short OPEN_EXISTING = 3;
        public const int WAIT_TIMEOUT = 258;
        public const short WAIT_OBJECT_0 = 0;

        [DllImport("kernel32.dll")]
        public static extern int CancelIo(int hFile);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(int hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int CreateEvent(ref FileIOApiDeclarations.SECURITY_ATTRIBUTES SecurityAttributes, int bManualReset, int bInitialState, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, ref FileIOApiDeclarations.SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern int ReadFile(int hFile, ref byte lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, ref FileIOApiDeclarations.OVERLAPPED lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern int WaitForSingleObject(int hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(int hFile, ref byte lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);

        public struct OVERLAPPED
        {
            public int Internal;
            public int InternalHigh;
            public int Offset;
            public int OffsetHigh;
            public int hEvent;
        }

        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public int lpSecurityDescriptor;
            public int bInheritHandle;
        }
    }
}
