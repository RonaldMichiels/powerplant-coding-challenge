using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PowerplantApp.Entities;
using PowerplantApp.Services;
using System.Text.Json;

namespace PowerplantApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnergyProductionController : ControllerBase
    {
        public EnergyProductionController() { }

        [HttpPost]
        public string GetProductionPlan([FromBody] JsonElement payload)
        {
            string payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
            var result = JsonConvert.DeserializeObject<EnergyPlanPayload>(payloadJson);
            if(result != null)
            {
                var energyPlanResponse = PowerLoadService.CalculatePower(result);
                return JsonConvert.SerializeObject(energyPlanResponse);
            } else
            {
                return "Invalid Input (Payload)";
            }


        }

    }
}
