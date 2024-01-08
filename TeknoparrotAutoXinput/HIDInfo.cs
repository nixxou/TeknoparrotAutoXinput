using System.Text;
using Newtonsoft.Json;
using XInput.Wrapper;
using System.Security.Cryptography;
using SharpDX.DirectInput;
using SDL2;
using static XInput.Wrapper.X.Gamepad;
using System;
using TeknoparrotAutoXinput;

public static class HIDInfo
{
	public static string LastXInput { get; private set; }

	public static string LastDInput { get; private set; }

	public static string LastSDL { get; private set; }

	public static string LastSDLNoRI { get; private set; }


	public static string GetDInputInfo(bool refresh)
	{
		if (!refresh && !String.IsNullOrEmpty(LastDInput)) return LastDInput;

		LastDInput = "";
		DirectInput directInput = new DirectInput();
		var ddevices = directInput.GetDevices();
		foreach (var deviceInstance in ddevices)
		{
			if (!IsStickType(deviceInstance))
				continue;

			var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
			LastDInput += $"{deviceInstance.ProductName}<>{deviceInstance.Type}<>{deviceInstance.InstanceGuid}<>{deviceInstance.InstanceName}<>{joystick.Properties.InterfacePath}" + "\r\n";

			//deviceList.Add(new DeviceInfo(deviceInstance));
		}
		return LastDInput;
	}

	private static bool IsStickType(DeviceInstance deviceInstance)
	{
		return deviceInstance.Type == SharpDX.DirectInput.DeviceType.Joystick
				|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Gamepad
				|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.FirstPerson
				|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Flight
				|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Driving
				|| deviceInstance.Type == SharpDX.DirectInput.DeviceType.Supplemental;
	}

	public static string GetSDLInfo(bool refresh)
	{
		if (!refresh && !String.IsNullOrEmpty(LastSDL)) return LastSDL;

		LastSDL = "";
		//SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
		SDL2.SDL.SDL_Quit();
		SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
		SDL2.SDL.SDL_JoystickUpdate();


		for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
		{
			var currentJoy = SDL.SDL_JoystickOpen(i);
			string caps = $"{SDL.SDL_JoystickNumAxes(currentJoy)} {SDL.SDL_JoystickNumBalls(currentJoy)} {SDL.SDL_JoystickNumButtons(currentJoy)} {SDL.SDL_JoystickNumHats(currentJoy)}";
			string signature = GetMD5Short(caps);
			const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
			byte[] guidBuffer = new byte[bufferSize];
			SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
			string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');

			string joyname = SDL2.SDL.SDL_JoystickName(currentJoy);

			LastSDL += $"SDL{i}<>{SDL2.SDL.SDL_JoystickNameForIndex(i)}<>{signature}<>{SDL.SDL_JoystickGetDeviceGUID(i)}<>{SDL.SDL_JoystickGetSerial(currentJoy)}<>{guidString}" + "\r\n";
			SDL.SDL_JoystickClose(currentJoy);

		}
		return LastSDL;
	}

	public static string GetSDLNoRIInfo(bool refresh)
	{
		if (!refresh && !String.IsNullOrEmpty(LastSDLNoRI)) return LastSDLNoRI;

		LastSDLNoRI = "";
		SDL2.SDL.SDL_Quit();
		SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_JOYSTICK_RAWINPUT, "0");
		SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK | SDL2.SDL.SDL_INIT_GAMECONTROLLER);
		SDL2.SDL.SDL_JoystickUpdate();


		for (int i = 0; i < SDL2.SDL.SDL_NumJoysticks(); i++)
		{
			var currentJoy = SDL.SDL_JoystickOpen(i);
			string caps = $"{SDL.SDL_JoystickNumAxes(currentJoy)} {SDL.SDL_JoystickNumBalls(currentJoy)} {SDL.SDL_JoystickNumButtons(currentJoy)} {SDL.SDL_JoystickNumHats(currentJoy)}";
			string signature = GetMD5Short(caps);
			const int bufferSize = 256; // La taille doit être au moins 33 pour stocker le GUID sous forme de chaîne (32 caractères + le caractère nul)
			byte[] guidBuffer = new byte[bufferSize];
			SDL.SDL_JoystickGetGUIDString(SDL.SDL_JoystickGetGUID(currentJoy), guidBuffer, bufferSize);
			string guidString = System.Text.Encoding.UTF8.GetString(guidBuffer).Trim('\0');

			LastSDLNoRI += $"SDLNORI{i}<>{SDL2.SDL.SDL_JoystickNameForIndex(i)}<>{signature}<>{SDL.SDL_JoystickGetDeviceGUID(i)}<>{SDL.SDL_JoystickGetSerial(currentJoy)}<>{guidString}" + "\r\n";
			SDL.SDL_JoystickClose(currentJoy);
		}
		return LastSDLNoRI;
	}


	public static string GetXINPUT(bool refresh, string ds4logPath = "")
	{

		if (!refresh && !String.IsNullOrEmpty(LastXInput)) return LastXInput;

		LastXInput = "";

		var gamepad = X.Gamepad_1;
		if (gamepad.Capabilities.Type != 0)
		{
			int slot = 0;
			var caps = gamepad.Capabilities;
			string json = JsonConvert.SerializeObject(caps, Newtonsoft.Json.Formatting.None);
			string signature = GetMD5Short(json);
			string extra = "\r\n";
			

			var capsEx = XExt.GetExtraData(slot);
			string controllerName = Program.GetJoyId(capsEx.vendorId, capsEx.productId);
			LastXInput += $"XINPUT{slot}<>Type={gamepad.Capabilities.SubType.ToString().Trim()}<>Signature={signature}<>VendorID=0x{capsEx.vendorId:X04}<>ProductID=0x{capsEx.productId:X04}<>RevisionID=0x{capsEx.revisionId:X04}<>{controllerName}{extra}";
		}


		gamepad = X.Gamepad_2;
		if (gamepad.Capabilities.Type != 0)
		{
			int slot = 1;
			var caps = gamepad.Capabilities;
			string json = JsonConvert.SerializeObject(caps, Newtonsoft.Json.Formatting.None);
			string signature = GetMD5Short(json);
			string extra = "\r\n";


			var capsEx = XExt.GetExtraData(slot);
			string controllerName = Program.GetJoyId(capsEx.vendorId, capsEx.productId);
			LastXInput += $"XINPUT{slot}<>Type={gamepad.Capabilities.SubType.ToString().Trim()}<>Signature={signature}<>VendorID=0x{capsEx.vendorId:X04}<>ProductID=0x{capsEx.productId:X04}<>RevisionID=0x{capsEx.revisionId:X04}<>{controllerName}{extra}";
		}

		gamepad = X.Gamepad_3;
		if (gamepad.Capabilities.Type != 0)
		{
			int slot = 2;
			var caps = gamepad.Capabilities;
			string json = JsonConvert.SerializeObject(caps, Newtonsoft.Json.Formatting.None);
			string signature = GetMD5Short(json);
			string extra = "\r\n";


			var capsEx = XExt.GetExtraData(slot);
			string controllerName = Program.GetJoyId(capsEx.vendorId, capsEx.productId);
			LastXInput += $"XINPUT{slot}<>Type={gamepad.Capabilities.SubType.ToString().Trim()}<>Signature={signature}<>VendorID=0x{capsEx.vendorId:X04}<>ProductID=0x{capsEx.productId:X04}<>RevisionID=0x{capsEx.revisionId:X04}<>{controllerName}{extra}";
		}

		gamepad = X.Gamepad_4;
		if (gamepad.Capabilities.Type != 0)
		{
			int slot = 3;
			var caps = gamepad.Capabilities;
			string json = JsonConvert.SerializeObject(caps, Newtonsoft.Json.Formatting.None);
			string signature = GetMD5Short(json);
			string extra = "\r\n";


			var capsEx = XExt.GetExtraData(slot);
			string controllerName = Program.GetJoyId(capsEx.vendorId, capsEx.productId);
			LastXInput += $"XINPUT{slot}<>Type={gamepad.Capabilities.SubType.ToString().Trim()}<>Signature={signature}<>VendorID=0x{capsEx.vendorId:X04}<>ProductID=0x{capsEx.productId:X04}<>RevisionID=0x{capsEx.revisionId:X04}<>{controllerName}{extra}";
		}
		return LastXInput;
	}

	public static string GetMD5Short(string input)
	{

		using (MD5 md5 = MD5.Create())
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			// Convertir les octets du hash en une chaîne hexadécimale
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2")); // X2 pour obtenir en majuscules
			}

			return sb.ToString().Substring(0, 6);
		}

	}

	public static void ClearCache()
	{
		LastXInput = null;
		LastDInput = null;
		LastSDLNoRI = null;
		LastSDL = null;
	}


}

