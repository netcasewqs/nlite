using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace NLite.Internal
{
    internal static partial class Guard
    {
        [DebuggerStepThrough]
        public static void NotNull(object argumentValue,
                                         string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
        }

       
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string argumentValue,
                                                 string argumentName)
        {
            if (argumentValue == null || argumentValue.Length == 0) throw new ArgumentNullException(argumentName);
        }



        [DebuggerStepThrough]
        internal static void IsFalse(bool condition)
        {
            if (condition)
            {
                Fail(null);
            }
        }

        [DebuggerStepThrough]
        internal static void IsTrue(bool condition)
        {
            if (!condition)
            {
                Fail(null);
            }
        }

        [DebuggerStepThrough]
        internal static void IsTrue(bool condition, [Localizable(false)]string message)
        {
            if (!condition)
            {
                Fail(message);
            }
        }

        [DebuggerStepThrough]
        internal static void Fail([Localizable(false)]string message)
        {
            throw new Exception(message);
        }
    }



    internal class Null
    {
        private Null() { }
        public static readonly Null Value = new Null();
    }
}
