using System.Runtime.InteropServices;

namespace TeknoparrotAutoXinput
{
	public static class DPIUtils
	{

		[DllImport("SetDpi.dll", EntryPoint = "dpi_GetRecommendedDPIScaling", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetRecommendedDPIScaling();

		[DllImport("SetDpi.dll", EntryPoint = "dpi_GetMonitorID", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetMonitorID(int index);

		[DllImport("SetDpi.dll", EntryPoint = "dpi_GetAdapterID", CallingConvention = CallingConvention.StdCall)]
		public static extern uint GetAdapterID(int index);

		[DllImport("SetDpi.dll", EntryPoint = "dpi_GetMonitorDPI", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetMonitorDPI(int index);

		[DllImport("SetDpi.dll", EntryPoint = "dpi_SetMonitorDPI", CallingConvention = CallingConvention.StdCall)]
		public static extern bool SetMonitorDPI(int index, int dpi);

	}

}
