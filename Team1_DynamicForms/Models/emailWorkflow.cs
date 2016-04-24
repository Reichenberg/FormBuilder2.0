using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team1_DynamicForms.Models
{
    public class EmailWorkflow
    {
        private string from;
        private string messageBody;
        private string[] to;

       
       //shouldn't need this bc of the repository
            public EmailWorkflow(string from, string[] to, string messageBody)
        {
            this.from = from;
            this.to = to;
            this.messageBody = messageBody;
        }

        // should be sent from our server, whatever it is
        public string EmailFrom { get; set; } 

        // should send out emails automatically to those attached to a workflow
        public string[] EmailTo { get; set; }

        // is probably gonna contain a static message, but whatever
        public string Message { get; set; }
    }


    
}
