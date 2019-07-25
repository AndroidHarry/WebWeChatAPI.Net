using Leestar54.WeChat.WebAPI.Modal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Leestar54.WeChat.WebAPI
{
    public static class HXmlUtils
    {
        public static void testXml()
        {
            while (true)
            {
                string filename = @"G:\_tmp\msg.xml";

                string sc = File.ReadAllText(filename);

                sc = sc.Replace("\\\"", "\"");

                XmlContent xc = new XmlContent(sc);
                xc.Proc();
            }
        }


        public static MPSubscribeMsg ExtractMPSubscribeMsg(this AddMsg msg)
        {
            if (msg.MsgType != MsgType.MM_DATA_APPMSG)
                return null;

            XmlContent xc = new XmlContent(msg.Content);
            xc.Proc();

            MPSubscribeMsg mpMsg = xc.mpSubscribeMsg;
            if (mpMsg != null)
            {
                mpMsg.UserName = msg.FromUserName;
            }

            return mpMsg;
        }

        public static List<MPSubscribeMsg> ExtractMPSubscribeMsg(this List<AddMsg> msgLst)
        {
            List<MPSubscribeMsg> lst = new List<MPSubscribeMsg>();

            foreach (AddMsg am in msgLst)
            {
                MPSubscribeMsg msg = am.ExtractMPSubscribeMsg();
                if (msg != null)
                {
                    lst.Add(msg);
                }
            }

            return lst.Count > 0 ? lst : null;
        }

        class XmlContent
        {
            public string src { get; set; }

            public MPSubscribeMsg mpSubscribeMsg { get; set; }

            public XmlContent(string xml)
            {
                src = xml;
            }

            public void Proc()
            {
                try
                {
                    string xml = WebUtility.HtmlDecode(src);

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(xml);

                    XmlNode mmreaderNode = xmldoc.SelectSingleNode("/msg/appmsg/mmreader");
                    XmlNode categoryNode = mmreaderNode.SelectSingleNode("category");

                    string scount = categoryNode.Attributes["count"].Value;
                    int count = 0;
                    int.TryParse(scount, out count);

                    XmlNodeList xnl = categoryNode.SelectNodes("item");
                    if (xnl != null && xnl.Count == count)
                    {
                        mpSubscribeMsg = new MPSubscribeMsg();
                        List<MPArticle> lstMPArticle = new List<MPArticle>();
                        mpSubscribeMsg.MPArticleList = lstMPArticle;

                        mpSubscribeMsg.NickName = mmreaderNode.SelectSingleNode("publisher/nickname").InnerText;

                        foreach (XmlNode n in xnl)
                        {
                            MPArticle mpa = new MPArticle();

                            mpa.Title = n.SelectSingleNode("title").InnerText;
                            mpa.Digest = n.SelectSingleNode("digest").InnerText;
                            mpa.Cover = n.SelectSingleNode("cover").InnerText;
                            mpa.Url = n.SelectSingleNode("url").InnerText;

                            lstMPArticle.Add(mpa);

                            if (mpSubscribeMsg.Time == 0)
                            {
                                string s_pub_time = n.SelectSingleNode("pub_time").InnerText;
                                long pub_time = 0;
                                long.TryParse(s_pub_time, out pub_time);
                                mpSubscribeMsg.Time = pub_time;
                            }
                        }

                        mpSubscribeMsg.MPArticleCount = lstMPArticle.Count;
                    }
                    else
                    {
                        HTrace.Write("XmlContent:Proc parse count error!");
                    }
                }
                catch (Exception ex)
                {
                    HTrace.Write(ex.Message);
                }
            }
        }
    }
}
