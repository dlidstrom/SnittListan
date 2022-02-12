﻿#nullable enable

using Raven.Client.Documents.Indexes;
using Snittlistan.Web.Models;

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
