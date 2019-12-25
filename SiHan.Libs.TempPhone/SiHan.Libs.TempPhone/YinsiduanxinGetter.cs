using HtmlAgilityPack;
using SiHan.Libs.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.TempPhone
{
    /// <summary>
    /// 获取www.yinsiduanxin.com网站的手机号
    /// </summary>
    public class YinsiduanxinGetter: ITempPhoneGetter
    {
        private HttpClient HttpClient { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public YinsiduanxinGetter()
        {
            HttpClient = new HttpClient();
            HttpClient.IsEnableFrequency = true;
            HttpClient.Proxy = ProxyFactory.GetSystemWebProxy();
        }
        /// <summary>
        /// 获取临时手机号
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAsync()
        {
            List<string> pList = new List<string>();
            int pageNum = await this.GetPageNums();
            for (int i = 1; i <= pageNum; i++)
            {
                List<string> items = await this.GetPhones(i);
                pList.AddRange(items);
            }
            return pList;
        }

        private async Task<List<string>> GetPhones(int pageNum)
        {
            List<string> phones = new List<string>();
            string url = $"https://www.yinsiduanxin.com/dl/{pageNum}.html";
            HttpResponse response = await this.HttpClient.GetAsync(url);
            if (response.IsSuccess())
            {
                string html = response.GetHtml();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var nodes = document.DocumentNode.SelectNodes("//p[contains(@class, 'card-phone')]");
                if (nodes != null)
                {
                    foreach (var item in nodes)
                    {
                        string phone = item.GetAttributeValue("id", "");
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            phones.Add(phone);
                        }
                    }
                }
            }
            return phones;
        }

        /// <summary>
        /// 获取页数
        /// </summary>
        private async Task<int> GetPageNums()
        {
            HttpRequest request = new HttpRequest("https://www.yinsiduanxin.com/");
            request.Timeout = 30;
            HttpResponse response = await request.SendAsync();
            if (response.IsSuccess())
            {
                string html = response.GetHtml();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var lastPageNode = document.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[last()-1]");
                if (lastPageNode == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(lastPageNode.InnerText);
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
