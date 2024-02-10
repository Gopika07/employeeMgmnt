using employeeMgmnt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using employeeMgmnt.Dto;
using employeeMgmnt.Data;
using Microsoft.AspNetCore.Authorization;

namespace employeeMgmnt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        [HttpPost("employee/login")]
        public async Task<IActionResult> Login(Employee emp)
        {
            var employee = DataManager.employees.FirstOrDefault(e => e.EmployeeName == emp.EmployeeName && e.Password == emp.Password);
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

        [HttpPost("employee/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        [HttpGet("employee/getAll")]
        [Authorize(Roles = "manager")]
        public IActionResult GetAllEmployees()
        {
            var allEmp = DataManager.employees.ToList();
            return Ok(allEmp);
        }

        [HttpGet("{employeeName}")]
        public IActionResult GetEmployeeById(string employeeName)
        {
            var employee = DataManager.employees.FirstOrDefault(e => e.EmployeeName == employeeName);
            if (employee != null)
            {
                var currEmployee = User.Identity.Name;
                if(currEmployee == employee.EmployeeName || User.IsInRole("manager"))
                {
                    return Ok(employee);
                }
            }
            return NotFound();
        }

        [HttpPost("employee/AddEmployee")]
        [Authorize(Roles = "manager")]
        public IActionResult AddEmployee([FromBody] EmployeeDto emp)
        {
            var employee = new Employee
            {
                EmployeeName = emp.employeeName,
                Password = emp.password,
                Role = "employee",
                ManagerId = emp.managerId
            };

            DataManager.employees.Add(employee);
            return Ok("Employee Added");
        }

        [HttpGet("employee/reportees/{managerId}")]
        [Authorize(Roles = "manager")]
        public IActionResult GetReportees(int managerId)
        {
            var reportees = DataManager.employees.Where(e => e.ManagerId == managerId && e.Role == "employee").ToList();
            return Ok(reportees);
        }
    }
}
