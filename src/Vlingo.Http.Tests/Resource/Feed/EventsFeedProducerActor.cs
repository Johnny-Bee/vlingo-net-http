// Copyright © 2012-2018 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Text;
using Vlingo.Actors;
using Vlingo.Http.Resource.Feed;

namespace Vlingo.Http.Tests.Resource.Feed
{
    public class EventsFeedProducerActor : Actor, IFeedProducer
    {
        public void ProduceFeedFor(FeedProductRequest request)
        {
            var body =
                new StringBuilder()
                    .Append(request.FeedName)
                    .Append(":")
                    .Append(request.FeedProductId)
                    .Append(":");
            
            for (var count = 1; count <= request.FeedProductElements; ++count)
            {
                body.Append(count).Append("\n");
            }
            
            var response = Response.Of(Response.ResponseStatus.Ok, body.ToString());
            request.Context.Completes.With(response);
        }
    }
}