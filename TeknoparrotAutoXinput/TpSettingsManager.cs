using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace TeknoparrotAutoXinput
{
	public static class TpSettingsManager
	{
		public static Dictionary<string, string> TpSettings = new Dictionary<string, string>();
		public static string OriginalXML;
		public static string ModifiedXML;
		public static XmlDocument xmlDoc = null;
		public static string windowed_search = "";
		public static List<string> tags = new List<string>();
		private static Dictionary<string, Dictionary<string,string>> allSettings = new Dictionary<string, Dictionary<string,string>>();
		public static bool IsWindowed = false;
		public static bool IsPatreon = false;



		public static void setOriginalXML(string xmlFile)
		{
			OriginalXML = File.ReadAllText(xmlFile);
			xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(OriginalXML);
			XmlNode patreonNode = xmlDoc.SelectSingleNode("//GameProfile/Patreon");
			if (patreonNode != null && patreonNode.InnerText == "true")
			{
				IsPatreon = true;
			}

		}



		public static void SetWindowedStatus()
		{
			string windowed_CategoryName = "";
			string windowed_FieldName = "";
			string windowed_FieldValue = "";
			if (windowed_search != "")
			{
				var windowedData = windowed_search.Split(',');
				if (windowedData.Count() == 3)
				{

					windowed_CategoryName = windowedData[0].Trim();
					windowed_FieldName = windowedData[1].Trim();
					windowed_FieldValue = windowedData[2].Trim();
				}
			}
			bool result = false;
			if (windowed_CategoryName != "" && windowed_FieldName != "" && windowed_FieldValue != "")
			{
				string xpathExpression = $"/GameProfile/ConfigValues/FieldInformation[CategoryName='{windowed_CategoryName}' and FieldName='{windowed_FieldName}' and FieldValue='{windowed_FieldValue}']";
				XmlNode fieldNode = TpSettingsManager.xmlDoc.SelectSingleNode(xpathExpression);
				if (fieldNode != null)
				{
					result = true;
					//Utils.LogMessage("Game Is Windowed");
				}
				if (!result)
				{
					xpathExpression = $"/GameProfile/ConfigValues/FieldInformation[CategoryName='General' and FieldName='DisplayMode' and FieldValue='Windowed']";
					fieldNode = TpSettingsManager.xmlDoc.SelectSingleNode(xpathExpression);
					if (fieldNode != null)
					{
						result = true;
						//Utils.LogMessage("Game Is Windowed");
					}
				}
				if (!result)
				{
					xpathExpression = $"/GameProfile/ConfigValues/FieldInformation[CategoryName='General' and FieldName='Windowed' and FieldValue='1']";
					fieldNode = TpSettingsManager.xmlDoc.SelectSingleNode(xpathExpression);
					if (fieldNode != null)
					{
						result = true;
						//Utils.LogMessage("Game Is Windowed");
					}
				}
			}
			IsWindowed = result;
		}

		public static void SetSettings(JObject tpSection)
		{
			try
			{
				allSettings = tpSection.ToObject<Dictionary<string, Dictionary<string,string>>>();

			}
			catch { }
		}

		public static void UpdateXML()
		{
			try
			{
				TpSettings = new Dictionary<string, string>();
				//allSettings = tpSection.ToObject<Dictionary<string, List<string>>>();

				//string jsonContent = File.ReadAllText(jsonFilePath);
				//var allSettings = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonContent);

				if (allSettings == null) return;

				if (allSettings.ContainsKey("default"))
				{
					ApplySettings(allSettings["default"]);
				}

				foreach (var allSettingsKeys in allSettings.Keys)
				{
					string allSettingsKeysTrim = allSettingsKeys.Trim().Replace(" ", "").ToLower();
					if (allSettingsKeys == "default") continue;
					if (allSettingsKeysTrim.Contains("&")) continue;

					if (tags.Contains(allSettingsKeysTrim))
					{
						ApplySettings(allSettings[allSettingsKeys]);
					}
				}

				foreach (var allSettingsKeys in allSettings.Keys)
				{
					string allSettingsKeysTrim = allSettingsKeys.Trim().Replace(" ", "").ToLower();
					if (allSettingsKeys == "default") continue;
					if (!allSettingsKeysTrim.Contains("&")) continue;

					bool allTagValid = true;
					foreach (var tagelem in allSettingsKeysTrim.Split('&'))
					{
						if (!tags.Contains(tagelem))
						{
							allTagValid = false;
							break;
						}
					}

					if (allTagValid)
					{
						ApplySettings(allSettings[allSettingsKeys]);
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error during overwrite: " + ex.Message);
			}

			XmlNodeList fieldNodes = xmlDoc.SelectNodes("//FieldInformation");
			foreach (XmlNode fieldNode in fieldNodes)
			{
				XmlNode categoryNameNode = fieldNode.SelectSingleNode("CategoryName");
				XmlNode fieldNameNode = fieldNode.SelectSingleNode("FieldName");
				XmlNode fieldValueNode = fieldNode.SelectSingleNode("FieldValue");

				if (categoryNameNode != null && fieldNameNode != null && fieldValueNode != null)
				{
					string keyToSearch = categoryNameNode.InnerText.Trim().ToLower() + "||" + fieldNameNode.InnerText.Trim().ToLower();
					if (TpSettings.ContainsKey(keyToSearch))
					{
						fieldValueNode.InnerText = TpSettings[keyToSearch];
					}
				}
			}
			ModifiedXML = xmlDoc.OuterXml;
			SetWindowedStatus();

		}

		private static void ApplySettings(Dictionary<string,string> settings)
		{
			foreach (var setting in settings)
			{
				var key = setting.Key.Trim().ToLower();
				var value = setting.Value.Trim();

				if (value.ToLower() == "{{apm3id}}") value = ConfigurationManager.MainConfig.patch_apm3id;
				if (value.ToLower() == "{{mariokartid}}") value = ConfigurationManager.MainConfig.patch_mariokartId;
				if (value.ToLower() == "{{customname}}") value = ConfigurationManager.MainConfig.patch_customName;
				if (value.ToLower() == "{{networkip}}") value = Program.patch_networkIP;
				if (value.ToLower() == "{{networkgateway}}") value = Program.patch_networkGateway;
				if (value.ToLower() == "{{networkmask}}") value = Program.patch_networkMask;
				if (value.ToLower() == "{{broadcastaddress}}") value = Program.patch_BroadcastAddress;
				if (value.ToLower() == "{{networkdns1}}") value = Program.patch_networkDns1;
				if (value.ToLower() == "{{networkdns2}}") value = Program.patch_networkDns2;

				TpSettings[key] = value;
				
			}
		}

		//Dictionary<string, JoystickButton> emptyJoystickButtonDictionary
		public static void emptyJoystickButtons(Dictionary<string, JoystickButton> emptyJoystickButtonDictionary)
		{
			string xmlFileContent = ModifiedXML;

			if (xmlFileContent.Contains("<JoystickButtons>") && xmlFileContent.Contains("</JoystickButtons>"))
			{
				string debutFichier = xmlFileContent.Split("<JoystickButtons>")[0];
				string finFichier = xmlFileContent.Split("</JoystickButtons>").Last();
				string xmlFinalContent = debutFichier + "\n" + "\t<JoystickButtons>";
				foreach (var button in emptyJoystickButtonDictionary)
				{
					xmlFinalContent += button.Value.Xml + "\n";
				}

				xmlFinalContent += "\t</JoystickButtons>" + "\n" + finFichier;
				xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(xmlFinalContent);
				UpdateXML();
			}
		}



	}
}
