using employeeMgmnt.Models;

namespace employeeMgmnt.Data
{
    public class DataManager
    {
        public static readonly List<Employee> employees = new List<Employee>()
        {
            new Employee { EmployeeName = "string", Password = "string", Role = "manager", ManagerId = 1},
            new Employee { EmployeeName =  "string", Password = "string", Role = "employee", ManagerId = 1 },
        };

        public static readonly List<Leave> LeaveApplications = new List<Leave>();

    }
}
