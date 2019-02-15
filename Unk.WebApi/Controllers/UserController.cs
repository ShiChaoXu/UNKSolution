using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unk.Biz;
using Unk.Biz.Entity;
using Unk.Core;
using Unk.Core.ViewModel;

namespace Unk.WebApi.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly UserInfoBiz g_UserInfoBiz = new UserInfoBiz();
        private readonly GainsInHistoryBiz g_GainsInHistoryBiz = new GainsInHistoryBiz();

        [HttpGet]
        public object GetAll()
        {
            var v_rList = g_UserInfoBiz.GetUserList();
            return new
            {
                Data = v_rList
            };
        }

        [HttpPost]
        public object Exist([FromBody] UserViewModels p_UserView)
        {
            UserInfoEntity v_user = CacheHelper.GetCache(CacheHelper.USER + p_UserView.p_UserName) as UserInfoEntity;
            List<TokenDetailsEntity> tokenList = new List<TokenDetailsEntity>();
            if (v_user == null)
            {
                v_user = g_UserInfoBiz.UserLogin(p_UserView.p_UserName, p_UserView.p_UserPwd);
                if (v_user != null)
                {
                    tokenList = g_GainsInHistoryBiz.GetUserTokenList(v_user.ID);
                }
            }
            return new {
                Data = new
                {
                    IsExist = v_user != null,
                    User = v_user,
                    TokenList = tokenList
                }
            };
        }

        [HttpGet]
        public object GetUserByPhone(string p_phone)
        {
            return new
            {
                Data = g_UserInfoBiz.GetUserByPhone(p_phone)
            };
        }

        [HttpGet]
        public object GetUserSingle(string p_id)
        {
            return new
            {
                Data = g_UserInfoBiz.GetUserSingle(p_id)
            };
        }

        [HttpGet]
        public object GetTokenSummary() {
            return new
            {
                Data = g_GainsInHistoryBiz.GetEveryDataPrice()
            };
        }

        [HttpGet]
        public object GetTokenTotalCount(string p_type, string p_userid)
        {
            return new {
                Data = new
                {
                    TotalCount = g_GainsInHistoryBiz.GetTokenCount(p_type, p_userid),
                    HasSignIn = !g_GainsInHistoryBiz.CheckHasSign(p_type, p_userid)
                }
            };
        }

        [HttpGet]
        public object GetGainsInHisotry(string p_type) {
            return new
            {
                Data = g_GainsInHistoryBiz.GetGainsInHisotryList(p_type)
            };
        }

        [HttpPost]
        public object UpdateAccountCoin([FromBody] UpdateAccountModels p_UserView)
        {
            return new
            {
                Data = g_GainsInHistoryBiz.UpdateAccountCoin(p_UserView.p_type, p_UserView.p_userid, p_UserView.p_total, p_UserView.p_desc)
            };
        }

        [HttpPost]
        public object RegNewUser([FromBody] RegNewUserModels p_UserView)
        {
            bool status = false;
            string message = "";
            if (g_UserInfoBiz.GetUserSingle(p_UserView.pUserID) == null)
            {
                status = false;
                message = "二维码已失效,请重新联系推荐人";
            }
            else
            {
                if (g_UserInfoBiz.GetUserByPhone(p_UserView.UserName) == null)
                {
                    if (g_UserInfoBiz.RegUser(p_UserView) == false)
                    {
                        status = false;
                        message = "系统故障, 请联系管理员";
                    }
                    else
                    {
                        status = true;
                        message = $"{p_UserView.UserName} 注册成功, 请登录系统!";
                    }
                }
                else {
                    status = false;
                    message = $"{p_UserView.UserName} 该用户已存在, 请输入新手机号.";
                }
            }
            return new
            {
                Data = status,
                Message = message
            };
        }

    }
}