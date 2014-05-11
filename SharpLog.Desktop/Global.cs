using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog.Desktop
{
    using SharpLog.Desktop.PortablilityScaffolds;
    using SharpLog.PortablilityScaffolds;

    using SimpleInjector;
    using SimpleInjector.Extensions;

    public static class Global
    {
        public static Container Services
        {
            get
            {
                return GetPlatformContainer();
            }
        }

        private static Container GetPlatformContainer()
        {
            var container = new Container();
            container.RegisterOpenGeneric(typeof(IConcurrentDictionary<,>), typeof(ConcurrentDictionaryFacade<,>));
            container.Verify();
            return container;
        }
    }
}
