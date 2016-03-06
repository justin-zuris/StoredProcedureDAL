using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Zuris
{
    public static class Extensions
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }

        public static List<T> BindToList<T>(this DataTable dataTable) where T : new()
        {
            var list = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(row.BindToObject<T>());
            }

            return list;
        }

        public static T BindToObject<T>(this DataRow row) where T : new()
        {
            var record = new T();
            foreach (DataColumn c in row.Table.Columns)
            {
                if (!record.TrySetPropertyValue(c.ColumnName, row[c]))
                {
                    // log the error? throw exception?
                }
            }

            return record;
        }

        public static T DeserializeXmlInto<T>(this string s) where T : class
        {
            T o = default(T);
            using (var reader = new StringReader(s))
            {
                var xmlSrz = new XmlSerializer(typeof(T));
                o = xmlSrz.Deserialize(reader) as T;
            }
            return o;
        }

        public static TE ToEnum<TE>(this string name, bool ignoreCase = false) where TE : struct
        {
            return (TE)Enum.Parse(typeof(TE), name, ignoreCase);
        }

        public static TE? ToNullableEnum<TE>(this string name, bool ignoreCase = false) where TE : struct
        {
            TE t;
            if (Enum.TryParse<TE>(name, ignoreCase, out t))
                return t;
            else 
                return null;
        }

        public static string SerializeObjectToXml(this object o, bool includeNamespaceAttribute = true, bool includeXmlDeclaration = true)
        {
            var xmlSrz = new XmlSerializer(o.GetType());
            var settings = new XmlWriterSettings { OmitXmlDeclaration = !includeXmlDeclaration, Indent = true };

            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlTextWriter.Create(stringWriter, settings))
            {
                if (!includeNamespaceAttribute)
                {
                    //Add an empty namespace and empty value
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");
                    xmlSrz.Serialize(xmlWriter, o, namespaces);
                }
                else
                {
                    xmlSrz.Serialize(xmlWriter, o);
                }

                xmlWriter.Flush();
                return stringWriter.ToString();
            }
        }

        public static string SerializeToXml(this object obj)
        {
            StringBuilder output = new StringBuilder();
            if (obj != null)
            {
                IEnumerator list = null;
                if (obj.GetType() is IEnumerable)
                {
                    list = (obj as IEnumerable).GetEnumerator();
                    if (!list.MoveNext()) list = null;
                }

                do
                {
                    if (list != null) obj = list.Current;

                    if (obj != null)
                    {
                        try
                        {
                            if (Convert.GetTypeCode(obj) != TypeCode.Object)
                            {
                                output.Append("<xml><value>").Append(Convert.ToString(obj)).Append("</value></xml>").AppendLine();
                            }

                            output.AppendLine(obj.SerializeObjectToXml());
                        }
                        catch
                        {
                            // manual xml serialization?
                        }
                    }
                } while (list != null && list.MoveNext());
            }
            return output.ToString();
        }

        public static bool TrySetPropertyValue(this object obj, string fieldName, object value)
        {
            bool wasBound = false;
            var property = obj.GetType().GetProperty(fieldName,
                BindingFlags.Public |
                BindingFlags.SetProperty |
                BindingFlags.Instance);

            if (property == null)
            {
                property = obj.GetType().GetProperty(fieldName,
                    BindingFlags.Public |
                    BindingFlags.SetProperty |
                    BindingFlags.IgnoreCase |
                    BindingFlags.Instance);
            }

            if (property != null)
            {
                var ptype = property.PropertyType;
                if (value == null || Convert.IsDBNull(value))
                {
                    // set to default?
                }
                else
                {
                    if (ptype != value.GetType())
                    {
                        var ctype = ptype.AutoResolveNullable();
                        try { value = Convert.ChangeType(value, ctype); }
                        catch { }
                    }
                    try { property.SetValue(obj, value, null); }
                    catch { }
                    wasBound = true;
                }
            }
            return wasBound;
        }

        public static bool IsNumber(this object obj)
        {
            if (Equals(obj, null))
            {
                return false;
            }

            Type objType = obj.GetType();
            objType = Nullable.GetUnderlyingType(objType) ?? objType;

            if (objType.IsPrimitive)
            {
                return objType != typeof(bool) &&
                    objType != typeof(char) &&
                    objType != typeof(IntPtr) &&
                    objType != typeof(UIntPtr);
            }

            return objType == typeof(decimal);
        }

        public static bool IsNullableType(this object o)
        {
            Type t = (o is Type) ? t = o as Type : o.GetType();
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullableValueType(this object o)
        {
            Type t = o.GetType();
            Type[] ga = t.GetGenericArguments();
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)
                && ga.Length == 1 && (ga[0].IsPrimitive || ga[0].IsValueType);
        }

        public static Type AutoResolveNullable(this object o)
        {
            Type t = (o is Type) ? t = o as Type : o.GetType();
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments()[0];
            }
            else
            {
                return t;
            }
        }

        public static Type AutoResolveNullable(this Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments()[0];
            }
            else
            {
                return t;
            }
        }

        public static byte[] ReadToEnd(this Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            var types = new List<Type>(type.GetInterfaces());
            if (type.BaseType != null)
            {
                var t = type;
                while (t.BaseType != null && t.BaseType != typeof(Object))
                {
                    types.Add(t.BaseType);
                    types.AddRange(t.GetInterfaces().Where(it => !types.Contains(it)));

                    t = t.BaseType;
                }
            }
            return types;
        }

        public static void CopyPropertiesFrom<T>(this object destination, object source)
        {
            Type srcType = source.GetType(), destType = destination.GetType();
            var type = typeof(T);
            if (type.IsAssignableFrom(srcType) && type.IsAssignableFrom(destType))
            {
                if (destination is IEnumerable)
                {
                    var destEnumerator = (destination as IEnumerable).GetEnumerator();
                    var srcEnumerator = (source as IEnumerable).GetEnumerator();

                    while (destEnumerator.MoveNext() && srcEnumerator.MoveNext())
                        destEnumerator.Current.CopyPropertiesFrom(srcEnumerator.Current);
                }
                else
                {
                    //FRAMEWORK VERSION CONFLICT: 4.5
                    //foreach (var property in type.GetRuntimeProperties().Where(p => p.CanRead && p.CanWrite))
                    foreach (var property in type.GetProperties().Where(p => p.CanRead && p.CanWrite))
                    {
                        //FRAMEWORK VERSION CONFLICT: 4.5
                        //property.SetValue(destination, property.GetValue(source));
                        property.SetValue(destination, property.GetValue(source, new object[] { }), new object[] { });
                    }
                }
            }
        }

        public static void CopyPropertiesFrom(this object destination, object source, bool forceTypeMatching = false)
        {
            Type srcType = source.GetType(), destType = destination.GetType();
            IEnumerable<Type> srcTypes = srcType.GetBaseTypes(), destTypes = destType.GetBaseTypes();
            Type[] sharedTypes = destTypes.Where(di => srcTypes.Any(si => si == di)).ToArray();

            if (forceTypeMatching && !destType.Equals(srcType) && sharedTypes.Length == 0)
                throw new ArgumentException("Type mismatch");

            if (destination is IEnumerable)
            {
                var destEnumerator = (destination as IEnumerable).GetEnumerator();
                var srcEnumerator = (source as IEnumerable).GetEnumerator();

                while (destEnumerator.MoveNext() && srcEnumerator.MoveNext())
                    destEnumerator.Current.CopyPropertiesFrom(srcEnumerator.Current);
            }
            else
            {
                if (!forceTypeMatching || destType == srcType)
                {
                    //FRAMEWORK VERSION CONFLICT: 4.5
                    //var destProperties = destType.GetRuntimeProperties().Where(p => p.CanWrite);
                    //foreach (var sourceProperty in srcType.GetRuntimeProperties().Where(p => p.CanRead))
                    var destProperties = destType.GetProperties().Where(p => p.CanWrite);
                    foreach (var sourceProperty in srcType.GetProperties().Where(p => p.CanRead))
                    {
                        //FRAMEWORK VERSION CONFLICT: 4.5
                        //var destProperty = destProperties.Where(p =>
                        //    p.Name == sourceProperty.Name &&
                        //    p.PropertyType.GetTypeInfo().IsAssignableFrom(sourceProperty.PropertyType.GetTypeInfo())).FirstOrDefault();
                        var destProperty = destProperties.Where(p =>
                            p.Name == sourceProperty.Name &&
                            p.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)).FirstOrDefault();

                        if (destProperty != null)
                            //FRAMEWORK VERSION CONFLICT: 4.5
                            //destProperty.SetValue(destination, sourceProperty.GetValue(source));
                            destProperty.SetValue(destination, sourceProperty.GetValue(source, new object[] { }), new object[] { });
                    }
                }

                if (sharedTypes != null && sharedTypes.Length > 0)
                {
                    foreach (var sharedType in sharedTypes)
                    {
                        //FRAMEWORK VERSION CONFLICT: 4.5
                        //foreach (var prop in sharedType.GetRuntimeProperties().Where(p => p.CanWrite && p.CanRead))
                        foreach (var prop in sharedType.GetProperties().Where(p => p.CanWrite && p.CanRead))
                        {
                            //FRAMEWORK VERSION CONFLICT: 4.5
                            //prop.SetValue(destination, prop.GetValue(source));
                            prop.SetValue(destination, prop.GetValue(source, new object[] { }), new object[] { });
                        }
                    }
                }
            }
        }

        public static void AppendIfExists(this StringBuilder sb, string targetString, bool addPrefixPadding = true, string paddingString = " ")
        {
            if (!String.IsNullOrEmpty(targetString))
            {
                if (addPrefixPadding)
                {
                    sb.Append(paddingString);
                }
                sb.Append(targetString);
            }
        }

        public static void AddIfNotNull<K>(this BindingList<K> list, K k)
        {
            if (k != null)
            {
                list.Add(k);
            }
        }

        public static string ToEnumDescription<T>(this T val) where T : struct
        {
            var type = typeof(T);
            var name = Enum.GetName(type, val);
            var descriptionAttributes = type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];

            return (descriptionAttributes != null && descriptionAttributes.Length > 0) ? descriptionAttributes[0].Description : null;
        }

        public static T FromEnumDescriptonTo<T>(this string desc)
        {
            var type = typeof(T);
            foreach (var name in Enum.GetNames(type))
            {
                var descriptionAttributes = type.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                var attributeDescription = (descriptionAttributes != null && descriptionAttributes.Length > 0) ? descriptionAttributes[0].Description : null;

                if (attributeDescription == desc) return (T)Enum.Parse(type, name);
            }
            return default(T);
        }
    }
}