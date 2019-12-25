using HtmlAgilityPack;
using SiHan.Libs.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.TempPhone
{
    /// <summary>
    /// materialtools网站的手机号
    /// </summary>
    public class MaterialToolsGetter: ITempPhoneGetter
    {
        private HttpClient HttpClient { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MaterialToolsGetter()
        {
            this.HttpClient = new HttpClient();
            this.HttpClient.IsEnableFrequency = true;
            this.HttpClient.Proxy = ProxyFactory.GetSystemWebProxy();
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
            string url = $"https://www.materialtools.com/?page={num}";
            HttpResponse response = await this.HttpClient.GetAsync(url);
            if (response.IsSuccess())
            {
                string html = response.GetHtml();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var nodes = document.DocumentNode.SelectNodes("//div[@class='phone_number-text']");
                if (nodes != null)
                {
                    foreach (var item in nodes)
                    {
                        string nodeText = item.InnerText.Trim();
                        if (nodeText.StartsWith("+86"))
                        {
                            nodeText = nodeText.TrimStart('+', '8', '6');
                            nodeText = nodeText.Trim();
                            phones.Add(nodeText);
                        }
                    }
                }
            }
            return phones;
        }
        private async Task<int> GetPageNums()
        {
            HttpResponse response = await this.HttpClient.GetAsync("https://www.materialtools.com/");
            if (response.IsSuccess())
            {
                string html = response.GetHtml();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var node = document.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[last()-1]/a");
                if (node == null)
                {
                    throw new Exception("未发现最后一页的页数");
                }
                else
                {
                    int num = Convert.ToInt32(node.InnerHtml);
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
