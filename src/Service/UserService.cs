using Picnic.Service;
using Picnic.SimpleAuth.Model;
using Picnic.Stores;

namespace Picnic.SimpleAuth.Service
{
    public interface IUserService : IGenericService<User>
    {
        
    }

    public class DefaultUserService : GenericService<User>, IUserService
    {
        /// <summary>
        /// ctor the Mighty
        /// </summary>
        public DefaultUserService(IGenericStore<User> store) : base(store)
        {

        }
    }
}