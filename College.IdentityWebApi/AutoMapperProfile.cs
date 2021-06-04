using AutoMapper;
using College.IdentityWebApi.Domain;
using College.IdentityWebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace College.IdentityWebApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<User, UserLoginModel>().ReverseMap();
        }
    }
}
