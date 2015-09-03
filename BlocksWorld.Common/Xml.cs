using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace BlocksWorld
{
	public static class Xml
	{
		static readonly Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();

		private static XmlSerializer GetSerializer(Type t)
		{
			if (serializers.ContainsKey(t) == false)
			{
				Type[] extraTypes = Type.EmptyTypes;
				// TODO: Load extra types here
				serializers.Add(t, new XmlSerializer(t, extraTypes));
			}
			return serializers[t];
		}

		private static T Load<T>(Stream source)
			where T : class, new()
		{
			var ser = GetSerializer(typeof(T));
			return ser.Deserialize(source) as T;
		}

		public static T LoadFromStream<T>(Stream source)
			where T : class, new()
		{
			return Load<T>(source);
		}

		public static T LoadFromFile<T>(string fileName)
			where T : class, new()
		{
			using (var source = File.Open(fileName, FileMode.Open, FileAccess.Read))
			{
				return Load<T>(source);
			}
		}

		/// <summary>
		/// Loads an object from a resource located in the calling assembly.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		public static T LoadFromResource<T>(string resourceName)
			where T : class, new()
		{
			return LoadFromResource<T>(Assembly.GetCallingAssembly(), resourceName);
		}

		/// <summary>
		/// Loads an object from a resource located in the assembly of the refernce type.
		/// </summary>
		/// <typeparam name="Asm">The reference type of which the assembly is used.</typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		public static T LoadFromResource<Asm, T>( string resourceName)
			where T : class, new()
		{
			return LoadFromResource<T>(typeof(Asm).Assembly, resourceName);
		}
		
		public static T LoadFromResource<T>(Assembly asm, string resourceName)
			where T : class, new()
		{
			using (var source = asm.GetManifestResourceStream(resourceName))
			{
				if (source == null)
					throw new KeyNotFoundException("The given resource does not exist.");
				return Load<T>(source);
			}
		}
	}
}