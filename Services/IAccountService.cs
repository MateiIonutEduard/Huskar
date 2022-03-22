using Huskar.Data;

namespace Huskar.Services
{
    public interface IAccountService
    {
        public Task<User> GetUser(string name, int auth);
        public Task FacebookSignIn(string id, string name, string token);
        public Task GoogleSignIn(string name, string profile);
        public Task InstagramSignIn(string name);
        public Task<string> FacebookProfile(string id, string token);
        public Task<string> InstagramProfile(string name);
    }
}
