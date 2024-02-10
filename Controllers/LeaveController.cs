using employeeMgmnt.Dto;
using employeeMgmnt.Models;
using employeeMgmnt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace employeeMgmnt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        [HttpPost("employee/ApplyLeave")]
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

            DataManager.LeaveApplications.Add(leaveApplication);
            return Ok("Leave applied Successfully");
        }

        [HttpPut("employee/ApproveLeave/{employeeName}")]
        [Authorize(Roles = "manager")]
        public IActionResult ApproveLeave(string employeeName)
        {
            var leaveApplication = DataManager.LeaveApplications.FirstOrDefault(la => la.EmployeeName == employeeName);
            if (leaveApplication != null)
            {
                leaveApplication.Status = "Approved";
                return Ok("Leave Approved!");
            }

            return NotFound("Leave could not be approved");
        }
    }
}
