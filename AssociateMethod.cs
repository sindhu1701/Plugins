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

    public class AssociateMethod : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.InitiatingUserId);
            try
            {
                EntityReference targetEntity = null;
                string relationshipName = string.Empty;
                Entity studentEntity = null;
                Entity standardEntity = null;
                Entity levelEntity = null;

                EntityReference relatedEntity = null;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    targetEntity = context.InputParameters["Target"] as EntityReference;
                }
                if (context.InputParameters.Contains("Relationship"))
                {
                    relationshipName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                }
                if (relationshipName != "cr659_cr659_standard_cr659_student")
                {
                    return;
                }

                //get related entities
                if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                {
                    EntityReferenceCollection relEntityCol = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                    relatedEntity = relEntityCol[0];
                }

                //retrieve employee entity
                studentEntity = service.Retrieve("cr659_student", targetEntity.Id, new ColumnSet("cr659_level", "cr659_class"));
                //retrieve location entity
                standardEntity = service.Retrieve("cr659_standard", relatedEntity.Id, new ColumnSet("cr659_level", "cr659_name"));
                //retrieve state entity
                levelEntity = service.Retrieve("cr659_level", standardEntity.GetAttributeValue<EntityReference>("cr659_level").Id, new ColumnSet("cr659_name"));
                //step 1
                //Event: pre operation - synchronous - associate
                //primary entity : none, secondary entity : none
                //checking the message name

                if (context.MessageName.ToLower() == "associate")
                {
                    if (studentEntity.GetAttributeValue<EntityReference>("cr659_level").Id != standardEntity.GetAttributeValue<EntityReference>("cr659_level").Id)
                    {
                        throw new Exception("Wrong Location");
                    }

                    //name from location entity
                    var class1 = standardEntity.GetAttributeValue<string>("cr659_name");
                    Entity updateStudent = new Entity("cr659_student");
                    updateStudent.Id = targetEntity.Id;
                    tracingService.Trace("Created: ", updateStudent.Id);

                    //update place field of Account with the place value in location
                    updateStudent["cr659_class"] = class1;
                    tracingService.Trace("Created: ", updateStudent["cr659_class"]);

                    service.Update(updateStudent);
                }


                //step 2
                //Event: pre operation - synchronous - disassociate
                //primary entity : none, secondary entity : none
                else if (context.MessageName.ToLower() == "disassociate")
                {
                    Entity updateStudent = new Entity("cr659_student");
                    updateStudent.Id = targetEntity.Id;
                    //update place field of Account to null
                    updateStudent["cr659_class"] =null;
                    service.Update(updateStudent);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}

