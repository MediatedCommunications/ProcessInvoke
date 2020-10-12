using System;

namespace System {
    internal static class ExceptionExtensions {
        internal static void Ignore<T>(this T ex) {
            ex?.Equals(ex);
        }
    }

}
