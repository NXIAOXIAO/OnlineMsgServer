using OnlineMsgServer.Common;

namespace OnlineMsgServer.Core
{
    class UserService
    {
        #region 服务器用户管理
        private static readonly List<User> _UserList = [];
        private static readonly object _UserListLock = new();

        /// <summary>
        /// 通过wsid添加用户记录
        /// </summary>
        public static void AddUserConnect(string wsid)
        {
            lock (_UserListLock)
            {
                User user = new(wsid);
                _UserList.Add(user);
            }
        }
        /// <summary>
        /// 通过wsid移除用户记录
        /// </summary>
        /// <param name="wsid"></param>
        public static void RemoveUserConnectByID(string wsid)
        {
            lock (_UserListLock)
            {
                User? user = _UserList.Find(u => u.ID == wsid);
                if (user != null)
                {
                    _UserList.Remove(user);
                }
            }
        }

        /// <summary>
        /// 通过publickey返回用户列表
        /// </summary>
        public static List<User> GetUserListByPublicKey(string publicKey)
        {
            lock (_UserListLock)
            {
                return _UserList.FindAll(u => u.PublicKey == publicKey);
            }
        }


        /// <summary>
        /// 通过wsid设置用户PublicKey
        /// </summary>
        public static void UserLogin(string wsid, string publickey, string name)
        {
            lock (_UserListLock)
            {
                User? user = _UserList.Find(u => u.ID == wsid);
                if (user != null)
                {
                    user.PublicKey = publickey;
                    user.Name = name;
                    Console.WriteLine(user.ID + " 登记成功");
                }
                else
                {
                    throw new Exception("用户不存在");
                }
            }
        }

        /// <summary>
        /// 通过wsid获取用户PublicKey
        /// </summary>
        public static string? GetUserPublicKeyByID(string wsid)
        {
            lock (_UserListLock)
            {
                User? user = _UserList.Find(u => u.ID == wsid);
                if (user != null)
                {
                    return user.PublicKey;
                }
                return null;
            }
        }

        /// <summary>
        /// 通过wsid获取UserName
        /// </summary>
        public static string? GetUserNameByID(string wsid)
        {
            lock (_UserListLock)
            {
                User? user = _UserList.Find(u => u.ID == wsid);
                if (user != null)
                {
                    return user.Name;
                }
                return null;
            }
        }

        /// <summary>
        /// 通过用户PublicKey获取wsid
        /// </summary>
        public static string? GetUserIDByPublicKey(string publicKey)
        {
            lock (_UserListLock)
            {
                User? user = _UserList.Find(u => u.PublicKey == publicKey);
                if (user != null)
                {
                    return user.ID;
                }
                return null;
            }
        }

        #endregion
    }
}