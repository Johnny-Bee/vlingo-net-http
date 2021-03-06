// Copyright © 2012-2018 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Actors;
using Vlingo.Http.Resource;
using Vlingo.Http.Resource.Serialization;
using Vlingo.Http.Tests.Sample.User.Model;

namespace Vlingo.Http.Tests.Sample.User
{
    public sealed class UserResource : ResourceHandler
    {
        private readonly UserRepository _repository = UserRepository.Instance();

        public UserResource(World world) => Stage = world.StageNamed("service");
        
        public void Register(UserData userData)
        {
            var userAddress = Stage.World.AddressFactory.UniquePrefixedWith("u-");
            var userState =
                UserStateFactory.From(
                    userAddress.IdString,
                    Name.From(userData.NameData.Given, userData.NameData.Family),
                    Contact.From(userData.ContactData.EmailAddress, userData.ContactData.TelephoneNumber));

            Stage.ActorFor<IUser>(Definition.Has<UserActor>(Definition.Parameters(userState)), userAddress);

            _repository.Save(userState);

            Completes.With(Response.Of(
                Response.ResponseStatus.Created,
                Headers.Of(ResponseHeader.Of(ResponseHeader.Location, UserLocation(userState.Id))), 
                JsonSerialization.Serialized(UserData.From(userState))));
        }
        
        public void ChangeContact(string userId, ContactData contactData)
        {
            Stage.ActorOf<IUser>(Stage.World.AddressFactory.From(userId))
                .AndThenTo(user => user.WithContact(new Contact(contactData.EmailAddress, contactData.TelephoneNumber)))
                .OtherwiseConsume(noUser => Completes.With(Response.Of(Response.ResponseStatus.NotFound, UserLocation(userId))))
                .AndThenConsume(userState => Response.Of(Response.ResponseStatus.Ok, JsonSerialization.Serialized(UserData.From(userState))));
        }

        public void ChangeName(string userId, NameData nameData)
        {
            Stage.ActorOf<IUser>(Stage.World.AddressFactory.From(userId))
                .AndThenTo(user => user.WithName(new Name(nameData.Given, nameData.Family)))
                .OtherwiseConsume(noUser => Completes.With(Response.Of(Response.ResponseStatus.NotFound, UserLocation(userId))))
                .AndThenConsume(userState => {
                    _repository.Save(userState);
                    Completes.With(Response.Of(Response.ResponseStatus.Ok, JsonSerialization.Serialized(UserData.From(userState))));
            });
        }

        public void QueryUser(string userId)
        {
            var userState = _repository.UserOf(userId);
            if (userState.DoesNotExist())
            {
                Completes.With(Response.Of(Response.ResponseStatus.NotFound, UserLocation(userId)));
            } else
            {
                Completes.With(Response.Of(Response.ResponseStatus.Ok, JsonSerialization.Serialized(UserData.From(userState))));
            }
        }

        public void QueryUserError(string userId)
        {
            throw new Exception("Test exception");
        }

        public void QueryUsers()
        {
            var users = new List<UserData>();
            foreach (var userState in _repository.Users)
            {
                users.Add(UserData.From(userState));
            }
            
            Completes.With(Response.Of(Response.ResponseStatus.Ok, JsonSerialization.Serialized(users)));
        }
        
        private string UserLocation(string userId)
        {
            return "/users/" + userId;
        }
    }
}