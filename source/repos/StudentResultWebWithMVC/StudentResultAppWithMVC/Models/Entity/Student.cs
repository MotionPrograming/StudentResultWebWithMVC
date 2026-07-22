namespace StudentResultAppWithMVC.Models.Entity
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int StudentAge { get; set; }
        public double TotalMark { get; set; }
        public double AverageMark { get; set; }
        public double CGPA { get; set; }
        public string? LetterGrade { get; set; }
        public int DepartmentId { get; set; }
        public Department? department { get; set; }
        public Result? result { get; set; }
    }
}
