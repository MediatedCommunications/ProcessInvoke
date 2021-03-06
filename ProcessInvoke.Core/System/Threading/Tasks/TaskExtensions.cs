﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks {
    internal static class TaskExtensions {

        internal static ConfiguredTaskAwaitable DefaultAwait(this Task This) {
            return This.RunOnAnyThread();
        }

        internal static ConfiguredTaskAwaitable<T> DefaultAwait<T>(this Task<T> This) {
            return This.RunOnAnyThread();
        }

        private const bool __RunOnAnyThread = false;

#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
        internal static ConfiguredTaskAwaitable RunOnAnyThread(this Task This) {
            return This.ConfigureAwait(__RunOnAnyThread);
        }

        internal static ConfiguredTaskAwaitable<T> RunOnAnyThread<T>(this Task<T> This) {
            return This.ConfigureAwait(__RunOnAnyThread);
        }
#pragma warning restore 

    }

}
