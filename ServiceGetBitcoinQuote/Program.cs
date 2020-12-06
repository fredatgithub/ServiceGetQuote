using System.ServiceProcess;

namespace ServiceGetBitcoinQuote
{
  static class Program
  {
    /// <summary>
    /// Point d'entrée principal de l'application.
    /// </summary>
    static void Main()
    {
      ServiceBase[] ServicesToRun;
      ServicesToRun = new ServiceBase[]
      {
                new ServiceGetQuote()
      };

      ServiceBase.Run(ServicesToRun);
    }
  }
}
