using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPlc2
{

    internal class MyLog
    {
        private Serilog.Core.Logger logger;
        private DataGridView? view;

        public MyLog(DataGridView? view)
        {
            //string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            string path = "log.txt";

            logger ??= new LoggerConfiguration()
                    .WriteTo.File(path, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            Log.Logger = logger;

            this.view = view;
        }

        public void Error(string msg)
        {
            Log.Error(msg);

            if (view != null)
            {
                var newData = new { c1 = msg, c2 = "错误", c3 = "" };
                view.Rows.Add(newData.c1, newData.c2, newData.c3);
            }
        }
    }
}
