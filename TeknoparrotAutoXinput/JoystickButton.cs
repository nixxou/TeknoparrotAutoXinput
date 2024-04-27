public class JoystickButtonData
{
	public int Button { get; set; }
	public bool IsAxis { get; set; }
	public bool IsAxisMinus { get; set; }
	public bool IsFullAxis { get; set; }
	public int PovDirection { get; set; }
	public bool IsReverseAxis { get; set; }
	public Guid JoystickGuid { get; set; }

	public string Title { get; set; }
	public string XinputTitle { get; set; }

	public string DeviceName { get; set; } = string.Empty;

}
