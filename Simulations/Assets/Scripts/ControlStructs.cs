using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ControlStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Electrostatics2D
    {
        public static int SizeOf()
        {
            return 1;
        }
    }
}
