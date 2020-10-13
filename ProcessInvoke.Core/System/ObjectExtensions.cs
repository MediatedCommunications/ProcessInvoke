using System;

namespace System {
    internal static class ObjectExtensions {
        internal static void Ignore<T>(this T ex) {
            ex?.Equals(ex);
        }
    }

}
