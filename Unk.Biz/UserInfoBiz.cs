using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Unk.Core.ViewModel;

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

        public Entity.UserInfoEntity GetUserSingle(string p_id) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo WHERE id = {p_id}").FirstOrDefault();
            }
        }
        public Entity.UserInfoEntity GetUserByPhone(string p_phone)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM UserInfo WHERE UserPhone = '{p_phone}'").FirstOrDefault();
            }
        }
        public Entity.UserInfoEntity UserLogin(string p_User, string p_Pwd)
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<Entity.UserInfoEntity>($@"SELECT * FROM [UNK].[dbo].[UserInfo] where UserPhone = '{p_User}' and UserPwd = '{p_Pwd}'").FirstOrDefault();
            }
        }

        public bool  RegUser(RegNewUserModels model)
        {
            var v_RefUser = GetUserSingle(model.pUserID);
            string CurrentID = string.Empty;
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                conn.Execute($@"
INSERT INTO [dbo].[UserInfo]
           ([UserName]
           ,[UserPhone]
           ,[UserPwd]
           ,[UserSex]
           ,[IDCard]
           ,[IDName]
           ,[IDBirthday]
           ,[Referrer]
           ,[Status]
)
     VALUES
           (
            '{model.CardName}'
           ,'{model.UserName}'
           ,'{model.UserPwd}'
           ,0
           ,'{model.CardNo}'
           ,'{model.CardNo}'
           ,''
           ,'{v_RefUser.UserPhone}'
           ,1
           )

");
                CurrentID = conn.Query<string>($@"SELECT ID FROM UserInfo WHERE UserPhone = '{model.UserName}'").FirstOrDefault();
            }

            new GainsInHistoryBiz().UpdateAccountCoin("UNK", CurrentID, "40", "新用户注册");
            new GainsInHistoryBiz().UpdateAccountCoin("UNK", model.pUserID, "20", $"推荐 {model.UserName} 新用户注册 奖励20");
            return true;
        }
    }
}
