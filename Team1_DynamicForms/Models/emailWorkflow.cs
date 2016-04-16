using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team1_DynamicForms.Models
{
    public class emailWorkflow
    {
        // should be sent from our server, whatever it is
        public string EmailFrom { get; set; } 

        // should send out emails automatically to those attached to a workflow
        public string EmailTo { get; set; }

        // is probably gonna contain a static message, but whatever
        public string Message { get; set; }
    }
}
