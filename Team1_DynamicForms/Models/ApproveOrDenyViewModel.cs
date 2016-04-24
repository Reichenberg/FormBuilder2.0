using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team1_DynamicForms.Models
{
    public class ApproveOrDenyViewModel
    {
        public List<string> formData { get; set; }
        public AccountWorkflow accountWf { get; set; }
    }
}
