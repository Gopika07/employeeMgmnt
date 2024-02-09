namespace employeeMgmnt.Dto
{
    public class LeaveDto
    {
        public string employeeName { get; set; }
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
        public string status { get; set; }
    }
}
