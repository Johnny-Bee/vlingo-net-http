﻿// Copyright (c) 2012-2019 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Http
{
    public class Method
    {
        private readonly string _name;

        private Method(string name)
        {
            _name = name;
        }

        public static Method CONNECT => new Method("CONNECT");
        public static Method DELETE => new Method("DELETE");
        public static Method GET => new Method("GET");
        public static Method HEAD => new Method("HEAD");
        public static Method OPTIONS => new Method("OPTIONS");
        public static Method PATCH => new Method("PATCH");
        public static Method POST => new Method("POST");
        public static Method PUT => new Method("PUT");
        public static Method TRACE => new Method("TRACE");

        public static Method From(string methodNameText)
        {
            var name = (methodNameText ?? string.Empty).ToUpperInvariant();
            switch (name)
            {
                case "CONNECT":
                    return CONNECT;
                case "DELETE":
                    return DELETE;
                case "GET":
                    return GET;
                case "HEAD":
                    return HEAD;
                case "OPTIONS":
                    return OPTIONS;
                case "PATCH":
                    return PATCH;
                case "POST":
                    return POST;
                case "PUT":
                    return PUT;
                case "TRACE":
                    return TRACE;
                default:
                    throw new ArgumentException($"{Response.ResponseStatus.MethodNotAllowed.GetDescription()}\n\n${methodNameText}");
            }
        }

        public bool IsCONNECT() => MethodEquals("CONNECT");
        public bool IsDELETE() => MethodEquals("DELETE");
        public bool IsGET() => MethodEquals("GET");
        public bool IsHEAD() => MethodEquals("HEAD");
        public bool IsOPTIONS() => MethodEquals("OPTIONS");
        public bool IsPATCH() => MethodEquals("PATCH");
        public bool IsPOST() => MethodEquals("POST");
        public bool IsPUT() => MethodEquals("PUT");
        public bool IsTRACE() => MethodEquals("TRACE");

        private bool MethodEquals(string name) => string.Equals(_name, name, StringComparison.InvariantCultureIgnoreCase);
    }
}
