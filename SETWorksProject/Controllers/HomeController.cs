using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SETWorksProject.Models;
using System.Collections.Generic;
using static SETWorksProject.Models.RocketJsonDeserialized;
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

                //parse JSON
                List<RocketLaunch> deserializedRockets = JsonSerializer.Deserialize<List<RocketLaunch>>(readTask.Result);

                

                

                //build new mission object for each returned launch
                foreach (RocketLaunch rocketLaunch in deserializedRockets)
                {
                    var utcdate = rocketLaunch.date_utc;
                    
                    var rocketName = rocketLaunch.name;
                    var wasSuccess = rocketLaunch.success;

                    Mission mission = new();
                    mission.Pic = rocketLaunch.links.patch.small;
                    mission.RocketName = rocketName;
                    mission.WasSuccess = wasSuccess;


                    mission.Date = utcdate.ToShortDateString();
                    mission.UTCTIME = utcdate.ToShortTimeString();

                    //converst utc to cst
                    mission.Time = TimeZoneInfo.ConvertTimeFromUtc(utcdate, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortTimeString();

                    //each payload is a list of payload Ids. Im not sure if there are multiple associated with one of these launches but just in case we'll call our GetPayLoad mass function to grab those from the api and sum them together
                    mission.Mass = 0;
                    foreach (var payload in rocketLaunch.payloads)
                    {
                        mission.Mass += GetPayloadMass(payload);
                    }





                    list.Add(mission);
                }




                //this code ranks the mass with highest mass as rank one 
                newlist = list.OrderBy(o => o.Mass).Reverse().ToList();

                foreach (var item in list)
                {
                    int rank = newlist.IndexOf(item) + 1;
                    item.MassRank = rank.ToString();
                    //some payloads have mass 0 or null we'll just call these as rank N/A
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
            //the api url string is like this "https://api.spacexdata.com/v4/payloads/" + id;

            client.BaseAddress = new Uri("https://api.spacexdata.com/v4/payloads/");
            
            //get the payload with the specified id
            var responseTask = client.GetAsync(id);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();

                readTask.Wait();


                Payload deserializedPayload = JsonSerializer.Deserialize<Payload>(readTask.Result);

                
                //some of these payloads have null masses or 0 mass

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

