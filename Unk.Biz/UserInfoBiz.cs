using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace Unk.Biz
{
    public class UserInfoBiz
    {
        public List<Entity.UserInfoEntity> GetUserList()
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo").ToList();
            }
        }

        public Entity.UserInfoEntity GetUserSingle(string p_Phone) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UesrInfo WHERE UserPhone = '{p_Phone}'").FirstOrDefault();
            }
        }
        public Entity.UserInfoEntity UserLogin(string p_User, string p_Pwd)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM [UNK].[dbo].[UserInfo] where UserPhone = '{p_User}' and UserPwd = '{p_Pwd}'").FirstOrDefault();
            }
        }
    }
}
