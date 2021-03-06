// Copyright © 2012-2020 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Http.Resource;
using Vlingo.Http.Resource.Serialization;

namespace Vlingo.Http.Tests.Resource
{
    public class TestMapper : IMapper
    {
        public object From(string data, Type type) => JsonSerialization.Deserialized(data, type);

        public string From<T>(T data) => JsonSerialization.Serialized(data);
    }
}