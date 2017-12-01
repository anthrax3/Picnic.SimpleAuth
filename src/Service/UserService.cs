using Microsoft.AspNetCore.Http;
using Picnic.Service;
using Picnic.SimpleAuth.Model;
using Picnic.Stores;

namespace Picnic.SimpleAuth.Service
{
    public class DefaultUserService : GenericService<User>, IUserService
    {
        /// <summary>
        /// ctor the Mighty
        /// </summary>
        public DefaultUserService(IGenericStore<User> store, IHttpContextAccessor httpContextAccessor) : base(store, httpContextAccessor) { }
    }
}