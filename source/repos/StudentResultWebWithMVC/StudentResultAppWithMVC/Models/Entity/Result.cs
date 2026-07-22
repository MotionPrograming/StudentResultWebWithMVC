namespace StudentResultAppWithMVC.Models.Entity
{
    public class Result
    {
        public int ResultId { get; set; }
        public double PhysicsMark { get; set; }
        public double ChemistryMark { get; set; }
        public double MathMark { get; set; }
        public int StudentId { get; set; }
        public Student student { get; set; }

    }
}
