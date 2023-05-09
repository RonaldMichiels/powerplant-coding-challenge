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
        }

        public string name { get; set; }

        public string type { get; set; }

        public float efficiency { get; set; }

        public int pmin { get; set; }

        public int pmax { get; set; }

        public float pricePerUnitOfElectricity { get; set; }
    }
}
