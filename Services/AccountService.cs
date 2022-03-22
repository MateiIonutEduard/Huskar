using Huskar.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
#pragma warning disable

namespace Huskar.Services
{
    public class AccountService : IAccountService
    {
        readonly MovieContext movieContext;
        public AccountService(MovieContext movieContext)
        { this.movieContext = movieContext; }

        public async Task<User> GetUser(string name, int auth)
        {
            var user = await movieContext.Users.
                FirstOrDefaultAsync(u => u.Name.CompareTo(name) == 0 && u.AuthModel == auth);

            return user;
        }

        public async Task FacebookSignIn(string id, string name, string token)
        {
            string profile = await FacebookProfile(id, token);
            var user = movieContext.Users.FirstOrDefault(u => u.Name.CompareTo(name) == 0 && u.AuthModel == 1);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Profile = profile,
                    AuthModel = 1
                };

                movieContext.Users.Add(user);
                await movieContext.SaveChangesAsync();
            }
            else
            {
                if(user.Profile.CompareTo(profile) != 0)
                {
                    user.Profile = profile;
                    await movieContext.SaveChangesAsync();
                }
            }
        }

        public async Task GoogleSignIn(string name, string profile)
        {
            var user = movieContext.Users.FirstOrDefault(u => u.Name.CompareTo(name) == 0 && u.AuthModel == 2);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Profile = profile,
                    AuthModel = 2
                };

                movieContext.Users.Add(user);
                await movieContext.SaveChangesAsync();
            }
            else
            {
                if(user.Profile.CompareTo(profile) != 0)
                {
                    user.Profile = profile;
                    await movieContext.SaveChangesAsync();
                }
            }
        }

        public async Task InstagramSignIn(string name)
        {
            var user = movieContext.Users.FirstOrDefault(u => u.Name.CompareTo(name) == 0 && u.AuthModel == 3);
            var picture = await InstagramProfile(name);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Profile = picture,
                    AuthModel = 3
                };

                movieContext.Users.Add(user);
                await movieContext.SaveChangesAsync();
            }
            else
            {
                if(user.Profile.CompareTo(picture) != 0)
                {
                    user.Profile = picture;
                    await movieContext.SaveChangesAsync();
                }
            }
        }

        public async Task<string> FacebookProfile(string id, string token)
        {
            string str = $"https://graph.facebook.com/{id}?fields=picture&access_token={token}";
            var client = new HttpClient();
            var res = await client.GetAsync(str);
            var data = await res.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(data);
            return obj.picture.data.url.ToString();
        }

        public async Task<string> InstagramProfile(string name)
        {
            string str = $"https://www.instagram.com/{name}/?__a=1";
            var client = new HttpClient();
            var res = await client.GetAsync(str);
            var data = await res.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(data);
            return obj.graphql.user.profile_pic_url.ToString();
        }
    }
}
