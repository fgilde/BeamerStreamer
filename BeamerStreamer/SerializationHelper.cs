//============================================================================================
// SerializationHelper
//--------------------------------------------------------------------------------------------
// Solution			CP.Suite
// Projekt:			CP.Suite.Common
// Namespace:		CP.Suite.Common
// File				SerializationHelper.cs
//
// (C) Copyright 2004 CP CORPORATE PLANNING AG
// http://www.corporate-planning.com
//
// Alle Rechte vorbehalten. All rights reserved.
//============================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace BeamerStreamer
{
	/// <summary>
	/// Hilf beim Serialisieren und deserialisieren von Klassen
	/// </summary>
	public static class SerializationHelper
	{
		/// <summary>
		/// Deseralisiert eine XML Datei
		/// </summary>
		public static T XmlDeserialize<T>(string filename)
		{
			T result;
			if (TryXmlDeserialize(filename, out result))
				return result;
			return default(T);
		}

		/// <summary>
		/// Deseralisiert eine XML Datei
		/// </summary>
		public static T XmlDeserialize<T>(Stream stream)
		{
			T result;
			if (TryXmlDeserialize(stream, out result))
				return result;
			return default(T);
		}


		/// <summary>
		/// Deseralisiert eine XML Datei und gibt bei erfolg true zurük
		/// </summary>
		public static bool TryXmlDeserialize<T>(string filename, out T result)
		{
			result = default(T);
			if (File.Exists(filename))
			{
				var fileStream = new FileStream(filename, FileMode.Open);
				try
				{
					return TryXmlDeserialize(fileStream, out result);
				}
				finally
				{
					fileStream.Close();
				}
			}
			return false;
		}

		/// <summary>
		/// Deseralisiert eine XML Datei und gibt bei erfolg true zurük
		/// </summary>
		public static bool TryXmlDeserialize<T>(Stream stream, out T result)
		{
			stream.Seek(0, SeekOrigin.Begin);
			var serializer = new XmlSerializer(typeof(T));
			result = (T)serializer.Deserialize(stream);
			return true;
		}



		/// <summary>
		/// Seralisiert eine  Klasse in eine XML Datei
		/// </summary>
		public static Stream XmlSerialize<T>(this T content, Stream stream = null)
		{
			if (stream == null)
				stream = new MemoryStream();
			TryXmlSerialize(content, stream);
			return stream;
		}

		/// <summary>
		/// Seralisiert eine Klasse in eine XML Datei und gibt bei erfolg true zurück
		/// </summary>
		public static bool TryXmlSerialize<T>(this T content, Stream stream)
		{

			stream.Seek(0, SeekOrigin.Begin);
			var serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(stream, content);
			stream.Position = 0;
			return true;
		}

		/// <summary>
		/// Seralisiert eine  Klasse in eine XML Datei
		/// </summary>
		public static bool XmlSerialize<T>(this T content, string filename)
		{
			return TryXmlSerialize(content, filename);
		}

		/// <summary>
		/// Seralisiert eine Klasse in eine XML Datei und gibt bei erfolg true zurück
		/// </summary>
		public static bool TryXmlSerialize<T>(this T content, string filename)
		{
			string dir = Path.GetDirectoryName(filename);
			if (!string.IsNullOrEmpty(dir))
			{
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				var fileStream = new FileStream(filename, FileMode.Create);
				try
				{
					if (TryXmlSerialize(content, fileStream))
						return File.Exists(filename);
				}
				finally
				{
					fileStream.Close();
				}
			}
			return false;
		}



		/// <summary>
		/// Deseralisiert eine Binärformatierte Datei
		/// </summary>
		public static T BinaryDeserialize<T>(string filename)
		{
			T result;
			if (TryBinaryDeserialize(filename, out result))
				return result;
			return default(T);
		}

		/// <summary>
		/// Deseralisiert eine Binärformatierte Datei und gibt bei erfolg true zurück
		/// </summary>
		public static bool TryBinaryDeserialize<T>(string filename, out T result)
		{
			result = default(T);
			if (File.Exists(filename))
			{
				var fileStream = new FileStream(filename, FileMode.Open);
				try
				{

					try
					{
						var serializer = new BinaryFormatter();
						result = (T)serializer.Deserialize(fileStream);
					}
					catch (Exception)
					{
						return false;
					}
					return true;
				}
				finally
				{
					fileStream.Close();
				}
			}
			return false;
		}

		/// <summary>
		/// Deseralisiert ein Bytearray
		/// </summary>
		public static T BinaryDeserialize<T>(byte[] content)
		{
			T result;
			if (TryBinaryDeserialize(content, out result))
				return result;
			return default(T);
		}

		/// <summary>
		/// Deseralisiert ein Bytearray und gibt bei erfolg true zurück
		/// </summary>
		public static bool TryBinaryDeserialize<T>(byte[] content, out T result)
		{
			result = default(T);

			var fileStream = new MemoryStream(content);
			try
			{
				var serializer = new BinaryFormatter();
				result = (T)serializer.Deserialize(fileStream);
				return true;
			}
			finally
			{
				fileStream.Close();
			}
		}



		/// <summary>
		/// Seralisiert eine Datei im Binärformat
		/// </summary>
		public static Stream BinarySerialize<T>(this T content, Stream stream = null)
		{
			if (stream == null)
				stream = new MemoryStream();
			TryBinarySerialize(content, stream);
			return stream;
		}

		/// <summary>
		/// Seralisiert eine Datei im Binärformat
		/// </summary>
		public static bool BinarySerialize<T>(this T content, string filename)
		{
			return TryBinarySerialize(content, filename);
		}

		/// <summary>
		/// Seralisiert eine Datei im Binärformat
		/// </summary>
		public static bool TryBinarySerialize<T>(this T content, Stream stream)
		{
			try
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, content);
				stream.Position = 0;
				return true;
			}
			catch (Exception e)
			{
				Trace.TraceError(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Seralisiert eine Klasse in eine XML Datei und gibt bei erfolg true zurück
		/// </summary>
		public static bool TryBinarySerialize<T>(this T content, string filename)
		{
			string dir = Path.GetDirectoryName(filename);
			if (!string.IsNullOrEmpty(dir))
			{
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				var fileStream = new FileStream(filename, FileMode.Create);
				try
				{
					if (TryBinarySerialize(content, fileStream))
						return File.Exists(filename);
				}
				finally
				{
					fileStream.Close();
				}
			}
			return false;
		}

	}
}