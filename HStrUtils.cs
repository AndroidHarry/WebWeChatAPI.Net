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
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HTrim(this string str)
        {
            return str.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
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

        public static string Fixml(this string xml)
        {
            return xml;
        }
    }
}
