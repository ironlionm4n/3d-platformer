﻿using System;

namespace Platformer
{
    public class FuncPredicate : IPredicate
    {
        private readonly Func<bool> func;
        
        public FuncPredicate(Func<bool> func)
        {
            this.func = func;
        }
        
        public bool Evaluate() => func();
    }
}