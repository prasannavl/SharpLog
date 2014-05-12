﻿// Author: Prasanna V. Loganathar
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
// Created: 3:43 PM 09-05-2014

namespace SharpLog.PortabilityScaffolds
{
    using System;
    using System.Collections.Generic;

    public interface IConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        bool IsEmpty { get; }
        bool TryAdd(TKey key, TValue value);

        TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory);

        TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory);
        bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
        TValue GetOrAdd(TKey key, TValue value);
        bool TryRemove(TKey key, out TValue value);
        KeyValuePair<TKey, TValue>[] ToArray();
    }
}