namespace PowerplantApp.Entities
{
    public class EnergyPlanResponse
    {
        public EnergyPlanResponse() 
        {
            this.EnergyResults = new List<EnergyResult>();
        }

        public List<EnergyResult> EnergyResults { get; set; }
    }
}
