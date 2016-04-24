using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team1_DynamicForms.Models
{
    public class WorkflowsToApproveViewModel
    {
        public List<AccountWorkflow> formsToApprove { get; set; }
        public List<string> usersWhoFilledForm { get; set; }
        public List<string> nameOfFilledForm { get; set; }
    }
}
