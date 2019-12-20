﻿// Copyright (c) 2012-2019 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vlingo.Actors;

namespace Vlingo.Http.Resource
{
    public class Resources
    {
        private readonly IDictionary<string, Resource> _namedResources;

        public static Resources Are(params Resource[] resources)
        {
            var all = new Resources();
            foreach(var resource in resources)
            {
                all._namedResources[resource.Name] = resource;
            }

            return all;
        }

        private Resources()
        {
            _namedResources = new Dictionary<string, Resource>();
        }

        internal Resources(IDictionary<string, Resource> namedResource)
        {
            _namedResources = new ReadOnlyDictionary<string, Resource>(namedResource);
        }

        internal Resources(Resource resource)
        {
            _namedResources = new Dictionary<string, Resource>();
            _namedResources[resource.Name] = resource;
        }

        public IEnumerable<Resource> ResourceHandlers => _namedResources.Values;
        
        public IDictionary<string, Resource> NamedResources => _namedResources;

        public override string ToString()
            => $"Resources[namedResource={_namedResources}]";

        internal void DispatchMatching(Context context, ILogger logger)
        {
            string message;

            try
            {
                foreach (var resource in _namedResources.Values)
                {
                    var matchResults = resource.MatchWith(context.Request?.Method, context.Request?.Uri);
                    if (matchResults.IsMatched)
                    {
                        var mappedParameters = matchResults.Action?.Map(context.Request, matchResults.Parameters);
                        resource.DispatchToHandlerWith(context, mappedParameters);
                        return;
                    }
                }
                message = $"No matching resource for method {context.Request?.Method} and Uri {context.Request?.Uri}";
                logger.Warn(message);
            }
            catch (Exception e)
            {
                message = $"Problem dispatching request for method {context.Request?.Method} and Uri {context.Request?.Uri} because: {e.Message}";
                logger.Error(message, e);
            }

            context.Completes.With(Response.Of(Response.ResponseStatus.NotFound, message));
        }
    }
}
