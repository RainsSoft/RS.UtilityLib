using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HML
{
    /// <summary>
    /// 缩放控件接口
    /// </summary>
    public interface IDpiControl
    {
        /// <summary>
        /// Dpi
        /// </summary>
        float ScaleDpi { get; set; }
    }

}
