﻿// Copyright (c) 2012-2019 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Actors;

namespace Vlingo.Http.Resource
{
    public class ResourceRequestHandlerActor : Actor, IResourceRequestHandler
    {
        private readonly ResourceHandler _resourceHandler;

        public ResourceRequestHandlerActor(ResourceHandler resourceHandler)
        {
            _resourceHandler = resourceHandler;
        }
        
        public void HandleFor<T>(Context context, Action<T>? consumer) where T : ResourceHandler
        {
            try
            {
                _resourceHandler.Context = context;
                _resourceHandler.Stage = Stage;
                consumer?.Invoke((T)_resourceHandler);
            }
            catch (Exception e)
            {
                Logger.Error("Error thrown by resource dispatcher", e);
                context.Completes.With(Response.Of(Response.ResponseStatus.InternalServerError));
            }
        }

        public void HandleFor(Context context, Action.MappedParameters mappedParameters, RequestHandler handler)
        {
            Action<ResourceHandler> consumer = resource =>
                handler
                .Execute(context.Request!, mappedParameters, resource.Logger!)
                .AndThen(outcome => RespondWith(context, outcome))
                .Otherwise<Response>(failure => RespondWith(context, failure))
                .RecoverFrom(exception => Response.Of(Response.ResponseStatus.BadRequest, exception.Message));

            HandleFor(context, consumer);
        }
        
        private Response RespondWith(Context context, Response response)
        {
            context.Completes.With(response);
            return response;
        }
    }
}