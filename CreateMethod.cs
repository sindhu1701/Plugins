using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace CreateQuery
{
    public class CreateMethod : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            if (context.PrimaryEntityName == "account")
            {
                Entity AccRecod = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("name", "fax"));
                string AccountName = AccRecod.GetAttributeValue<string>("name");
                string faxno = AccRecod.GetAttributeValue<string>("fax");
                Entity ContactRecord = new Entity("contact");

                ContactRecord["firstname"] = AccountName; 
                ContactRecord["lastname"] = "Baker";
                tracingService.Trace("Contact created . firstname and lastname:" + ContactRecord["firstname"] + " "+ ContactRecord["lastname"]);

                ContactRecord["fax"] = faxno;
                ContactRecord["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                ContactRecord["accountrolecode"] = new OptionSetValue(2);
                Guid contactId = service.Create(ContactRecord);
                tracingService.Trace("contact created. id :" + contactId);

            }   
        }   
    }
}
