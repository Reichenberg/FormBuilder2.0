using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team1_DynamicForms.Models
{
    public class FormsAndSubmissionsViewModel
    {
        public SubmissionWhole submittedForms { get; set; }

        public WholeForm unsubmittedForms { get; set; }
    }
}