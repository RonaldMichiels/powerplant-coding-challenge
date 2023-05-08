namespace PowerplantApp.Entities
{
    public class EnergyResult
    {
        public EnergyResult(string name, int p)
        {
            this.name = name;
            this.p = p;
        }

        public string name { get; set; }

        public int p { get; set; }

    }
}
