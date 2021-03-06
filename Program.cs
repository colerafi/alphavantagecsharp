﻿using System;
using System.Collections.Generic;
using System.Linq;
using models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace models
{
    public class StockData
    {
        [JsonProperty("Meta Data")]
        public MetaData MetaData { get; set; }
        [JsonProperty("Time Series (Daily)")]
        public Dictionary<DateTime, DailyData> TimeSeries { get; set; }
    }
    public class MetaData
    {
        [JsonProperty("1. Information")]
        public string Information { get; set; }
        [JsonProperty("2. Symbol")]
        public string Symbol { get; set; }
        [JsonProperty("3. Last Refreshed")]
        public DateTimeOffset LastRefreshed { get; set; }
        [JsonProperty("4. Time Zone")]
        public string TimeZone { get; set; }
    }
    public class DailyData
    {
        [JsonProperty("1. open")]
        public double Open { get; set; }       
        [JsonProperty("2. high")]
        public double High { get; set; }
        [JsonProperty("3. low")]
        public double Low { get; set; }
        [JsonProperty("4. close")]
        public double Close { get; set; }
        [JsonProperty("5. volume")]
        public long Volume { get; set; }

        public double Change() { return Open - Close; }
    }
}
class Program
{
    public static String api_key = "650CS2ENBH9B4PB1";

    //Full = Everything, Compact = last 100 
    public static StockData getStock(String symbol, Boolean full)
    {
        var request = new RestRequest(Method.GET).AddParameter("symbol", symbol)
            .AddParameter("apikey", api_key)
            .AddParameter("function", "TIME_SERIES_DAILY")
            .AddParameter("outputsize", full ? "full" :"compact")
            .AddParameter("datatype", "json");
        var response = new RestClient("https://www.alphavantage.co/query?").Execute(request);
        return JsonConvert.DeserializeObject<StockData>(response.Content);
    }

    public static void QuestionOne()
    {
        
        var msft = getStock("msft", false);
        var startRange = DateTime.Today.AddDays(-7);
        var endRange = DateTime.Today;
        var sum = 0;
        var count = 0;

        foreach (var e in msft.TimeSeries)
        {
            if (e.Key >= startRange && e.Key <= endRange)
            {
                sum += (int) e.Value.Volume;
                count++;

            }
        }
        Console.WriteLine("7 day average volume of " + msft.MetaData.Symbol + " " + sum/count);
    }
    public static void QuestionTwo()
    {
        var aapl = getStock("aapl", true);
        var startRange = DateTime.Today.AddDays(-183);
        var endRange = DateTime.Today;
        double highest = 0;

        foreach (var e in aapl.TimeSeries)
        {
            if (e.Key >= startRange && e.Key <= endRange)
            {
                if(e.Value.Close > highest)
                {
                    highest = e.Value.Close;
                }
            }
        }
        Console.WriteLine("Highest closing price of " + aapl.MetaData.Symbol + " is " + highest);
    }
    public static void QuestionThree()
    {
        var ba = getStock("ba", false);
        var startRange = DateTime.Today.AddDays(-31);
        var endRange = DateTime.Today;
        foreach (var e in ba.TimeSeries)
        {
            if (e.Key >= startRange && e.Key <= endRange)
            {
                Console.WriteLine(e.Key.ToShortDateString() + ": " + ba.MetaData.Symbol + " price changed: " + e.Value.Change());
            }
        }
    }
    public static void QuestionFour(String[] symbols)
    {
        StockData roi = null;
        var highest = 0.0;
        foreach(String s in symbols)
        {
            var stock = getStock(s, false);
            Console.WriteLine(stock.MetaData.Symbol);
            var list = stock.TimeSeries.Values.ToList();
            var ret = list.ElementAt(0).Close - list.ElementAt(31).Close;
            if(ret > highest)
            {
                highest = ret;
                roi = stock;
            }
        }
        Console.WriteLine("Highest ROI was " + roi.MetaData.Symbol + " with a $" + highest + " return");
    }
    static public void Main(String[] args)
    {
        QuestionOne();
        QuestionTwo();
        QuestionThree();
        String[] symbols = {"msft","ba"};
        QuestionFour(symbols);
    }
}
