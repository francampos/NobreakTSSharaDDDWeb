using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Runtime.InteropServices;

namespace NobreakTSSharaDDDweb.HidRS232
{
  //[StandardModule]
  internal sealed class HidApiDeclarations
  {
    public const short HidP_Input = 0;
    public const short HidP_Output = 1;
    public const short HidP_Feature = 2;

    [DllImport("hid.dll")]
    public static extern bool HidD_FlushQueue(int HidDeviceObject);

    [DllImport("hid.dll")]
    public static extern bool HidD_FreePreparsedData(ref IntPtr PreparsedData);

    [DllImport("hid.dll")]
    public static extern bool HidD_GetAttributes(int HidDeviceObject, ref HidApiDeclarations.HIDD_ATTRIBUTES Attributes);

    [DllImport("hid.dll")]
    public static extern bool HidD_GetFeature(int HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

    [DllImport("hid.dll")]
    public static extern bool HidD_GetInputReport(int HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

    [DllImport("hid.dll")]
    public static extern void HidD_GetHidGuid(ref Guid HidGuid);

    [DllImport("hid.dll")]
    public static extern bool HidD_GetNumInputBuffers(int HidDeviceObject, ref int NumberBuffers);

    [DllImport("hid.dll")]
    public static extern bool HidD_GetPreparsedData(int HidDeviceObject, ref IntPtr PreparsedData);

    [DllImport("hid.dll")]
    public static extern bool HidD_SetFeature(int HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

    [DllImport("hid.dll")]
    public static extern bool HidD_SetNumInputBuffers(int HidDeviceObject, int NumberBuffers);

    [DllImport("hid.dll")]
    public static extern bool HidD_SetOutputReport(int HidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

    [DllImport("hid.dll")]
    public static extern int HidP_GetCaps(IntPtr PreparsedData, ref HidApiDeclarations.HIDP_CAPS Capabilities);

    [DllImport("hid.dll")]
    public static extern int HidP_GetValueCaps(short ReportType, ref byte ValueCaps, ref short ValueCapsLength, IntPtr PreparsedData);

    public struct HIDD_ATTRIBUTES
    {
      public int Size;
      public short VendorId;
      public short ProductId;
      public short VersionNumber;
    }

    public struct HIDP_CAPS
    {
      public short Usage;
      public short UsagePage;
      public short InputReportByteLength;
      public short OutputReportByteLength;
      public short FeatureReportByteLength;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
      public short[] Reserved;
      public short NumberLinkCollectionNodes;
      public short NumberInputButtonCaps;
      public short NumberInputValueCaps;
      public short NumberInputDataIndices;
      public short NumberOutputButtonCaps;
      public short NumberOutputValueCaps;
      public short NumberOutputDataIndices;
      public short NumberFeatureButtonCaps;
      public short NumberFeatureValueCaps;
      public short NumberFeatureDataIndices;
    }

    public struct HidP_Value_Caps
    {
      public short UsagePage;
      public byte ReportId;
      public int IsAlias;
      public short BitField;
      public short LinkCollection;
      public short LinkUsage;
      public short LinkUsagePage;
      public int IsRange;
      public int IsStringRange;
      public int IsDesignatorRange;
      public int IsAbsolute;
      public int HasNull;
      public byte Reserved;
      public short BitSize;
      public short ReportCount;
      public short Reserved2;
      public short Reserved3;
      public short Reserved4;
      public short Reserved5;
      public short Reserved6;
      public int LogicalMin;
      public int LogicalMax;
      public int PhysicalMin;
      public int PhysicalMax;
      public short UsageMin;
      public short UsageMax;
      public short StringMin;
      public short StringMax;
      public short DesignatorMin;
      public short DesignatorMax;
      public short DataIndexMin;
      public short DataIndexMax;
    }
  }
}
