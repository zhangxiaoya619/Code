using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    /// <summary>
    /// 购买权限接口
    /// </summary>
    public interface IPower
    {
        /// <summary>
        /// 获取用户是否已经购买激活
        /// </summary>
        /// <param name="practicalItemID">科目ID</param>
        /// <returns>是否购买激活</returns>
        bool HasPower(string practicalItemID);
    }
}
