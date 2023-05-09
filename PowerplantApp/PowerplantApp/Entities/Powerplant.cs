namespace PowerplantApp.Entities
{
    public class Powerplant
    {

        public Powerplant(string name, string type, float efficiency, int pmin, int pmax)
        {
            this.name = name;
            this.type = type;
            this.efficiency = efficiency;
            this.pmin = pmin;
            this.pmax = pmax;
            this.pricePerUnitOfElectricity = 0;
            this.pcurrent = 0;
            this.minCost = 0;
            this.maxCost = 0;
            this.currentCost = 0;
        }

        public string name { get; set; }

        public string type { get; set; }

        public int typeCategory { get; set; }

        public float efficiency { get; set; }

        public int pmin { get; set; }

        public int pmax { get; set; }

        public float effectifePmax { get; set; }

        public float pricePerUnitOfElectricity { get; set; }

        public int pcurrent { get; set; }

        public float minCost { get; set; }

        public float maxCost { get; set; }

        public float currentCost { get; set; }
    }
}
