using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unk.Biz.Entity;
using Dapper;
using System.Data.SqlClient;

namespace Unk.Biz
{
    public class GainsInHistoryBiz
    {
        public List<EveryDataViewGainsEntity> GetEveryDataPrice()
        {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                var _results = new List<EveryDataViewGainsEntity>();
                var aList = conn.Query<TokenStoreEntity>("SELECT * FROM [TokenStore]");

                foreach (var item in aList)
                {
                    EveryDataViewGainsEntity entity = new EveryDataViewGainsEntity();
                    var rList = conn.Query<GainsInHistoryEntity>($@"select top(2) b.TokenID,b.CurrentPrice,b.CreateTime from dbo.TokenStore as a 
left join
GainsInHistory as b 
on a.TokenHearderText = b.TokenID
where a.TokenHearderText = '{item.TokenHearderText}'
order by b.CreateTime desc").ToList();
                    entity.TokenID = item.TokenHearderText;
                    entity.TokenIcon = item.TokenIco;
                    entity.CurrentDescript = item.TokenDescription;
                    entity.CurrentPrice = rList[0].CurrentPrice;
                    entity.YesterdayPrice = rList[1].CurrentPrice;
                    entity.IncreaseThan = Math.Round(((entity.CurrentPrice - entity.YesterdayPrice) / entity.YesterdayPrice) * 100, 2);
                    _results.Add(entity);
                }

                return _results;
            }
        }

        public List<TokenDetailsEntity> GetUserTokenList(int p_UserID) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                var rList = conn.Query<TokenDetailsEntity>($@"
SELECT * FROM TokenDetails WHERE UserID = ${p_UserID}
").ToList();
                rList = rList.GroupBy(x => x.TokenType).Select(x => new TokenDetailsEntity()
                {
                    TokenType = x.FirstOrDefault().TokenType,
                    CurrentIcon = x.Sum(y => y.CurrentIcon)
                }).ToList();
                return rList;
            }
        }

        public List<GainsInHistoryEntity> GetGainsInHisotryList(string p_type) {
            using (SqlConnection conn = new SqlConnection(Core.Utils.SqlConnectionString))
            {
                return conn.Query<GainsInHistoryEntity>($@"SELECT * FROM [GainsInHistory]
  where TokenID = '{p_type}'
  order by CreateTime desc").ToList();
            }
        }
    } 
}
