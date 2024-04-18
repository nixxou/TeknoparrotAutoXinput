using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vJoyInterfaceWrap;

namespace XJoy
{
	class vJoyManager
	{
		public const int MaxButtons = 128;

		public class DigitalInput
		{
			public uint ButtonIndex;
			string DisplayName;

			public DigitalInput(uint _ButtonIndex, string _DisplayName)
			{
				ButtonIndex = _ButtonIndex;
				DisplayName = _DisplayName;
			}

			public override string ToString()
			{
				return DisplayName;
			}
		}

		public class AnalogInput
		{
			public HID_USAGES Axis;
			string DisplayName;

			public AnalogInput(HID_USAGES _Axis, string _DisplayName)
			{
				Axis = _Axis;
				DisplayName = _DisplayName;
			}

			public override string ToString()
			{
				return DisplayName;
			}
		}

		public struct AxisExtents
		{
			public long Min;
			public long Max;

			public AxisExtents(long m1, long m2) { Min = m1; Max = m2; }
		}

		public vJoy m_joystick;
		private uint m_vJoyID = 0;
		private bool bDeviceAcquired = false;

		public uint ActiveVJoyID
		{
			get { return m_vJoyID; }
		}

		public bool IsDeviceAcquired
		{
			get
			{
				return bDeviceAcquired;
			}
		}

		public vJoyManager()
		{
			m_joystick = new vJoy();

			if (!vJoyEnabled())
			{
				System.Windows.Forms.MessageBox.Show("Couldn't detect vJoy support. Make sure vJoy is installed.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				return;
			}

			UInt32 DllVer = 0;
			UInt32 DrvVer = 0;
			bool match = m_joystick.DriverMatch(ref DllVer, ref DrvVer);
			if (!match)
				System.Windows.Forms.MessageBox.Show("vJoy Dll version (" + DllVer + ") and Driver version (" + DrvVer + ") doesn't match. May cause bugs.", "vJoy warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
		}

		~vJoyManager()
		{
			ReleaseDevice();
		}

		public bool vJoyEnabled()
		{
			try
			{
				return (m_joystick != null && m_joystick.vJoyEnabled());
			}
			catch (DllNotFoundException)
			{
				System.Windows.Forms.MessageBox.Show("Error, vJoyInterface.dll not found. Exiting...", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				Environment.Exit(-1);
				return false;
			}
		}

		public void InitDevice(uint ID)
		{
			if (!vJoyEnabled())
				return;

			ReleaseDevice();

			bDeviceAcquired = m_joystick.AcquireVJD(ID);
			m_vJoyID = ID;
		}

		public void ReleaseDevice()
		{
			if (bDeviceAcquired)
			{
				m_joystick.ResetVJD(m_vJoyID);
				m_joystick.RelinquishVJD(m_vJoyID);
				bDeviceAcquired = false;
			}
		}

		public bool[] GetAvailableIndices()
		{
			bool[] devices = new bool[16];

			if (vJoyEnabled())
			{
				for (uint i = 0; i < 16; i++)
				{
					VjdStat status = m_joystick.GetVJDStatus(i);
					switch (status)
					{
						case VjdStat.VJD_STAT_FREE:
							devices[i] = true;
							break;
						case VjdStat.VJD_STAT_OWN:
						case VjdStat.VJD_STAT_BUSY:
						case VjdStat.VJD_STAT_MISS:
						case VjdStat.VJD_STAT_UNKN:
						default:
							devices[i] = false;
							break;
					}
				}
			}

			return devices;
		}

		public long GetMaxForAxis(HID_USAGES Axis)
		{
			long maxval = 0;
			if (bDeviceAcquired)
			{
				m_joystick.GetVJDAxisMax(m_vJoyID, Axis, ref maxval);
			}
			return maxval;
		}

		public long GetMinForAxis(HID_USAGES Axis)
		{
			long minval = 0;
			if (bDeviceAcquired)
			{
				m_joystick.GetVJDAxisMin(m_vJoyID, Axis, ref minval);
			}
			return minval;
		}

		public int GetButtonCount(uint Index)
		{
			return m_joystick.GetVJDButtonNumber(Index);
		}

		public List<HID_USAGES> GetExistingAxes(uint Index)
		{
			List<HID_USAGES> availableAxes = new List<HID_USAGES>();

			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_X)) availableAxes.Add(HID_USAGES.HID_USAGE_X);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_Y)) availableAxes.Add(HID_USAGES.HID_USAGE_Y);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_Z)) availableAxes.Add(HID_USAGES.HID_USAGE_Z);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_RX)) availableAxes.Add(HID_USAGES.HID_USAGE_RX);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_RY)) availableAxes.Add(HID_USAGES.HID_USAGE_RY);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_RZ)) availableAxes.Add(HID_USAGES.HID_USAGE_RZ);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_SL0)) availableAxes.Add(HID_USAGES.HID_USAGE_SL0);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_SL1)) availableAxes.Add(HID_USAGES.HID_USAGE_SL1);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_WHL)) availableAxes.Add(HID_USAGES.HID_USAGE_WHL);
			if (m_joystick.GetVJDAxisExist(Index, HID_USAGES.HID_USAGE_POV)) availableAxes.Add(HID_USAGES.HID_USAGE_POV);

			return availableAxes;
		}

		public AxisExtents GetAxisExtents(HID_USAGES Axis)
		{
			return new AxisExtents(GetMinForAxis(Axis), GetMaxForAxis(Axis));
		}

		public void SetButton(uint Index, bool Value)
		{
			if (bDeviceAcquired)
			{
				m_joystick.SetBtn(Value, m_vJoyID, Index);
			}
		}

		public void SetAxis(HID_USAGES Axis, int Value)
		{
			if (bDeviceAcquired)
			{
				m_joystick.SetAxis(Value, m_vJoyID, Axis);
			}
		}

		public static string AxisToFriendlyName(HID_USAGES Axis)
		{
			switch (Axis)
			{
				case HID_USAGES.HID_USAGE_X:
					return "X";
				case HID_USAGES.HID_USAGE_Y:
					return "Y";
				case HID_USAGES.HID_USAGE_Z:
					return "Z";
				case HID_USAGES.HID_USAGE_RX:
					return "RX";
				case HID_USAGES.HID_USAGE_RY:
					return "RY";
				case HID_USAGES.HID_USAGE_RZ:
					return "RZ";
				case HID_USAGES.HID_USAGE_SL0:
					return "Slider";
				case HID_USAGES.HID_USAGE_SL1:
					return "Dial/Slider 2";
				case HID_USAGES.HID_USAGE_WHL:
					return "Wheel";
				case HID_USAGES.HID_USAGE_POV:
					return "PoV";
				default:
					return Axis.ToString();
			}
		}
	}
}
