﻿using Raven.Client.Indexes;
using Snittlistan.Web.Models;

#nullable enable

namespace Snittlistan.Web.Infrastructure.Indexes;
public class User_ByEmail : AbstractIndexCreationTask<User>
{
    public User_ByEmail()
    {
        Map = users => from user in users
                       select new
                       {
                           user.Email,
                           user.ActivationKey
                       };
    }
}
