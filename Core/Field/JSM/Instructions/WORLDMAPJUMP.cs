﻿using System;


namespace OpenVIII
{
    internal sealed class WORLDMAPJUMP : JsmInstruction
    {
        private IJsmExpression _arg0;
        private IJsmExpression _arg1;
        private IJsmExpression _arg2;

        public WORLDMAPJUMP(IJsmExpression arg0, IJsmExpression arg1, IJsmExpression arg2)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public WORLDMAPJUMP(Int32 parameter, IStack<IJsmExpression> stack)
            : this(
                arg2: stack.Pop(),
                arg1: stack.Pop(),
                arg0: stack.Pop())
        {
        }

        public override String ToString()
        {
            return $"{nameof(WORLDMAPJUMP)}({nameof(_arg0)}: {_arg0}, {nameof(_arg1)}: {_arg1}, {nameof(_arg2)}: {_arg2})";
        }
    }
}