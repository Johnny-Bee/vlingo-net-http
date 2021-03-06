﻿// Copyright (c) 2012-2019 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Http.Resource
{
    public static class DefaultErrorHandler
    {
        private static Func<Exception, Response> Handler = ex =>
        {
            if (ex is MediaTypeNotSupportedException)
            {
                return Response.Of(Response.ResponseStatus.UnsupportedMediaType);
            }
            else if (ex is ArgumentException)
            {
                return Response.Of(Response.ResponseStatus.BadRequest);
            }
            else
            {
                return Response.Of(Response.ResponseStatus.InternalServerError);
            }
        };

        public static IErrorHandler Instance { get; } = new ErrorHandlerImpl(Handler);
    }
}
