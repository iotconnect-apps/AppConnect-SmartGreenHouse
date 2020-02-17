using System.Collections.Generic;

namespace component.sessiontoken
{
    public class SessionTokenInfo
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string EmployeeCode { get; set; }
        public string EmailId { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string LeaderId { get; set; }
        public string ManagerId { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public IEnumerable<string> Departments { get; set; }

        public IEnumerable<string> Claims { get; set; }
        public IEnumerable<string> Policies { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
