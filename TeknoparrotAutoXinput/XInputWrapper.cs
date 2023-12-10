// XInputWrapper.cs
using System.Runtime.InteropServices;

public class XInputWrapper
{
	const string XInputWrapperDll = "XInputEx.dll";

	[DllImport(XInputWrapperDll, CallingConvention = CallingConvention.Cdecl)]
	public static extern bool GetControllerInfo(uint dwUserIndex, out ushort vendorId, out ushort productId, out ushort revisionId);
}
