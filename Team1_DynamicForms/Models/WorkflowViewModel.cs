using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Team1_DynamicForms.Models
{
    public class WorkFlowOverviewViewModel
    {
        public WorkFlowOverviewViewModel()
        {
            Workflows = new List<WorkFlowGroupViewModel>();
        }
        //Make into three lists, one for in progress, one for viewed, one for completed
        public List<WorkFlowGroupViewModel> Workflows { get; set; }
    }

    public class WorkFlowGroupViewModel
    {
        public WorkFlowGroupViewModel()
        {
            FormNames = new List<string>();
        }

        public string Type { get; set; }
        public List<string> FormNames { get; set; }
    }
     
    public class WorkFlowCreateViewModel
    {
        public WorkFlowCreateViewModel()
        {
            partialViewModel = new WorkFlowCreatePartialViewModel();
        }
        public string FormName { get; set; }
        public WorkFlowCreatePartialViewModel partialViewModel { get; set; }

    }

    public class WorkFlowCreatePartialViewModel
    {
        public int FormId { get; set; }
        [Required]
        public string UserEmails { get; set; }
    }


}