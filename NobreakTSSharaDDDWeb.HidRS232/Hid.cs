using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace NobreakTSSharaDDDweb.HidRS232
{
    internal class Hid
    {
        private const string ModuleName = "Hid";
        internal HidApiDeclarations.HIDP_CAPS Capabilities;
        internal HidApiDeclarations.HIDD_ATTRIBUTES DeviceAttributes;

        internal bool FlushQueue(int hidHandle)
        {
            bool flag = false;
            try
            {
                flag = HidApiDeclarations.HidD_FlushQueue(hidHandle);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        [HandleProcessCorruptedStateExceptions]
        internal HidApiDeclarations.HIDP_CAPS GetDeviceCapabilities(int hidHandle)
        {
            byte[] inArray = new byte[30];
            byte[] numArray = new byte[1024];
            HidApiDeclarations.HIDP_CAPS capabilities = new HidApiDeclarations.HIDP_CAPS();
            try
            {
                IntPtr PreparsedData = new IntPtr();
                bool flag = HidApiDeclarations.HidD_GetPreparsedData(hidHandle, ref PreparsedData);
                Convert.ToBase64String(inArray);
                if (HidApiDeclarations.HidP_GetCaps(PreparsedData, ref Capabilities) != 0)
                {
                    HidApiDeclarations.HidP_GetValueCaps((short)0, ref numArray[0], ref Capabilities.NumberInputValueCaps, PreparsedData);
                    flag = HidApiDeclarations.HidD_FreePreparsedData(ref PreparsedData);
                }
                capabilities = Capabilities;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return capabilities;
        }

        internal string GetHIDUsage(HidApiDeclarations.HIDP_CAPS MyCapabilities)
        {
            string str1 = "";
            try
            {
                string str2 = "";
                int num = checked((int)MyCapabilities.UsagePage * 256 + (int)MyCapabilities.Usage);
                if (num == 258)
                    str2 = "mouse";
                if (num == 262)
                    str2 = "keyboard";
                str1 = str2;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return str1;
        }

        internal bool GetNumberOfInputBuffers(int hidDeviceObject, ref int numberOfInputBuffers)
        {
            bool flag1 = false;
            try
            {
                bool flag2;
                if (!IsWindows98Gold())
                {
                    flag2 = HidApiDeclarations.HidD_GetNumInputBuffers(hidDeviceObject, ref numberOfInputBuffers);
                }
                else
                {
                    numberOfInputBuffers = 2;
                    flag2 = true;
                }
                flag1 = flag2;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return flag1;
        }

        internal bool SetNumberOfInputBuffers(int hidDeviceObject, int numberBuffers)
        {
            bool flag1 = false;
            try
            {
                if (!IsWindows98Gold())
                {
                    HidApiDeclarations.HidD_SetNumInputBuffers(hidDeviceObject, numberBuffers);
                    bool flag2 = false;
                    flag1 = flag2;
                }
                else
                    flag1 = false;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return flag1;
        }

        internal bool IsWindowsXpOrLater()
        {
            bool flag = false;
            try
            {
                flag = Environment.OSVersion.Version >= new Version(5, 1);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        internal bool IsWindows98Gold()
        {
            bool flag = false;
            try
            {
                flag = Environment.OSVersion.Version < new Version(4, 10, 2183);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Hid.HandleException("Hid", ex);
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        public static void HandleException(string moduleName, Exception e)
        {
            try
            {
                //int num = (int)MessageBox.Show("Exception: " + e.Message + "\r\n" + "Module: " + moduleName + "\r\n" + "Method: " + e.TargetSite.Name, "Unexpected Exception", MessageBoxButtons.OK);
            }
            finally
            {
            }
        }

        internal abstract class DeviceReport
        {
            internal int HIDHandle;
            internal bool MyDeviceDetected;
            internal int Result;
            internal int ReadHandle;

            protected abstract void ProtectedRead(int readHandle, int hidHandle, int writeHandle, ref bool myDeviceDetected, ref byte[] readBuffer, ref bool success);

            internal void Read(int readHandle, int hidHandle, int writeHandle, ref bool myDeviceDetected, ref byte[] readBuffer, ref bool success)
            {
                try
                {
                    ProtectedRead(readHandle, hidHandle, writeHandle, ref myDeviceDetected, ref readBuffer, ref success);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
            }
        }

        internal class InFeatureReport : Hid.DeviceReport
        {
            protected override void ProtectedRead(int readHandle, int hidHandle, int writeHandle, ref bool myDeviceDetected, ref byte[] inFeatureReportBuffer, ref bool success)
            {
                try
                {
                    success = HidApiDeclarations.HidD_GetFeature(hidHandle, ref inFeatureReportBuffer[0], checked(Information.UBound((Array)inFeatureReportBuffer, 1) + 1));
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
            }
        }

        internal class InputReport : Hid.DeviceReport
        {
            private bool ReadyForOverlappedTransfer;
            [SpecialName]
            private FileIOApiDeclarations.OVERLAPPED HIDOverlapped;
            [SpecialName]
            private int EventObject;

            internal void CancelTransfer(int readHandle, int hidHandle, int writeHandle)
            {
                try
                {
                    FileIOApiDeclarations.CancelIo(readHandle);
                    if (hidHandle != 0)
                        FileIOApiDeclarations.CloseHandle(hidHandle);
                    if (readHandle != 0)
                        FileIOApiDeclarations.CloseHandle(readHandle);
                    if (writeHandle == 0)
                        return;
                    FileIOApiDeclarations.CloseHandle(writeHandle);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
            }

            internal void PrepareForOverlappedTransfer(ref FileIOApiDeclarations.OVERLAPPED hidOverlapped, ref int eventObject)
            {
                try
                {
                    FileIOApiDeclarations.SECURITY_ATTRIBUTES SecurityAttributes = new FileIOApiDeclarations.SECURITY_ATTRIBUTES();
                    SecurityAttributes.lpSecurityDescriptor = 0;
                    SecurityAttributes.bInheritHandle = -1;
                    SecurityAttributes.nLength = Strings.Len(SecurityAttributes);
                    eventObject = FileIOApiDeclarations.CreateEvent(ref SecurityAttributes, 0, -1, "");
                    hidOverlapped.Offset = 0;
                    hidOverlapped.OffsetHigh = 0;
                    hidOverlapped.hEvent = eventObject;
                    ReadyForOverlappedTransfer = true;
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
            }

            protected override void ProtectedRead(int readHandle, int hidHandle, int writeHandle, ref bool myDeviceDetected, ref byte[] inputReportBuffer, ref bool success)
            {
                try
                {
                    if (!ReadyForOverlappedTransfer)
                        PrepareForOverlappedTransfer(ref HIDOverlapped, ref EventObject);
                    int lpNumberOfBytesRead = 0;
                    FileIOApiDeclarations.ReadFile(readHandle, ref inputReportBuffer[0], checked(Information.UBound((Array)inputReportBuffer, 1) + 1), ref lpNumberOfBytesRead, ref HIDOverlapped);
                    int num = FileIOApiDeclarations.WaitForSingleObject(EventObject, 100);
                    switch (num)
                    {
                        case 0:
                            success = true;
                            break;
                        case 258:
                            CancelTransfer(readHandle, hidHandle, writeHandle);
                            success = false;
                            myDeviceDetected = false;
                            break;
                        default:
                            CancelTransfer(readHandle, hidHandle, writeHandle);
                            success = false;
                            myDeviceDetected = false;
                            break;
                    }
                    if (num == 0)
                        success = true;
                    else
                        success = false;
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
            }
        }

        internal class InputReportViaControlTransfer : Hid.DeviceReport
        {
            protected override void ProtectedRead(int readHandle, int hidHandle, int writeHandle, ref bool myDeviceDetected, ref byte[] inputReportBuffer, ref bool success)
            {
                try
                {
                    success = HidApiDeclarations.HidD_GetInputReport(hidHandle, ref inputReportBuffer[0], checked(Information.UBound((Array)inputReportBuffer, 1) + 1));
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
            }
        }

        internal abstract class HostReport
        {
            protected abstract bool ProtectedWrite(int deviceHandle, byte[] reportBuffer);

            internal bool Write(byte[] reportBuffer, int deviceHandle)
            {
                bool flag = false;
                try
                {
                    flag = ProtectedWrite(deviceHandle, reportBuffer);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
                return flag;
            }
        }

        public class OutFeatureReport : Hid.HostReport
        {
            protected override bool ProtectedWrite(int hidHandle, byte[] outFeatureReportBuffer)
            {
                bool flag = false;
                try
                {
                    flag = HidApiDeclarations.HidD_SetFeature(hidHandle, ref outFeatureReportBuffer[0], checked(Information.UBound((Array)outFeatureReportBuffer, 1) + 1));
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
                return flag;
            }
        }

        public class OutputReport : Hid.HostReport
        {
            protected override bool ProtectedWrite(int writeHandle, byte[] outputReportBuffer)
            {
                bool flag1 = false;
                try
                {
                    int lpNumberOfBytesWritten = 0;
                    bool flag2 = FileIOApiDeclarations.WriteFile(writeHandle, ref outputReportBuffer[0], checked(Information.UBound((Array)outputReportBuffer, 1) + 1), ref lpNumberOfBytesWritten, 0);
                    if (!flag2 && writeHandle != 0)
                        FileIOApiDeclarations.CloseHandle(writeHandle);
                    flag1 = flag2;
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
                return flag1;
            }
        }

        public class OutputReportViaControlTransfer : Hid.HostReport
        {
            protected override bool ProtectedWrite(int hidHandle, byte[] outputReportBuffer)
            {
                bool flag = false;
                try
                {
                    flag = HidApiDeclarations.HidD_SetOutputReport(hidHandle, ref outputReportBuffer[0], checked(Information.UBound((Array)outputReportBuffer, 1) + 1));
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Hid.HandleException("Hid", ex);
                    ProjectData.ClearProjectError();
                }
                return flag;
            }
        }
    }
}