using WADProject1.Models;

namespace WADProject1.Services
{
    public interface IUserService
    {
        User CurrentUser { get; set; }
        void SetCurrentUser(User user);
    }

    public class UserService : IUserService
    {
        public User CurrentUser { get; set; }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }
    }

}
