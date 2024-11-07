using System.Runtime.InteropServices;

namespace ControllerToMouseMapper.Internal.Win32;

[StructLayout(LayoutKind.Explicit)]
internal struct InputUnion
{

    [FieldOffset(0)]
    public MOUSEINPUT mi;

    [FieldOffset(0)]
    public KEYBDINPUT ki;

    [FieldOffset(0)]
    public HARDWAREINPUT hi;

}