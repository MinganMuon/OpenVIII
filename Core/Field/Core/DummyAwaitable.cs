﻿using System;

namespace OpenVIII
{
    public sealed class DummyAwaitable : IAwaitable
    {
        public static IAwaitable Instance { get; } = new DummyAwaitable();

        public IAwaiter GetAwaiter()
        {
            return DummyAwaiter.Instance;
        }
    }
}