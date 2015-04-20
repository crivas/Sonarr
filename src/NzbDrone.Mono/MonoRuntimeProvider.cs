using System;
using System.Reflection;
using Mono.Unix.Native;
using NLog;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Extensions;

namespace NzbDrone.Mono
{
    public class MonoRuntimeProvider : RuntimeInfoBase
    {
        private readonly Logger _logger;

        public MonoRuntimeProvider(Common.IServiceProvider serviceProvider, Logger logger)
            :base(serviceProvider, logger)
        {
            _logger = logger;
        }

        public override String RuntimeVersion
        {
            get
            {
                try
                {
                    var type = Type.GetType("Mono.Runtime");

                    if (type != null)
                    {
                        var displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);

                        if (displayName != null)
                        {
                            return displayName.Invoke(null, null).ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Unable to get mono version: " + ex.Message, ex);
                }

                return String.Empty;
            }
        }

        public Version KernelVersion
        {
            get
            {
                try
                {
                    Utsname results;
                    var res = Syscall.uname(out results);
                    if (res != 0)
                    {
                        _logger.Debug("Failed to retrieve Kernel version. Syscall uname failed.");
                        return null;
                    }

                    Version version;
                    if (results.version.IsNullOrWhiteSpace() || !Version.TryParse(results.version.Replace('-', '.'), out version))
                    {
                        _logger.Debug("Failed to parse kernel version {0}.", results.version);
                        return null;
                    }

                    return version;
                }
                catch (Exception ex)
                {
                    _logger.DebugException("Failed to retrieve Kernel version.", ex);
                    return null;
                }
            }
        }

    }
}
