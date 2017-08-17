using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business
{
    /// <summary>
    /// 实操文档管理
    /// </summary>
    internal interface IPracticalContentManager
    {
        /// <summary>
        /// 根据实操科目ID获取实操文档
        /// </summary>
        /// <param name="id">实操ID</param>
        /// <returns>实操文档路径</returns>
        string LoadContentByPractialID(string id);
        /// <summary>
        /// 保存实操文档内容
        /// </summary>
        /// <param name="filePath">实操文档路径</param>
        void SaveContent(string filePath);
    }
}
