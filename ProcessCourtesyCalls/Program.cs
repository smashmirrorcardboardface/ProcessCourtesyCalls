using System;
using System.Timers;
using System.Web.Services.Protocols;
using Topshelf;
using System.Configuration;

namespace ProcessCourtesyCalls
{
    public class TownCrier
    {
        readonly Timer _timer;

        public TownCrier()
        {
            try
            {
                _timer = new Timer(Convert.ToDouble(ConfigurationManager.AppSettings["TimerPeriod"])) {AutoReset = true};
            }
            catch (OverflowException)
            {
            }
            _timer.Elapsed += OnTimerOnElapsed;
        }

        private void OnTimerOnElapsed(object sender, ElapsedEventArgs eventArgs)
        {
            var obj = new local.regus.lombardi.CSTS_ExternalIntegrations();
            Console.WriteLine(" Calling Web Methods");
            Console.WriteLine("---------------------");
            Console.WriteLine("\n Calling processCourtesyCalls Method");
            try
            {
                obj.processCourtesyCalls();
            }
            catch (SoapException error)
            {
                Console.WriteLine(error.Detail.ToString());
            }
        }

        public void Start()
        {
            _timer.Start();

        }
        public void Stop() { _timer.Stop(); }
    }

    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>                                 //1
            {
                x.Service<TownCrier>(s =>                        //2
                {
                    s.ConstructUsing(name => new TownCrier());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription("Courtesy Call Poller Host");        //7
                x.SetDisplayName("CSTS CC Host");                       //8
                x.SetServiceName("CCPollerHost");                       //9
            });                                                  //10
        }
    }
}
