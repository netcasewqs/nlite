using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Test.IoC.Addins.Components
{
    [AddIn(Name = "Dashboard", Author = "Kevin Wang", Url = "http://netcasewqs.cnblogs.com", Version = "1.0")]
    public class DashboardAddIn : AddInBase
    {
        public override void Start()
        {
            Console.WriteLine("Dashboard start...");
        }
    }

    [AddIn(Name = "Editor", Author = "Kevin Wang", Url = "http://netcasewqs.cnblogs.com", Version = "1.0")]
    public class EditorAddIn : AddInBase
    {
        public override void Start()
        {
            Console.WriteLine("Editer start...");
        }
    }

    [AddIn(Name = "PlayList", Author = "Kevin Wang", Url = "http://netcasewqs.cnblogs.com", Version = "1.0")]
    public class PlaylistAddIn : AddInBase
    {
        public override void Start()
        {
            Console.WriteLine("PlayList start...");
        }
    }

       
}
