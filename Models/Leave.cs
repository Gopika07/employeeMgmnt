namespace employeeMgmnt.Models
{
    public class Leave
    {
        public string employeeName { get; set; }
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
        public string status;
    }
}
