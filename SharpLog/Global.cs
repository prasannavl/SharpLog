// Author: Prasanna V. Loganathar
// Project: SharpLog
// 
// Copyright 2014 Launchark. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//  
// Created: 2:06 AM 10-05-2014

namespace SharpLog
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using SharpLog.PortabilityScaffolds;

    using SimpleInjector;

    public static class Global
    {
        private static readonly string[] PlatformSupportAssemblyNames = { "SharpLog.Desktop" };

        private static Container container;

        public static Container Container
        {
            get
            {
                if (container == null)
                {
                    Interlocked.CompareExchange(ref container, ConstructPlatformContainer(), null);
                }
                return container;
            }
            set
            {
                container = value;
            }
        }

        private static Container ConstructPlatformContainer()
        {
            var platformContainer = new Container();
            platformContainer.Options.AllowOverridingRegistrations = true;

            var platformBootstrapperTypeInfoCache = typeof(IPlatformBootstrap).GetTypeInfo();

            var defaultBootStrapper = new DefaultBootstrap();
            defaultBootStrapper.RegisterPlatformServices(platformContainer);

            foreach (var assemblyName in PlatformSupportAssemblyNames)
            {
                var assembly = Assembly.Load(new AssemblyName(assemblyName));
                if (assembly != null)
                {
                    var bootstrapper =
                        (IPlatformBootstrap)
                        assembly.DefinedTypes.Where(platformBootstrapperTypeInfoCache.IsAssignableFrom)
                            .Select(x => Activator.CreateInstance(x.AsType()))
                            .FirstOrDefault();
                    if (bootstrapper != null)
                    {
                        bootstrapper.RegisterPlatformServices(platformContainer);
                    }
                }
            }

            platformContainer.Verify();
            return platformContainer;
        }
    }
}