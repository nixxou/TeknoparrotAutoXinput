using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace MonitorSwitcherGUI
{
	/// <summary>
	/// This class takes care of wrapping "Connecting and Configuring Displays(CCD) Win32 API"
	/// Original author Erti-Chris Eelmaa || easter199 at hotmail dot com
	/// Modifications made by Martin Krämer || martinkraemer84 at gmail dot com
	/// </summary>
	public class CCDWrapper
	{
		public const int ERROR_SUCCESS = 0;

		[StructLayout(LayoutKind.Sequential)]
		public struct LUID
		{
			public uint LowPart;
			public uint HighPart;
		}

		[Flags]
		public enum DisplayConfigVideoOutputTechnology : uint
		{
			Other = 4294967295, // -1
			Hd15 = 0,
			Svideo = 1,
			CompositeVideo = 2,
			ComponentVideo = 3,
			Dvi = 4,
			Hdmi = 5,
			Lvds = 6,
			DJpn = 8,
			Sdi = 9,
			DisplayportExternal = 10,
			DisplayportEmbedded = 11,
			UdiExternal = 12,
			UdiEmbedded = 13,
			Sdtvdongle = 14,
			Miracast = 15,
			IndirectWired = 16,
			IndirectVirtual = 17,
			Internal = 0x80000000,
			ForceUint32 = 0xFFFFFFFF
		}

		#region SdcFlags enum

		[Flags]
		public enum SdcFlags : uint
		{
			Zero = 0,

			TopologyInternal = 0x00000001,
			TopologyClone = 0x00000002,
			TopologyExtend = 0x00000004,
			TopologyExternal = 0x00000008,
			TopologySupplied = 0x00000010,

			UseSuppliedDisplayConfig = 0x00000020,
			Validate = 0x00000040,
			Apply = 0x00000080,
			NoOptimization = 0x00000100,
			SaveToDatabase = 0x00000200,
			AllowChanges = 0x00000400,
			PathPersistIfRequired = 0x00000800,
			ForceModeEnumeration = 0x00001000,
			AllowPathOrderChanges = 0x00002000,
			VirtualModeAware = 0x00008000,

			UseDatabaseCurrent = TopologyInternal | TopologyClone | TopologyExtend | TopologyExternal
		}

		[Flags]
		public enum DisplayConfigFlags : uint
		{
			Zero = 0x0,
			PathActive = 0x00000001,
			PathPrefferedUnscaled = 0x00000004,
			PathSupportVirtualMode = 0x00000008,
			PathValidFlags = 0x0000000D
		}

		[Flags]
		public enum DisplayConfigSourceStatus
		{
			Zero = 0x0,
			InUse = 0x00000001
		}

		[Flags]
		public enum DisplayConfigTargetStatus : uint
		{
			Zero = 0x0,

			InUse = 0x00000001,
			FORCIBLE = 0x00000002,
			ForcedAvailabilityBoot = 0x00000004,
			ForcedAvailabilityPath = 0x00000008,
			ForcedAvailabilitySystem = 0x00000010,
			Is_HMD = 0x00000020,
		}

		[Flags]
		public enum DisplayConfigRotation : uint
		{
			Zero = 0x0,

			Identity = 1,
			Rotate90 = 2,
			Rotate180 = 3,
			Rotate270 = 4,
			ForceUint32 = 0xFFFFFFFF
		}

		[Flags]
		public enum DisplayConfigPixelFormat : uint
		{
			Zero = 0x0,

			Pixelformat8Bpp = 1,
			Pixelformat16Bpp = 2,
			Pixelformat24Bpp = 3,
			Pixelformat32Bpp = 4,
			PixelformatNongdi = 5,
			PixelformatForceUint32 = 0xffffffff
		}

		[Flags]
		public enum DisplayConfigScaling : uint
		{
			Zero = 0x0,

			Identity = 1,
			Centered = 2,
			Stretched = 3,
			Aspectratiocenteredmax = 4,
			Custom = 5,
			Preferred = 128,
			ForceUint32 = 0xFFFFFFFF
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigRational
		{
			public uint numerator;
			public uint denominator;
		}

		[Flags]
		public enum DisplayConfigScanLineOrdering : uint
		{
			Unspecified = 0,
			Progressive = 1,
			Interlaced = 2,
			InterlacedUpperfieldfirst = Interlaced,
			InterlacedLowerfieldfirst = 3,
			ForceUint32 = 0xFFFFFFFF
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigPathInfo
		{
			public DisplayConfigPathSourceInfo sourceInfo;
			public DisplayConfigPathTargetInfo targetInfo;
			public uint flags;
		}

		[Flags]
		public enum DisplayConfigModeInfoType : uint
		{
			Zero = 0,

			Source = 1,
			Target = 2,
			DesktopImage = 3,
			ForceUint32 = 0xFFFFFFFF
		}

		public struct DpiInfo
		{
			public int id;
			public uint adapter;
			public int dpi;
		}

		public enum DisplayConfigDpiInfoType : uint
		{
			id = 0,
			adapter = 0,
			dpi = 100
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct DisplayConfigModeInfo
		{
			[FieldOffset((0))]
			public DisplayConfigModeInfoType infoType;

			[FieldOffset(4)]
			public uint id;

			[FieldOffset(8)]
			public LUID adapterId;

			[FieldOffset(16)]
			public DisplayConfigTargetMode targetMode;

			[FieldOffset(16)]
			public DisplayConfigSourceMode sourceMode;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfig2DRegion
		{
			public uint cx;
			public uint cy;
		}

		public enum D3DkmdtVideoSignalStandard : UInt32
		{
			Uninitialized = 0,
			VesaDmt = 1,
			VesaGtf = 2,
			VesaCvt = 3,
			Ibm = 4,
			Apple = 5,
			NtscM = 6,
			NtscJ = 7,
			Ntsc443 = 8,
			PalB = 9,
			PalB1 = 10,
			PalG = 11,
			PalH = 12,
			PalI = 13,
			PalD = 14,
			PalN = 15,
			PalNc = 16,
			SecamB = 17,
			SecamD = 18,
			SecamG = 19,
			SecamH = 20,
			SecamK = 21,
			SecamK1 = 22,
			SecamL = 23,
			SecamL1 = 24,
			Eia861 = 25,
			Eia861A = 26,
			Eia861B = 27,
			PalK = 28,
			PalK1 = 29,
			PalL = 30,
			PalM = 31,
			Other = 255,
			USB = 65791
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigVideoSignalInfo
		{
			public long pixelRate;
			public DisplayConfigRational hSyncFreq;
			public DisplayConfigRational vSyncFreq;
			public DisplayConfig2DRegion activeSize;
			public DisplayConfig2DRegion totalSize;

			public D3DkmdtVideoSignalStandard videoStandard;
			public DisplayConfigScanLineOrdering ScanLineOrdering;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigTargetMode
		{
			public DisplayConfigVideoSignalInfo targetVideoSignalInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PointL
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigSourceMode
		{
			public uint width;
			public uint height;
			public DisplayConfigPixelFormat pixelFormat;
			public PointL position;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigPathSourceInfo
		{
			public LUID adapterId;
			public uint id;
			public uint modeInfoIdx;

			public DisplayConfigSourceStatus statusFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigPathTargetInfo
		{
			public LUID adapterId;
			public uint id;
			public uint modeInfoIdx;
			public DisplayConfigVideoOutputTechnology outputTechnology;
			public DisplayConfigRotation rotation;
			public DisplayConfigScaling scaling;
			public DisplayConfigRational refreshRate;
			public DisplayConfigScanLineOrdering scanLineOrdering;

			public bool targetAvailable;
			public DisplayConfigTargetStatus statusFlags;
		}

		[Flags]
		public enum QueryDisplayFlags : uint
		{
			Zero = 0x0,

			AllPaths = 0x00000001,
			OnlyActivePaths = 0x00000002,
			DatabaseCurrent = 0x00000004,
			VirtualModeAware = 0x00000010,
			IncludeHMD = 0x00000020,
		}

		[Flags]
		public enum DisplayConfigTopologyId : uint
		{
			Zero = 0x0,

			Internal = 0x00000001,
			Clone = 0x00000002,
			Extend = 0x00000004,
			External = 0x00000008,
			Supplied = 0x00000010,
			ForceUint32 = 0xFFFFFFFF
		}


		#endregion

		[DllImport("User32.dll")]
		public static extern int SetDisplayConfig(
			uint numPathArrayElements,
			[In] DisplayConfigPathInfo[] pathArray,
			uint numModeInfoArrayElements,
			[In] DisplayConfigModeInfo[] modeInfoArray,
			SdcFlags flags
		);

		[DllImport("User32.dll")]
		public static extern int QueryDisplayConfig(
			QueryDisplayFlags flags,
			ref uint numPathArrayElements,
			[Out] DisplayConfigPathInfo[] pathInfoArray,
			ref uint modeInfoArrayElements,
			[Out] DisplayConfigModeInfo[] modeInfoArray,
			IntPtr z
		);

		[DllImport("User32.dll")]
		public static extern int GetDisplayConfigBufferSizes(QueryDisplayFlags flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

		public enum DisplayConfigDeviceInfoType : uint
		{
			GetSourceName = 1,
			GetTargetName = 2,
			GetTargetPreferredMode = 3,
			GetAdapterName = 4,
			SetTargetPersistence = 5,
			GetTargetBaseType = 6,
			GetSupportVirtualResolution = 7,
			SetSupportVirtualResolution = 8,
			AdvancedColorInfo = 9,
			AdvancedColorState = 10,
			SDRWhiteLevel = 11,
			ForceUint32 = 0xFFFFFFFF,

		}

		/// <summary>
		/// Use this enum so that you don't have to hardcode magic values.
		/// </summary>
		public enum StatusCode : uint
		{
			Success = 0,
			InvalidParameter = 87,
			NotSupported = 50,
			AccessDenied = 5,
			GenFailure = 31,
			BadConfiguration = 1610,
			InSufficientBuffer = 122,
		}

		/// <summary>
		/// Just an contract.
		/// </summary>
		public interface IDisplayConfigInfo
		{
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigTargetDeviceNameFlags
		{
			public uint value;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DisplayConfigDeviceInfoHeader
		{
			public DisplayConfigDeviceInfoType type;
			public uint size;
			public LUID adapterId;
			public uint id;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DisplayConfigTargetDeviceName
		{
			public DisplayConfigDeviceInfoHeader header;
			public DisplayConfigTargetDeviceNameFlags flags;
			public DisplayConfigVideoOutputTechnology outputTechnology;
			public ushort edidManufactureId;
			public ushort edidProductCodeId;
			public uint connectorInstance;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string monitorFriendlyDeviceName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string monitorDevicePath;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DisplayConfigSourceDeviceName : IDisplayConfigInfo
		{
			private const int Cchdevicename = 32;

			public DisplayConfigDeviceInfoHeader header;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Cchdevicename)]
			public string viewGdiDeviceName;
		}

		/*[DllImport("User32.dll")]
        private static extern StatusCode DisplayConfigGetDeviceInfo(IntPtr requestPacket);
        public static StatusCode DisplayConfigGetDeviceInfo<T>(ref T displayConfig) where T : IDisplayConfigInfo
        {
            return MarshalStructureAndCall(ref displayConfig, DisplayConfigGetDeviceInfo);
        }*/

		[DllImport("user32.dll")]
		public static extern int DisplayConfigGetDeviceInfo(ref DisplayConfigTargetDeviceName deviceName);

		public static string MonitorFriendlyName(LUID adapterId, uint targetId)
		{
			var deviceName = new DisplayConfigTargetDeviceName
			{
				header =
				{
					size = (uint)Marshal.SizeOf(typeof (DisplayConfigTargetDeviceName)),
					adapterId = adapterId,
					id = targetId,
					type = DisplayConfigDeviceInfoType.GetTargetName
				}
			};
			var error = DisplayConfigGetDeviceInfo(ref deviceName);
			if (error != ERROR_SUCCESS)
				throw new Win32Exception(error);
			return deviceName.monitorFriendlyDeviceName;
		}

		[StructLayout(LayoutKind.Sequential)]
		[Serializable]
		public struct MonitorAdditionalInfo
		{
			//public string monitorDevicePath { get; set; }
			//public string monitorFriendlyDevice { get; set; }
			public ushort manufactureId { get; set; }
			public ushort productCodeId { get; set; }
			public Boolean valid { get; set; }


			[XmlElement(ElementName = "monitorDevicePath")]
			public String monitorDevicePath64
			{
				get
				{
					string outValue = monitorDevicePath;
					if (outValue == null)
					{
						outValue = "";
					}
					return Convert.ToBase64String(System.Text.Encoding.UTF32.GetBytes(outValue));
				}
				set
				{
					if (value == null)
					{
						monitorDevicePath = null;
						return;
					}

					monitorDevicePath = System.Text.Encoding.UTF32.GetString(Convert.FromBase64String(value));
				}
			}

			[XmlIgnore]
			public String monitorDevicePath;

			[XmlElement(ElementName = "monitorFriendlyDevice")]
			public String monitorFriendlyDevice64
			{
				get
				{
					string outValue = monitorFriendlyDevice;
					if (outValue == null)
					{
						outValue = "";
					}
					return Convert.ToBase64String(System.Text.Encoding.UTF32.GetBytes(outValue));
				}
				set
				{
					if (value == null)
					{
						monitorFriendlyDevice = null;
						return;
					}

					monitorFriendlyDevice = System.Text.Encoding.UTF32.GetString(Convert.FromBase64String(value));
				}
			}

			[XmlIgnore]
			public String monitorFriendlyDevice;
		}

		public static MonitorAdditionalInfo GetMonitorAdditionalInfo(LUID adapterId, uint targetId)
		{
			MonitorAdditionalInfo result = new MonitorAdditionalInfo();
			var deviceName = new DisplayConfigTargetDeviceName
			{
				header =
				{
					size = (uint)Marshal.SizeOf(typeof (DisplayConfigTargetDeviceName)),
					adapterId = adapterId,
					id = targetId,
					type = DisplayConfigDeviceInfoType.GetTargetName
				}
			};
			var error = DisplayConfigGetDeviceInfo(ref deviceName);
			if (error != ERROR_SUCCESS)
				throw new Win32Exception(error);

			result.valid = true;
			result.manufactureId = deviceName.edidManufactureId;
			result.productCodeId = deviceName.edidProductCodeId;
			result.monitorDevicePath = deviceName.monitorDevicePath;
			result.monitorFriendlyDevice = deviceName.monitorFriendlyDeviceName;

			return result;
		}

		/// <summary>
		/// The idea of this method is to make sure we have type-safety, without any stupid overloads.
		/// Without this, you would need to marshal yourself everything when using DisplayConfigGetDeviceInfo,
		/// or SetDeviceInfo, without any type-safety. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="displayConfig"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		private static StatusCode MarshalStructureAndCall<T>(ref T displayConfig,
			Func<IntPtr, StatusCode> func) where T : IDisplayConfigInfo
		{
			var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(displayConfig));
			Marshal.StructureToPtr(displayConfig, ptr, false);

			var returnValue = func(ptr);

			displayConfig = (T)Marshal.PtrToStructure(ptr, displayConfig.GetType());

			Marshal.FreeHGlobal(ptr);
			return returnValue;
		}
	}
}
