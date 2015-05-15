namespace Models
{
    public class RepsModel
    {
        public RepsModel(string name, int reps, string details)
        {
            this.RepName = name;
            this.Reps = reps;
            this.Details = details;
        }
        public RepsModel(string name, int reps)
        {
            this.RepName = name;
            this.Reps = reps;
            this.Details = "";
        }
        public int Reps { get; set; }

        public string RepName { get; set; }

        public string Details { get; set; }

        public override string ToString()
        {
            return string.Format(RepName +"         Reps = "+ Reps + "  " + Details);
        }
    }
}
