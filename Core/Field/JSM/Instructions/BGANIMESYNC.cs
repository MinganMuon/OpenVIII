﻿using System;


namespace OpenVIII
{
    internal sealed class BGANIMESYNC : JsmInstruction
    {
        public BGANIMESYNC()
        {
        }

        public BGANIMESYNC(Int32 parameter, IStack<IJsmExpression> stack)
            : this()
        {
        }

        public override String ToString()
        {
            return $"{nameof(BGANIMESYNC)}()";
        }
    }
}