using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Instrumentation;
using System.Text.RegularExpressions;
using NzbDrone.Common.Processes;

namespace NzbDrone.Mono
{
    public class LinuxProcessAffinity : ISetProcessAffinity
    {
        private readonly MonoRuntimeProvider _runtimeProvider;
        private readonly Logger _logger;

        public LinuxProcessAffinity(MonoRuntimeProvider runtimeProvider, Logger logger)
        {
            _runtimeProvider = runtimeProvider;
            _logger = logger;
        }

        [DllImport("libc.so.6", SetLastError = true)]
        protected static extern int sched_setaffinity(long pid, IntPtr num_bytes, byte[] affinity);

        public void InitializeAffinity()
        {
            // TODO: Check Kernel version. 3.13.0-46 and lower are unaffected
            var kernelVersion = _runtimeProvider.KernelVersion;
            if (kernelVersion == null || kernelVersion <= new Version(3, 13, 0, 46))
            {
                return;
            }

            // TODO: Check mono version.

            try
            {
                // Workaround: Force affinity to single core to prevent native mono crashes on specific kernel versions.
                sched_setaffinity(System.Diagnostics.Process.GetCurrentProcess().Id, new IntPtr(1), new byte[] { 1 });
            }
            catch (Exception ex)
            {
                _logger.DebugException("Failed to change processor affinity.", ex);
            }
        }
    }
}
