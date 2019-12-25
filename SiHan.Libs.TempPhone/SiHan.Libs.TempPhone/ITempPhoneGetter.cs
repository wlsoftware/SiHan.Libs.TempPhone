using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.TempPhone
{
    /// <summary>
    /// 临时手机号获取接口
    /// </summary>
    public interface ITempPhoneGetter
    {
        /// <summary>
        /// 获取临时手机号
        /// </summary>
        Task<List<string>> GetAsync();
    }
}
