using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace CreateQuery.Helper
{
	public static class JSONhelper
	{
		public static T Deserialize<T> (string json)
		{
			using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				var Serializer = new DataContractJsonSerializer(typeof(T));
				return (T)Serializer.ReadObject(memoryStream);
			}
		}
	}
}
