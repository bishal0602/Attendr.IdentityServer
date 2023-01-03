using AutoMapper;

namespace Attendr.IdentityServer.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.UserRegistrationDto, Entities.User>();
        }
    }
}
