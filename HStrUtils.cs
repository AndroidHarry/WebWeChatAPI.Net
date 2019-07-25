using Leestar54.WeChat.WebAPI.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leestar54.WeChat.WebAPI
{
    public static class HStrUtils
    {
        /// <summary>
        /// 去除 回车 制表 符, 空格
        /// 不能去除 空格, AddMsg 的 Content 字段内部是 xml 信息，去除 空格 后，会导致解析不了 ！！！
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HTrimSpechar(this string str)
        {
            return str.Replace("\n", "").Replace("\t", "").Replace("\r", "");
        }

        public static string ReplaceQuotationMarks(this string str)
        {
            return str.Replace("\"", "\\\"");
        }

        public static string ToJson(this List<MPSubscribeMsg> lst)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var msg in lst)
            {
                sb.Append("{" + msg + "},");
            }

            return $"[{sb.ToString().TrimEnd(',')}]";
        }

        public static string ToJson(this List<AddMsg> lst)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var msg in lst)
            {
                sb.Append("{" + msg + "},");
            }

            return $"[{sb.ToString().TrimEnd(',')}]";
        }
    }
}
