using Newtonsoft.Json;

namespace PowerplantApp.Entities
{
    public class Fuel
    {

        public Fuel(float gas, float kerosine, int co2, int wind)
        {
            this.gas = gas;
            this.kerosine = kerosine;
            this.co2 = co2;
            this.wind = wind;
        }

        [JsonProperty("gas(euro/MWh)")]
        public float gas{ get; set; }
        [JsonProperty("kerosine(euro/MWh)")]
        public float kerosine { get; set; }
        [JsonProperty("co2(euro/ton)")]
        public int co2 { get; set; }
        [JsonProperty("wind(%)")]
        public int wind { get; set; }




    }
}
