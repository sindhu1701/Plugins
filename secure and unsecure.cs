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
    using DataContract;
    using Helper;

    public class secureUnsecure : IPlugin
    {
        public readonly string unsecureValue;
        public readonly string secureValue;
        public secureUnsecure(string unsecureConfig, string secureConfig) //cr659_name : XYZ
        {
            if(string.IsNullOrWhiteSpace (unsecureConfig))
			{
                unsecureValue = string.Empty;
            }

			else
			{
               Configuration data = JSONhelper.Deserialize<Configuration>(unsecureConfig);
               unsecureValue = data.Name;
			}

            if(string.IsNullOrWhiteSpace (secureConfig))
			{
                secureValue = string.Empty;
			}

			else
			{
                Configuration data = JSONhelper.Deserialize<Configuration>(secureConfig);
                secureValue = data.Name;
			}
		}

        public void Execute(IServiceProvider serviceProvider)
        {


			try
			{
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                if (context.Depth > 1)
                {
                    return;
				}
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = factory.CreateOrganizationService(context.UserId);

                    EntityReference standardEntityRef = context.InputParameters["Target"] as EntityReference;


                    Entity updateStandarad = new Entity(standardEntityRef.LogicalName);
                    updateStandarad.Id = standardEntityRef.Id;
                    updateStandarad["cr659_name"] = secureValue + " " + unsecureValue;

                    tracingService.Trace("Created: ", updateStandarad["cr659_name"]);
                    service.Update(updateStandarad);



                    //shared Variables
                    Guid Contactid = service.Create(new Entity("contact"));
                }

            }

			catch(Exception Ex)
			{
                throw new InvalidPluginExecutionException(Ex.Message);
			}

           
        }
    }
}

