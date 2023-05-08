namespace PowerplantApp.Entities
{
    public class EnergyPlanPayload
    {
        public EnergyPlanPayload() { }

        public int load { get; set; }

        public Fuel fuels { get; set; }

        public List<Powerplant> powerplants { get; set; }

    }
}
