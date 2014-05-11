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
    using System.Threading;

    using SharpLog.PortableScaffolds;

    using SimpleInjector;
    using SimpleInjector.Extensions;

    public static class Global
    {
        private static Container servicesContainer;
        public static readonly Type[] RequiredTypes = new[] { typeof(IConcurrentDictionary<,>) };

        public static Container Services
        {
            get
            {
                if (servicesContainer == null)
                {
                    Interlocked.CompareExchange(ref servicesContainer, CreateDefaultContainer(), null);
                }
                return servicesContainer;
            }
            set
            {
                if (VerfiyDefaultServices(value))
                {
                    servicesContainer = value;
                }

                throw new ArgumentException(
                    "Services do not contain the registration for the minimum required services",
                    "value");
            }
        }

        private static bool VerfiyDefaultServices(Container container)
        {
            foreach (var type in RequiredTypes)
            {
                if (container.GetRegistration(type) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static Container CreateDefaultContainer()
        {
            var container = new Container();
            container.RegisterOpenGeneric(typeof(IConcurrentDictionary<,>), typeof(ConcurrentDictionary<,>));
            container.Verify();
            return container;
        }
    }
}