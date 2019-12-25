using HtmlAgilityPack;
using SiHan.Libs.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiHan.Libs.TempPhone
{
    /// <summary>
    /// yunduanxin网站临时手机号获取器
    /// </summary>
    public class YunDuanXinGetter: ITempPhoneGetter
    {
        private HttpClient HttpClient { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public YunDuanXinGetter()
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
            int num = await this.GetPageNums();
            List<string> allList = new List<string>();
            for (int i = 1; i <= num; i++)
            {
                List<string> itemList = await this.GetPhones(i);
                allList.AddRange(itemList);
            }
            return allList;
        }

        private async Task<List<string>> GetPhones(int num)
        {
            List<string> phones = new List<string>();
            string url = $"https://yunduanxin.net/China-Phone-Number/Page/{num}";
            HttpResponse response = await this.HttpClient.GetAsync(url);
            if (response.IsSuccess())
            {
                string html = response.GetHtml();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var nodes = document.DocumentNode.SelectNodes("//h4[@class='number-boxes-item-number']");
                if (nodes != null)
                {
                    foreach (var item in nodes)
                    {
                        string nodeText = item.InnerText;
                        nodeText = nodeText.TrimStart('+', '8', '6');
                        nodeText = nodeText.Trim();
                        phones.Add(nodeText);
                    }
                }
            }
            return phones;
        }

        private async Task<int> GetPageNums()
        {
            HttpResponse response = await this.HttpClient.GetAsync("https://yunduanxin.net/China-Phone-Number/");
            if (response.IsSuccess())
            {
                string html = response.GetHtml();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var node = document.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[last()]/a");
                if (node == null)
                {
                    throw new Exception("未发现最后一页的页数");
                }
                else
                {
                    string href = node.GetAttributeValue("href", "");
                    int num = Convert.ToInt32(Regex.Match(href, @"\d+").Value);
                    return num;
                }
            }
            else
            {
                throw new Exception("未发现最后一页的页数");
            }
        }
    }
}
