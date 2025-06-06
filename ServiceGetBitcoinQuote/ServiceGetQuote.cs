﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace ServiceGetBitcoinQuote
{
  public partial class ServiceGetQuote : ServiceBase
  {
    private int eventId = 1;

    public ServiceGetQuote()
    {
      InitializeComponent();
      eventLog1 = new EventLog();
      if (!EventLog.SourceExists("ServiceGetQuote"))
      {
        EventLog.CreateEventSource("ServiceGetQuote", "MyNewLog");
      }

      eventLog1.Source = "MySource";
      eventLog1.Log = "MyNewLog";
    }

    protected override void OnStart(string[] args)
    {
      eventLog1.WriteEntry("Démarrage du service GetQuote.");
      // Set up a timer that triggers every minute.
      Timer timer = new Timer
      {
        Interval = 60000 // 60 seconds
      };

      timer.Elapsed += new ElapsedEventHandler(OnTimer);
      timer.Start(); // every minute do stuff
    }

    private void OnTimer(object sender, ElapsedEventArgs e)
    {
      try
      {
        // Fire and forget the async task
        _ = DoWorkAsync();
      }
      catch (Exception exception)
      {
        eventLog1.WriteEntry($"Error in OnTimer: {exception.Message}", EventLogEntryType.Error);
      }
    }

    private async Task DoWorkAsync()
    {
      // old url
      // https://api.bitget.com/api/v2/spot/market/tickers?symbol=BTCEUR
      // 27/02/2025 new API
      // https://api.bitget.com/api/v2/spot/market/tickers?symbol=BTCEUR
      // https://api.bitget.com/api/v2/spot/market/tickers?symbol=BTCUSD
      eventLog1.WriteEntry("Getting a quote", EventLogEntryType.Information, eventId++);
      BitcoinAPI bitcoinAPI = new BitcoinAPI();
      double bitcoinPriceUSDT = await bitcoinAPI.GetBitcoinPriceAsync("USDT");
      eventLog1.WriteEntry($"Quote USDT: {bitcoinPriceUSDT}", EventLogEntryType.Information, eventId++);
      double bitcoinPriceEUR = await bitcoinAPI.GetBitcoinPriceAsync("EUR");
      eventLog1.WriteEntry($"Quote EURO: {bitcoinPriceEUR}", EventLogEntryType.Information, eventId++);
      DateTime today = DateTime.Now;
      if (bitcoinPriceEUR == 0 || bitcoinPriceUSDT == 0)
      {
        return;
      }

      bool insertResult = DALHelper.WriteToDatabase(today, bitcoinPriceEUR, bitcoinPriceUSDT);
      if (insertResult)
      {
        eventLog1.WriteEntry("Quote written to db successfully", EventLogEntryType.Information, eventId++);
      }
      else
      {
        eventLog1.WriteEntry("Quote not written to db successfully", EventLogEntryType.Information, eventId++);
      }

      bitcoinAPI = null;
    }

    private void OnTimerOld(object sender, ElapsedEventArgs e)
    {
      //eventLog1.WriteEntry("Getting a quote", EventLogEntryType.Information, eventId++);
      //const string apiUrl = "https://api.coindesk.com/v1/bpi/currentprice.json";
      //var myJsonResponse = InternetHelper.GetAPIFromUrl(apiUrl);
      //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
      //DateTime theDate = myDeserializedClass.Time.UpdatedISO;
      //double rateEuros = myDeserializedClass.Bpi.EUR.Rate_float;
      //double rateDollar = myDeserializedClass.Bpi.USD.Rate_float;
      //var latestDate = DALHelper.GetLatestDate();
      //DateTime latestDateFromDB = DateTime.Parse(latestDate);
      //bool insertResult = false;
      //myJsonResponse = InternetHelper.GetAPIFromUrl(apiUrl);
      //myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
      //theDate = myDeserializedClass.Time.UpdatedISO;
      //rateEuros = myDeserializedClass.Bpi.EUR.Rate_float;
      //rateDollar = myDeserializedClass.Bpi.USD.Rate_float;
      //latestDate = DALHelper.GetLatestDate();
      //latestDateFromDB = DateTime.Parse(latestDate);
      //insertResult = DALHelper.WriteToDatabase(theDate, rateEuros, rateDollar);
    }

    protected override void OnStop()
    {
      eventLog1.WriteEntry("Arrêt du service GetQuote.");
    }

    protected override void OnContinue()
    {
      eventLog1.WriteEntry("On-Continue du service GetQuote.");
    }

    protected override void OnPause()
    {
      eventLog1.WriteEntry("On-Pause du service GetQuote.");
    }
    protected override void OnShutdown()
    {
      eventLog1.WriteEntry("On shutdown du service GetQuote.");
    }

    public enum ServiceState
    {
      SERVICE_STOPPED = 0x00000001,
      SERVICE_START_PENDING = 0x00000002,
      SERVICE_STOP_PENDING = 0x00000003,
      SERVICE_RUNNING = 0x00000004,
      SERVICE_CONTINUE_PENDING = 0x00000005,
      SERVICE_PAUSE_PENDING = 0x00000006,
      SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
      public int dwServiceType;
      public ServiceState dwCurrentState;
      public int dwControlsAccepted;
      public int dwWin32ExitCode;
      public int dwServiceSpecificExitCode;
      public int dwCheckPoint;
      public int dwWaitHint;
    };
  }
}
