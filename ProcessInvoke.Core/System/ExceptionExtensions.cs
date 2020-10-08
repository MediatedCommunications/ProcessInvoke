using System;

namespace System {
    internal static class ExceptionExtensions {
        internal static void Ignore(this Exception ex) {
            ex.Equals(ex);
        }
    }

}
