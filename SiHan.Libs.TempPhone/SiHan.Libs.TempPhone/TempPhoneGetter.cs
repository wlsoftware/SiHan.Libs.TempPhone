using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.TempPhone
{
    /// <summary>
    /// 临时号码获取器
    /// </summary>
    public class TempPhoneGetter : ITempPhoneGetter
    {
        /// <summary>
        /// 获取所有网站的临时手机号，耗时较长
        /// </summary>
        public async Task<List<string>> GetAsync()
        {
            List<string> AllList = new List<string>();
            try
            {
                YinsiduanxinGetter yinsiduanxin = new YinsiduanxinGetter();
                AllList.AddRange(await yinsiduanxin.GetAsync());
            }
            catch
            {
            }
            try
            {
                YunDuanXinGetter yunDuanXin = new YunDuanXinGetter();
                AllList.AddRange(await yunDuanXin.GetAsync());
            }
            catch
            {
            }
            try
            {
                MaterialToolsGetter materialTools = new MaterialToolsGetter();
                AllList.AddRange(await materialTools.GetAsync());
            }
            catch
            {
            }
            return AllList;
        }
    }
}
