using employeeMgmnt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using employeeMgmnt.Dto;
using Microsoft.AspNetCore.Authorization;

namespace employeeMgmnt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private static readonly List<Employee> employees = new List<Employee>()
        {
            new Employee { EmployeeName = "string", Password = "string", Role = "Administrator", ManagerId = 1},
            new Employee { EmployeeName =  "string", Password = "string", Role = "employee", ManagerId = 1 },
        };

        private static readonly List<Leave> LeaveApplications = new List<Leave>();

        [HttpPost("login")]
        public async Task<IActionResult> Login(Employee emp)
        {
            var employee = employees.FirstOrDefault(e => e.EmployeeName == emp.EmployeeName && e.Password == emp.Password);
            if (employee != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, employee.EmployeeName),
                    new Claim("Name", $"{employee.EmployeeName}"),
                    new Claim(ClaimTypes.Role, employee.Role),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return Ok($"Welcome, {employee.EmployeeName}!");
            }
            return Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAllEmployees()
        {
            var allEmp = employees.ToList();
            return Ok(allEmp);
        }

        [HttpGet("{employeeName}")]
        public IActionResult GetEmployeeById(string employeeName)
        {
            var employee = employees.FirstOrDefault(e => e.EmployeeName == employeeName);
            if (employee != null)
            {
                var currEmployee = User.Identity.Name;
                if(currEmployee == employee.EmployeeName || User.IsInRole("Administrator"))
                {
                    return Ok(employee);
                }
            }
            return NotFound();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Administrator")]
        public IActionResult AddEmployee([FromBody] EmployeeDto emp)
        {
            var employee = new Employee
            {
                EmployeeName = emp.employeeName,
                Password = emp.password,
                Role = "employee",
                ManagerId = emp.managerId
            };

            employees.Add(employee);
            return Ok("Employee Added");
        }

        [HttpGet("reportees/{managerId}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetReportees(int managerId)
        {
            var reportees = employees.Where(e => e.ManagerId == managerId && e.Role == "employee").ToList();
            return Ok(reportees);
        }


        [HttpPost("ApplyLeave")]
        [Authorize]
        public IActionResult ApplyLeave([FromBody] LeaveDto leave)
        {
                var leaveApplication = new Leave
                {
                    EmployeeName = leave.employeeName,
                    StartDate = leave.startDate,
                    EndDate = leave.endDate,
                    Status = "Pending"
                };

                LeaveApplications.Add(leaveApplication);
                return Ok("Leave applied Successfully");
        }

        [HttpPut("ApproveLeave/{employeeName}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ApproveLeave(string employeeName)
        {
            var leaveApplication = LeaveApplications.FirstOrDefault(la => la.EmployeeName == employeeName);
            if (leaveApplication != null)
            {
                leaveApplication.Status = "Approved";
                return Ok("Leave Approved!");
            }

            return NotFound("Leave could not be approved");
        }

        //[HttpGet("{employeeName}")]
        //[Authorize]
        //public IActionResult GetLeaveApplication(string employeeName)
        //{
        //    var leaveApplication = LeaveApplications.FirstOrDefault(la => la.employeeName == employeeName);
        //    if (leaveApplication != null)
        //    {
        //        // Only allow managers to see approved leaves or their own pending leaves
        //        if (User.IsInRole("Manager") || (leaveApplication.status == "Pending" && leaveApplication.employeeName == User.Identity.Name))
        //        {
        //            return Ok(leaveApplication);
        //        }
        //    }

        //    return NotFound();
        //}


    }
}
