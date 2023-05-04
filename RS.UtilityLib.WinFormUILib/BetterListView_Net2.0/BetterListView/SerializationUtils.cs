using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Common serialization utilities.
	/// </summary>
	internal static class SerializationUtils
	{
		/// <summary>
		///   Convert Image instance to its corresponding Base64 string representation.
		/// </summary>
		/// <param name="image">image to convert</param>
		/// <param name="format">image representation format</param>
		/// <returns>Base64 string representation of image</returns>
		public static string ImageToBase64String(Image image, ImageFormat format) {
			Checks.CheckNotNull(format, "format");
			if (image == null) {
				return string.Empty;
			}
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, format);
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Close();
			memoryStream.Dispose();
			return Convert.ToBase64String(inArray);
		}

		/// <summary>
		///   Convert Base64 string reprentation of image to a new Image instance.
		/// </summary>
		/// <param name="strImage">Base64 string reprentation of image</param>
		/// <returns>Image instance</returns>
		public static Image ImageFromBase64String(string strImage) {
			Checks.CheckNotNull(strImage, "strImage");
			if (strImage.Length != 0) {
				Checks.CheckBase64String(strImage, "strImage");
			}
			if (strImage.Length == 0) {
				return null;
			}
			byte[] array = Convert.FromBase64String(strImage);
			MemoryStream memoryStream = new MemoryStream(array, 0, array.Length);
			memoryStream.Write(array, 0, array.Length);
			Image result = Image.FromStream(memoryStream);
			memoryStream.Close();
			memoryStream.Dispose();
			return result;
		}

		/// <summary>
		///   Try to deserialize the specified byte array to object.
		/// </summary>
		/// <param name="data">data to deserialize</param>
		/// <returns>deserialized object on success; null on fail</returns>
		public static object DeserializeBinary(byte[] data) {
			object result;
			if (data != null && data.Length != 0) {
				MemoryStream memoryStream = new MemoryStream(data);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				result = binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
				memoryStream.Dispose();
			}
			else {
				result = null;
			}
			return result;
		}

		/// <summary>
		///   Try to deserialize the specified object from XML.
		///   If object type does not support XML deserialization, object is deserialized from Base64 string.
		/// </summary>
		/// <typeparam name="T">type of the serialized object</typeparam>
		/// <param name="reader">XML reader of serialized input</param>
		/// <param name="fieldName">name of the serialized field</param>
		/// <returns>deserialized object, or null</returns>
		public static T DeserializeXml<T>(XmlReader reader, string fieldName) {
			return SerializationUtils.DeserializeXml(reader, fieldName, default(T));
		}

		/// <summary>
		///   Try to deserialize the specified object from XML.
		///   If object type does not support XML deserialization, object is deserialized from Base64 string.
		/// </summary>
		/// <typeparam name="T">type of the serialized object</typeparam>
		/// <param name="reader">XML reader of serialized input</param>
		/// <param name="fieldName">name of the serialized field</param>
		/// <param name="defaultValue">value to return when the object is not available</param>
		/// <returns>deserialized object, or null</returns>
		public static T DeserializeXml<T>(XmlReader reader, string fieldName, T defaultValue) {
			Checks.CheckNotNull(reader, "reader");
			Checks.CheckString(fieldName, "fieldName");
			if (!reader.Name.Equals(fieldName, StringComparison.Ordinal)) {
				throw new SerializationException("Unexpected node name: '" + reader.Name + "' (expected '" + fieldName + "').");
			}
			object obj;
			if (reader.IsEmptyElement) {
				obj = null;
				reader.Read();
			}
			else {
				reader.ReadStartElement();
				if (reader.NodeType == XmlNodeType.Element) {
					bool isEmptyElement = reader.IsEmptyElement;
					obj = new XmlSerializer(typeof(T)).Deserialize(reader);
					if (isEmptyElement) {
						reader.Read();
					}
				}
				else if (reader.NodeType == XmlNodeType.Text) {
					byte[] buffer = Convert.FromBase64String(reader.ReadContentAsString());
					MemoryStream memoryStream = new MemoryStream(buffer);
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					obj = binaryFormatter.Deserialize(memoryStream);
					memoryStream.Close();
					memoryStream.Dispose();
				}
				else {
					obj = null;
				}
				reader.ReadEndElement();
			}
			if (obj == null) {
				return defaultValue;
			}
			return (T)obj;
		}

		/// <summary>
		///   Try to serialize the specified object to byte array.
		/// </summary>
		/// <param name="obj">object to serialize</param>
		/// <returns>array with serialized object on success; zero-length array on fail</returns>
		public static byte[] SerializeBinary(object obj) {
			byte[] result;
			if (obj != null && obj.GetType().IsSerializable) {
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, obj);
				result = memoryStream.ToArray();
				memoryStream.Close();
				memoryStream.Dispose();
			}
			else {
				result = new byte[0];
			}
			return result;
		}

		/// <summary>
		///   Try to serialize the specified object to XML.
		///   If object type does not support XML serialization, object is binary serialized as Base64 string.
		/// </summary>
		/// <param name="writer">XML writer for serialized output</param>
		/// <param name="fieldName">name of the serialized field</param>
		/// <param name="obj">object to serialize</param>
		public static void SerializeXml(XmlWriter writer, string fieldName, object obj) {
			Checks.CheckNotNull(writer, "writer");
			Checks.CheckString(fieldName, "fieldName");
			writer.WriteStartElement(fieldName);
			if (obj != null) {
				Type type = obj.GetType();
				if (obj is IXmlSerializable) {
					new XmlSerializer(type).Serialize(writer, obj);
				}
				else if (type.IsSerializable) {
					MemoryStream memoryStream = new MemoryStream();
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memoryStream, obj);
					byte[] inArray = memoryStream.ToArray();
					memoryStream.Close();
					memoryStream.Dispose();
					writer.WriteString(Convert.ToBase64String(inArray));
				}
			}
			writer.WriteEndElement();
		}
	}
}