using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace OrleansModel
{
    public static class Util
    {
        public static void Ldarg(this ILGenerator il, int i)
        {
            switch (i)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (i <= byte.MaxValue)
                    {
                        il.Emit(OpCodes.Ldarg_S, (byte)i);
                        break;
                    }
                    il.Emit(OpCodes.Ldarg, i);
                    break;
            }
        }
    }
}
