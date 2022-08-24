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
    public class DelMethod : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            if (context.PrimaryEntityName == "account")
            {
                Entity AccRecod = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("telephone1", "primarycontactid"));
                //string faxno = AccRecod.Contains("fax") ? AccRecod.GetAttributeValue<string>("fax") :"";
                string phno = AccRecod.GetAttributeValue<string>("telephone1");
                tracingService.Trace(phno);
                var primaryContactId = AccRecod.Contains("primarycontactid") ? AccRecod.GetAttributeValue<EntityReference>("primarycontactid") : null;
                tracingService.Trace("Id : "+primaryContactId.Id);
                //Guid u = new Guid();
                //EntityReference pclookup = (EntityReference)AccRecod.Attributes["primarycontactid"];
                //u = pclookup.Id;
                //tracingService.Trace(u.ToString());

                Entity contactUpdate = new Entity("contact");
                contactUpdate.Id = primaryContactId.Id;
                tracingService.Trace("onupdate");

                contactUpdate["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                contactUpdate["telephone1"] = phno;

                tracingService.Trace("ph id before update: " + contactUpdate["telephone1"]);

                service.Update(contactUpdate);
                service.Delete("contact", contactUpdate.Id);
                tracingService.Trace("Phone updated:");
            }
        }
    }
}
