using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NzbDrone.Common.Processes
{
    public interface ISetProcessAffinity
    {
        void InitializeAffinity();
    }

}
