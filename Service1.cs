using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SAPEpsilonSend.Modules;

namespace SAPEpsilonSend
{
    public partial class Service1 : ServiceBase
    {
        private EpsilonSendMethods oTask = new EpsilonSendMethods();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            Settings1 settings = new Settings1();

            if (oTask == null)
                oTask = new EpsilonSendMethods();

            oTask.Start();
        }

        protected override void OnStop()
        {
            if (oTask != null)
                oTask.Stop();
        }
    }
}
