using WordSnapWeb.Models;

namespace WordSnapWeb.Services
{
    public class UserSession
    {
        private static UserSession? _instance;
        private static readonly object _lock = new object();

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserSession();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Login(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
