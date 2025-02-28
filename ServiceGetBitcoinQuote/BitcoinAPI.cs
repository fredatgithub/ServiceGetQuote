using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceGetBitcoinQuote
{
  internal class BitcoinAPI
  {
    // get bitcoin euro price from api https://api.bitget.com/api/v2/spot/market/tickers?symbol=BTCEUR
    public async Task<double> GetBitcoinPriceAsync(string currency)
    {
      try
      {
        HttpClient client = new HttpClient();
        Console.WriteLine("Requesting URL: " + "https://api.bitget.com/api/v2/spot/market/tickers?symbol=BTC" + currency);
        string response = await client.GetStringAsync("https://api.bitget.com/api/v2/spot/market/tickers?symbol=BTC" + currency);
        JObject json = JObject.Parse(response);
        double bitcoinPrice = (double)json["data"][0]["lastPr"];
        return bitcoinPrice;
      }
      catch (HttpRequestException)
      {
        // do nothing
      }
      catch (JsonException)
      {
        // do nothing
      }

      return 0;
    }
  }
}
