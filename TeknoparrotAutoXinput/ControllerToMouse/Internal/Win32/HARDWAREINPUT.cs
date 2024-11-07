﻿using System.Runtime.InteropServices;

namespace ControllerToMouseMapper.Internal.Win32;

[StructLayout(LayoutKind.Sequential)]
internal struct HARDWAREINPUT
{
    public int uMsg;
    public short wParamL;
    public short wParamH;
}
