using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct XINPUT_CAPABILITIES_EX
{
	public XINPUT_CAPABILITIESEX Capabilities;
	public ushort vendorId;
	public ushort productId;
	public ushort revisionId;
	public uint a4; // unknown
}

[StructLayout(LayoutKind.Sequential)]
struct XINPUT_CAPABILITIESEX
{
	public byte Type;
	public byte SubType;
	public ushort Flags;
	public XINPUT_GAMEPADEX Gamepad;
	public XINPUT_VIBRATIONEX Vibration;
}

[StructLayout(LayoutKind.Sequential)]
struct XINPUT_GAMEPADEX
{
	public ushort wButtons;
	public byte bLeftTrigger;
	public byte bRightTrigger;
	public short sThumbLX;
	public short sThumbLY;
	public short sThumbRX;
	public short sThumbRY;
}

[StructLayout(LayoutKind.Sequential)]
struct XINPUT_VIBRATIONEX
{
	public ushort wLeftMotorSpeed;
	public ushort wRightMotorSpeed;
}

public struct XINPUT_ExtraData
{
	public ushort vendorId;
	public ushort productId;
	public ushort revisionId;
}

namespace TeknoparrotAutoXinput
{

	static class XExt
	{

		const uint ERROR_SUCCESS = 0;

		[DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "#100")]
		static extern uint XInputGetCapabilitiesEx(uint a1, uint dwUserIndex, uint dwFlags, ref XINPUT_CAPABILITIES_EX pCapabilities);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, uint procName);

		static IntPtr moduleHandle = LoadLibrary("xinput1_4.dll");
		static uint ordinal = 108;
		static IntPtr address = GetProcAddress(moduleHandle, ordinal);

		delegate uint _XInputGetCapabilitiesEx(uint a1, uint dwUserIndex, uint dwFlags, ref XINPUT_CAPABILITIES_EX pCapabilities);

		public static XINPUT_ExtraData GetExtraData(int id)
		{
			var extra = new XINPUT_ExtraData();
			extra.vendorId = extra.productId = extra.revisionId = 0;

			try
			{
				if (address != IntPtr.Zero)
				{
					_XInputGetCapabilitiesEx XInputGetCapabilitiesEx = Marshal.GetDelegateForFunctionPointer<_XInputGetCapabilitiesEx>(address);
					XINPUT_CAPABILITIES_EX capsEx = new XINPUT_CAPABILITIES_EX();
					if (XInputGetCapabilitiesEx(1, (uint)id, 0, ref capsEx) == ERROR_SUCCESS)
					{
						extra.vendorId = capsEx.vendorId;
						extra.productId = capsEx.productId;
						extra.revisionId = capsEx.revisionId;
					}

				}
			}
			catch { }

			return extra;

		}
	}
}
