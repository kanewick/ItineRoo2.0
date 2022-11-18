using DataAccess.Models;
using ItineRoo.WebAPI.Data;
using ItineRoo.WebAPI.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class UserService : IUserService
    {
        private readonly ItineRooContext _context;

        public UserService(ItineRooContext context)
        {
            _context = context;
        }
        public UserModel? GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                return new UserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName

                };
            }

            return null;
        }

        public bool AddUser(UserModel user)
        {
            try
            {
                _context.Users.Add(new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });

                _context.Save();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("USer could be added due too " + e.Message);
            }
        }
    }
}
