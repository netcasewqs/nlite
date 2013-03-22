using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Test.IoC.Addins.Components
{
    //插件管理器接口，用来启动和停止插件的
    [Contract]
    public interface IAddInManager:IStartable
    {
    }
}
