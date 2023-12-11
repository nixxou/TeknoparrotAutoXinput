using Gma.System.MouseKeyHook;
using TeknoparrotAutoXinput;

namespace TestVgme
{
	public class PickKeyCombo
	{
		private Form1 AddForm { get; set; }

		private string TextBoxName { get; set; }

		public Keys[] Keys { get; set; }

		public PickKeyCombo(Form1 addForm)
		{
			this.AddForm = addForm;
			this.Keys = new Keys[3];
		}

		public void StartPicking(string textBoxName)
		{
			TextBoxName = textBoxName;
			Keys = new Keys[3];
			HookManager.CleanHook();
			HookManager.SetKeyDown(GlobalKeyDown);
			HookManager.SetKeyUp(GlobalKeyUp);
		}

		private void GlobalKeyDown(object sender, KeyEventArgs e)
		{

			string[] parts = GetKeyCombo(Keys, true).Split('+');
			if (parts.Where(p => p == PrettyKeys.Convert(e.KeyCode)).Any()) return; // dont allow duplicate keys in the combo

			if (parts.Length < 3)
			{
				for (int i = 0; i < Keys.Length; i++)
				{
					if (Keys[i] == System.Windows.Forms.Keys.None)
					{
						Keys[i] = e.KeyCode;
						break;
					}
				}
			}
			//AddForm.TextBoxKeyCombo.Text = GetKeyCombo(this.Keys, true);
			//AddForm.DrawKeyDisplay();

		}

		private void GlobalKeyUp(object sender, KeyEventArgs e)
		{

			AddForm.Keys = Keys;
			// update form
			foreach (Control c in AddForm.Controls)
			{
				c.Enabled = true;
			}



			AddForm.DrawKeyDisplay(TextBoxName);
			//AddForm.TextBoxKeyCombo.Select(AddForm.TextBoxKeyCombo.Text.Length, 0);

			// remove hook

			HookManager.CleanHook();
		}

		public static string GetKeyCombo(Keys[] keys, bool pretty)
		{
			string res = string.Empty;
			int count = 0;
			foreach (Keys key in keys.Where(k => k != System.Windows.Forms.Keys.None))
			{
				if (count != 0) res += "+";
				res += pretty ? PrettyKeys.Convert(key) : key.ToString();
				count++;
			}
			return res;
		}
	}

	public static class PrettyKeys
	{
		public static readonly Dictionary<Keys, string> PrettyDictionary = new Dictionary<Keys, string>
		{
			{ Keys.OemCloseBrackets, "RightBracket" },
			{ Keys.OemOpenBrackets, "LeftBracket" },
			{ Keys.OemQuestion, "ForwardSlash" },
			{ Keys.OemSemicolon, "Semicolon" },
			{ Keys.LControlKey, "Control" },
			{ Keys.RControlKey, "Control" },
			{ Keys.Oemtilde, "Tilde" },
			{ Keys.Oem5, "BackSlash" },
			{ Keys.LShiftKey, "Shift" },
			{ Keys.RShiftKey, "Shift" },
			{ Keys.LWin, "Windows" },
			{ Keys.RWin, "Windows" },
			{ Keys.LMenu, "Alt" },
			{ Keys.RMenu, "Alt" },
			{ Keys.Oemcomma, "Comma" },
			{ Keys.OemPeriod, "Period" },
			{ Keys.Oem7, "Quote" },
			{ Keys.OemMinus, "Minus" },
			{ Keys.Oemplus, "Plus" },
			{ Keys.Return, "Enter" },
			{ Keys.D0, "0" },
			{ Keys.D1, "1" },
			{ Keys.D2, "2" },
			{ Keys.D3, "3" },
			{ Keys.D4, "4" },
			{ Keys.D5, "5" },
			{ Keys.D6, "6" },
			{ Keys.D7, "7" },
			{ Keys.D8, "8" },
			{ Keys.D9, "9" },
		};

		public static Keys Convert(string key)
		{
			var outVal = PrettyDictionary.Where(kvp => kvp.Value == key).FirstOrDefault();
			return outVal.Value == null ? Keys.None : outVal.Key;
		}

		public static string Convert(Keys key)
		{
			string res = key.ToString();
			if (PrettyDictionary.TryGetValue(key, out string temp))
			{
				res = temp;
			}
			return res;
		}
	}

	public static class HookManager
	{
		private static IKeyboardMouseEvents _globalHook = null;
		private static IKeyboardMouseEvents GlobalHook
		{
			get
			{
				if (_globalHook == null)
				{
					_globalHook = Hook.GlobalEvents();
				}
				return _globalHook;
			}
		}

		public static void CleanHook()
		{
			if (_globalHook != null)
			{
				_globalHook.Dispose();
				_globalHook = null;
			}
		}

		public static void SetCombinationHook(Dictionary<Combination, Action> combos)
		{
			try
			{
				GlobalHook.OnCombination(combos);
			}
			catch
			{
				CleanHook();
			}
		}

		public static void SetMouseDown(MouseEventHandler function)
		{
			try
			{
				GlobalHook.MouseDown += function;
			}
			catch
			{
				CleanHook();
			}
		}

		public static void SetMouseUp(MouseEventHandler function)
		{
			try
			{
				GlobalHook.MouseUp += function;
			}
			catch
			{
				CleanHook();
			}
		}

		public static void SetKeyDown(KeyEventHandler function)
		{
			try
			{
				GlobalHook.KeyDown += function;
			}
			catch
			{
				CleanHook();
			}
		}

		public static void SetKeyUp(KeyEventHandler function)
		{
			try
			{
				GlobalHook.KeyUp += function;
			}
			catch
			{
				CleanHook();
			}
		}
	}



}