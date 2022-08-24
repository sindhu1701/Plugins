using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CreateQuery.DataContract
{
	[DataContract]
	public class Configuration
	{
		[DataMember]
		public string Name;
	}
}
