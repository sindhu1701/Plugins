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
    public class UpdateMethod : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            if (context.PrimaryEntityName == "account")
            {
                Entity AccRecod = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("fax","telephone1","primarycontactid"));
                //string faxno = AccRecod.Contains("fax") ? AccRecod.GetAttributeValue<string>("fax") :"";
                string faxno = AccRecod.GetAttributeValue<string>("fax");
                string tele = AccRecod.GetAttributeValue<string>("telephone1");
                tracingService.Trace("Fax value:" + faxno + "," +  "Telephone:" + tele);
              
                Guid u = new Guid();
                EntityReference pclookup = (EntityReference)AccRecod.Attributes["primarycontactid"];
                u = pclookup.Id;
                //tracingService.Trace(u.ToString());

                Entity contactUpdate = new Entity("contact");
                contactUpdate.Id = u;
                tracingService.Trace("onupdate: "+ contactUpdate.Id);

                contactUpdate["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
				contactUpdate["fax"] = faxno;
                contactUpdate["telephone1"] = tele;

                tracingService.Trace("Fax id before update: " + contactUpdate["fax"] + contactUpdate["telephone1"]);

                service.Update(contactUpdate);
                tracingService.Trace("Fax  After updated:" + contactUpdate );

                ////query expression
                //QueryExpression query = new QueryExpression();
                //query.EntityName = "contact";
                //ColumnSet col = new ColumnSet("firstname", "fax", "telephone1", "parentcustomerid");
                //query.ColumnSet = col;

                ////{
                ////    EntityName = "contact",
                ////    ColumnSet = new ColumnSet("firstname", "fax", "parentcustomerid")
                ////};


                //tracingService.Trace("Query Expression :" + query.EntityName);
                //query.Criteria.AddCondition(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, context.PrimaryEntityId));

                //tracingService.Trace("Query  Expression value before executed");
                //EntityCollection collection = service.RetrieveMultiple(query);


                //tracingService.Trace("Query Experssion After executed:" + collection.Entities.Count);

                //string str = string.Empty;

                //foreach (Entity e in collection.Entities)
                //{
                //    str = str + e.Attributes["firstname"].ToString();
                //    str = str + e.Attributes["fax"].ToString();
                //    str = str + e.Attributes["telephone1"].ToString();

                //}


                //throw new InvalidPluginExecutionException(str);
            }
        }
    }
}
                                                                                                                                                                                                                                                                                                                    