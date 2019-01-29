using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unk.Biz;
using Unk.Biz.Entity;
using Unk.Core;
using Unk.WebApi.ViewModel;

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
        public object GetTokenSummary() {
            return new
            {
                Data = g_GainsInHistoryBiz.GetEveryDataPrice()
            };
        }

        [HttpGet]
        public object GetGainsInHisotry(string p_type) {
            return new
            {
                Data = g_GainsInHistoryBiz.GetGainsInHisotryList(p_type)
            };
        }

    }
}