using Microsoft.VisualBasic.CompilerServices;
using NobreakTSSharaDDDweb.HidRS232;
using System;
using System.Runtime.InteropServices;


namespace NobreakTSSharaDDDWeb.HidRS232
{
    internal class DeviceManagement
    {
        private const string moduleName = "DeviceManagement";

        internal bool DeviceNameMatch(string mydevicePathName)
        {
            bool flag = false;
            try
            {
                DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICEINTERFACE_1 deviceinterface1 = new DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICEINTERFACE_1();
                DeviceManagementApiDeclarations.DEV_BROADCAST_HDR devBroadcastHdr = new DeviceManagementApiDeclarations.DEV_BROADCAST_HDR();
                //Marshal.PtrToStructure(m.LParam, (object)devBroadcastHdr);
                if (devBroadcastHdr.dbch_devicetype == 5)
                {
                    int length = checked((int)Math.Round(unchecked((double)checked(devBroadcastHdr.dbch_size - 32) / 2.0)));
                    deviceinterface1.dbcc_name = new char[checked(length + 1)];
                    //Marshal.PtrToStructure(m.LParam, (object)deviceinterface1);
                    flag = string.Compare(new string(deviceinterface1.dbcc_name, 0, length), mydevicePathName, true) == 0;
                }
            }
            catch (Exception ex)
            {
                //ProjectData.SetProjectError(ex);
                DeviceManagement.HandleException("DeviceManagement", ex);
                //ProjectData.ClearProjectError();
            }
            return flag;
        }

        internal bool FindDeviceFromGuid(Guid myGuid, ref string[] devicePathName)
        {
            bool flag1 = false;
            try
            {
                IntPtr classDevs = DeviceManagementApiDeclarations.SetupDiGetClassDevs(ref myGuid, (string)null, 0, 18);
                bool flag2 = false;
                int MemberIndex = 0;
                bool flag3 = false;
                do
                {
                    DeviceManagementApiDeclarations.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new DeviceManagementApiDeclarations.SP_DEVICE_INTERFACE_DATA();
                    DeviceInterfaceData.cbSize = Marshal.SizeOf((object)DeviceInterfaceData);
                    if (!DeviceManagementApiDeclarations.SetupDiEnumDeviceInterfaces(classDevs, 0, ref myGuid, MemberIndex, ref DeviceInterfaceData))
                    {
                        flag3 = true;
                    }
                    else
                    {
                        int RequiredSize = 0;
                        bool deviceInterfaceDetail = DeviceManagementApiDeclarations.SetupDiGetDeviceInterfaceDetail(classDevs, ref DeviceInterfaceData, IntPtr.Zero, 0, ref RequiredSize, IntPtr.Zero);
                        DeviceManagementApiDeclarations.SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetailData = new DeviceManagementApiDeclarations.SP_DEVICE_INTERFACE_DETAIL_DATA();
                        interfaceDetailData.cbSize = Marshal.SizeOf((object)interfaceDetailData);
                        IntPtr num = Marshal.AllocHGlobal(RequiredSize);
                        Marshal.WriteInt32(num, checked(4 + Marshal.SystemDefaultCharSize));
                        deviceInterfaceDetail = DeviceManagementApiDeclarations.SetupDiGetDeviceInterfaceDetail(classDevs, ref DeviceInterfaceData, num, RequiredSize, ref RequiredSize, IntPtr.Zero);
                        string stringAuto = Marshal.PtrToStringAuto(new IntPtr(checked(num.ToInt32() + 4)));
                        devicePathName[MemberIndex] = stringAuto;
                        Marshal.FreeHGlobal(num);
                        flag2 = true;
                    }
                    checked { ++MemberIndex; }
                }
                while (!flag3);
                devicePathName = (string[])Utils.CopyArray((Array)devicePathName, (Array)new string[checked(MemberIndex - 1 + 1)]);
                DeviceManagementApiDeclarations.SetupDiDestroyDeviceInfoList(classDevs);
                flag1 = flag2;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                DeviceManagement.HandleException("DeviceManagement", ex);
                ProjectData.ClearProjectError();
            }
            return flag1;
        }

        //internal bool RegisterForDeviceNotifications(string devicePathName, IntPtr formHandle, Guid classGuid, ref IntPtr deviceNotificationHandle)
        //{
        //    DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICEINTERFACE broadcastDeviceinterface = new DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICEINTERFACE();
        //    bool flag = false;
        //    try
        //    {
        //        int cb = Marshal.SizeOf((object)broadcastDeviceinterface);
        //        broadcastDeviceinterface.dbcc_size = cb;
        //        broadcastDeviceinterface.dbcc_devicetype = 5;
        //        broadcastDeviceinterface.dbcc_reserved = 0;
        //        broadcastDeviceinterface.dbcc_classguid = classGuid;
        //        IntPtr num = Marshal.AllocHGlobal(cb);
        //        Marshal.StructureToPtr((object)broadcastDeviceinterface, num, true);
        //        deviceNotificationHandle = DeviceManagementApiDeclarations.RegisterDeviceNotification(formHandle, num, 0);
        //        Marshal.PtrToStructure(num, (object)broadcastDeviceinterface);
        //        Marshal.FreeHGlobal(num);
        //        flag = deviceNotificationHandle.ToInt32() != IntPtr.Zero.ToInt32();
        //    }
        //    catch (Exception ex)
        //    {
        //        ProjectData.SetProjectError(ex);
        //        DeviceManagement.HandleException("DeviceManagement", ex);
        //        ProjectData.ClearProjectError();
        //    }
        //    return flag;
        //}

        internal bool RegisterForDeviceNotifications(string devicePathName, IntPtr formHandle, ref IntPtr deviceNotificationHandle)
        {
            DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICEINTERFACE broadcastDeviceinterface = new DeviceManagementApiDeclarations.DEV_BROADCAST_DEVICEINTERFACE();
            bool flag = false;
            try
            {
                int cb = Marshal.SizeOf((object)broadcastDeviceinterface);
                broadcastDeviceinterface.dbcc_size = cb;
                broadcastDeviceinterface.dbcc_devicetype = 5;
                broadcastDeviceinterface.dbcc_reserved = 0;
                //broadcastDeviceinterface.dbcc_classguid = classGuid;
                IntPtr num = Marshal.AllocHGlobal(cb);
                Marshal.StructureToPtr((object)broadcastDeviceinterface, num, true);
                deviceNotificationHandle = DeviceManagementApiDeclarations.RegisterDeviceNotification(formHandle, num, 0);
                Marshal.PtrToStructure(num, (object)broadcastDeviceinterface);
                Marshal.FreeHGlobal(num);
                flag = deviceNotificationHandle.ToInt32() != IntPtr.Zero.ToInt32();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                DeviceManagement.HandleException("DeviceManagement", ex);
                ProjectData.ClearProjectError();
            }
            return flag;
        }

        internal void StopReceivingDeviceNotifications(IntPtr deviceNotificationHandle)
        {
            try
            {
                DeviceManagementApiDeclarations.UnregisterDeviceNotification(deviceNotificationHandle);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                DeviceManagement.HandleException("DeviceManagement", ex);
                ProjectData.ClearProjectError();
            }
        }

        public static void HandleException(string moduleName, Exception e)
        {
            try
            {
                Console.WriteLine("Exception: " + e.Message + "\r\n" + "Module: " + moduleName + "\r\n" + "Method: " + e.TargetSite.Name);
            }
            finally
            {
            }
        }
    }
}
