using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SETWorksProject.Models;
using System.Collections.Generic;
using static SETWorksProject.Models.JsonDeserialized;
using static SETWorksProject.Models.PayloadDeserialization;

namespace SETWorksProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {

        List<Mission> list = new();
        List<Mission> newlist = new();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://api.spacexdata.com/v4/launches");
            //HTTP GET
            var responseTask = client.GetAsync("");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();

                readTask.Wait();


                List<Root> myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(readTask.Result);

                //myDeserializedClass.name;

                


                foreach (Root root in myDeserializedClass)
                {
                    var utcdate = root.date_utc;
                    
                    var rocketName = root.name;
                    var wasSuccess = root.success;

                    Mission mission = new();
                    mission.RocketName = rocketName;
                    mission.WasSuccess = wasSuccess;
                    mission.Date = utcdate.ToShortDateString();
                    mission.UTCTIME = utcdate.ToShortTimeString();
                    mission.Time = TimeZoneInfo.ConvertTimeFromUtc(utcdate, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString();
                    mission.Mass = 0;
                    foreach (var payload in root.payloads)
                    {
                        mission.Mass += GetPayloadMass(payload);
                    }





                    list.Add(mission);
                }





                newlist = list.OrderBy(o => o.Mass).Reverse().ToList();

                foreach (var item in list)
                {
                    int rank = newlist.IndexOf(item) + 1;
                    item.MassRank = rank.ToString();

                    if (item.Mass == 0 || item.Mass == null)
                    {
                        item.MassRank = "N/A";
                    }
                }





            }
            else //web api sent error response 
            {

                return View("Error");
            }
        }
        list.Reverse();

        return View(list);
    }


    public double? GetPayloadMass(string id)
    {
        using (var client = new HttpClient())
        {
            //string urlstring = "https://api.spacexdata.com/v4/payloads/" + id;

            client.BaseAddress = new Uri("https://api.spacexdata.com/v4/payloads/");
            //HTTP GET
            var responseTask = client.GetAsync(id);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();

                readTask.Wait();


                Payload deserializedPayload = JsonSerializer.Deserialize<Payload>(readTask.Result);

                
                

                return deserializedPayload.mass_kg;



            }
            else return 0;
        }
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

