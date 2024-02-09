namespace employeeMgmnt.Models
{
    public class Employee
    {
        public string EmployeeName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int ManagerId { get; set; }
    }
}
