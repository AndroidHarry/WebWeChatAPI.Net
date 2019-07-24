using Leestar54.WeChat.WebAPI.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leestar54.WeChat.WebAPI.Modal
{

    /// <summary>
    /// 公众号文章消息
    /// </summary>
    public class MPSubscribeMsg
    {
        /// <summary>
        /// 公众号UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 公众号文章数量
        /// </summary>
        public int MPArticleCount { get; set; }
        /// <summary>
        /// 公众号文章
        /// </summary>
        public List<MPArticle> MPArticleList { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public Int64 Time { get; set; }
        /// <summary>
        /// 公众号昵称
        /// </summary>
        public string NickName { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var mpa in MPArticleList)
            {
                sb.Append("{" + mpa + "},");
            }

            return $"\"UserName\":\"{UserName.HTrim()}\", \"MPArticleCount\":{MPArticleCount}, \"Time\":{Time}, \"NickName\":\"{NickName.HTrim()}\", \"MPArticleList\":[{sb.ToString().TrimEnd(',')}]";
        }
    }

}
