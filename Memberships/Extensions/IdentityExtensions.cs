﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Memberships.Models;

namespace Memberships.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserFirstName(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create();
            var user = db.Users.FirstOrDefault(u => u.UserName.Equals(identity.Name));

            return user != null ? user.FirstName : string.Empty;
        }

        public static async Task GetUsers(this List<UserViewModel> users)
        {
            var db = ApplicationDbContext.Create();

            users.AddRange(await (from user in db.Users
                    select new UserViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName
                    }).OrderBy(m => m.Email).ToListAsync());
        }
    }
}