// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.IO;
// using System.Reflection;
// using Google.Protobuf.WellKnownTypes;
// using log4net;
// using MiNET.Blocks;
// using MiNET.Utils;
// using Type = Google.Protobuf.WellKnownTypes.Type;
//
// namespace CyberCore.Utils
// {
//     [AttributeUsage(AttributeTargets.Property)]
//     public class SaveIgnore : Attribute
//     {
//     }
//
//     [AttributeUsage(AttributeTargets.Property)]
//     public class SavePath : Attribute
//     {
//         public String Path { get; set; }
//     }
//
//     public class CustomDictionary<String,Object>
//     {
//         public DataHolder CFG { get; set; } = null;
//         // public Dictionary<String,Object> Data { get; private set; }= new Dictionary<string, object>();
//
//         private static readonly ILog Log = LogManager.GetLogger(typeof(CustomDictionary<String,Object>));
//
//         private void LoadValues(string data)
//         {
//             Dictionary<string, object> dictionary = new Dictionary<string, object>();
//             string str1 = data;
//             string[] separator = new string[3]
//             {
//                 "\r\n",
//                 "\n",
//                 Environment.NewLine
//             };
//             foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
//             {
//                 string str3 = str2.Trim();
//                 if (!str3.StartsWith("#") && str3.Contains("="))
//                 {
//                     string[] strArray = str3.Split('=', 2, StringSplitOptions.None);
//                     string lower = strArray[0].ToLower();
//                     string str4 = strArray[1];
//                     Log.Debug((object) (lower + "=" + str4));
//                     if (!dictionary.ContainsKey(lower))
//                         dictionary.Add(lower, AddParseValue(str4));
//                 }
//             }
//
//             CFG = new DataHolder(dictionary);
//         }
//
//         public Object AddParseValue(String val)
//         {
//             try
//             {
//                 if (int.TryParse(val, out int i)) return i;
//                 if (double.TryParse(val, out double d)) return d;
//                 if (long.TryParse(val, out long l)) return l;
//                 if (decimal.TryParse(val, out decimal dd)) return dd;
//                 if (DateTime.TryParse(val, out DateTime dt)) return dt;
//                 if (val.Contains("{") && val.Contains("}")) //Dictionary
//                 /*
//                  *    { key1=value1,key2=123,key3=33.3,key4=44444444444}
//                  * 
//                  */
//                 {
//                     //TODO
//                 }
//
//                 if (val.Contains("[") && val.Contains("]"))//List
//                 {
//                     String[] t = val.Split("]",StringSplitOptions.RemoveEmptyEntries);
//                     if(t.Length ==  2)
//                 }
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//             }
//
//             return val;
//         }
//
//         string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
//
//         public CustomDictionary<String,Object>()
//         {
//             if (CFG == null) return;
//             //Or GetProperties()
//             foreach (FieldInfo entry in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
//             {
//                 if (entry.Name.Equals("configFile", StringComparison.OrdinalIgnoreCase))
//                 {
//                     String fn = (string) entry.GetValue(this);
//                     String fpath = Path.Combine(directoryName, Config.ConfigFileName);
//                     Log.Info((object) ("Trying to load config-file " + fpath));
//                     if (File.Exists(fpath)) LoadValues(File.ReadAllText(fpath));
//                     if (CFG.Count == 0) return;
//                     continue;
//                 }
//
//                 if (skipSave(entry)) continue;
//                 String path = getPath(entry);
//                 if (path == null) continue;
//             }
//
//             for (Field field :
//             this.getClass().getDeclaredFields()) {
//                 if (field.getName().equals("configFile")) continue;
//                 if (skipSave(field)) continue;
//                 String path = getPath(field);
//                 if (path == null) continue;
//                 if (path.isEmpty()) continue;
//                 field.setAccessible(true);
//                 try
//                 {
//                     if (field.getType() == int.class || field.getType() == int.class)
//                     field.set(this, CFG.getInt(path, field.getInt(this)));
//                     else if (field.getType() == bool.class || field.getType() == bool.class)
//                     field.set(this, CFG.getBoolean(path, field.getBoolean(this)));
//                     else if (field.getType() == long.class || field.getType() == Long.class)
//                     field.set(this, CFG.getLong(path, field.getLong(this)));
//                     else if (field.getType() == double.class || field.getType() == Double.class)
//                     field.set(this, CFG.getDouble(path, field.getDouble(this)));
//                     else if (field.getType() == String.class)
//                         field.set(this, CFG.GetString(path, (String) field.get(this)));
//                     else if (field.getType() == Dictionary<String,Object>.class)
//                         field.set(this, CFG.getSection(path));
//                     else if (field.getType() == List.class)
//                     {
//                         Type genericFieldType = field.getGenericType();
//                         if (genericFieldType instanceof ParameterizedType) {
//                             ParameterizedType aType = (ParameterizedType) genericFieldType;
//                             Class fieldArgClass = (Class) aType.getActualTypeArguments()[0];
//                             if (fieldArgClass == int.class) field.set(this, CFG.getIntegerList(path));
//                             else if (fieldArgClass == bool.class) field.set(this, CFG.getBooleanList(path));
//                             else if (fieldArgClass == Double.class) field.set(this, CFG.getDoubleList(path));
//                             else if (fieldArgClass == Character.class) field.set(this, CFG.getCharacterList(path));
//                             else if (fieldArgClass == Byte.class) field.set(this, CFG.getByteList(path));
//                             else if (fieldArgClass == Float.class) field.set(this, CFG.getFloatList(path));
//                             else if (fieldArgClass == Short.class) field.set(this, CFG.getFloatList(path));
//                             else if (fieldArgClass == String.class) field.set(this, CFG.getStringList(path));
//                         } else field.set(this, CFG.getList(path)); // Hell knows what's kind of List was found :)
//                     }
//                     else
//                         throw new IllegalStateException("SimpleConfig did not supports class: " +
//                                                         field.getType().getName());
//                 }
//                 catch (Exception e)
//                 {
//                     Server.getInstance().getLogger().logException(e);
//                     return;
//                 }
//             }
//         }
//
//         private bool skipSave(FieldInfo field)
//         {
//             if (field.GetCustomAttributes(typeof(SaveIgnore), false).Length == 0) return false;
//             return true;
//         }
//
//         private bool skipLoad(Field field)
//         {
//             if (!field.isAnnotationPresent(SimpleConfig.Skip.class)) return false;
//             return field.getAnnotation(SimpleConfig.Skip.class).skipLoad();
//         }
//
//         private String getPath(FieldInfo field)
//         {
//             String path = null;
//             var a = field.GetCustomAttributes(typeof(SavePath), false);
//             if (a.Length != 0)
//             {
//                 var aa = (SavePath) a[0];
//                 if (aa != null)
//                 {
//                     path = aa.Path;
//                 }
//             }
//
//             if (path == null) path = field.Name.Replace("_", ".");
//             if (Modifier.isFinal(field.getModifiers())) return null;
//             if (Modifier.isPrivate(field.getModifiers())) field.setAccessible(true);
//             return path;
//         }
//
//         public Dictionary<String,Object> save()
//         {
//             Dictionary<String,Object> cfg = new Dictionary<String,Object>();
//             for (Field field :
//             this.getClass().getDeclaredFields()) {
//                 if (skipSave(field)) continue;
//                 String path = getPath(field);
//                 try
//                 {
//                     cfg.set(path, field.get(this));
//                 }
//                 catch (Exception e)
//                 {
//                     e.printStackTrace();
//                 }
//
// //            cfg.put(path, field.get(this));
//             }
//             return cfg;
//         }
//
//         public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
//         {
//             throw new NotImplementedException();
//         }
//
//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return GetEnumerator();
//         }
//
//         public void Add(KeyValuePair<string, object> item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Clear()
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool Contains(KeyValuePair<string, object> item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool Remove(KeyValuePair<string, object> item)
//         {
//             throw new NotImplementedException();
//         }
//
//         public int Count { get; }
//         public bool IsReadOnly { get; }
//
//         public void Add(string key, object value)
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool ContainsKey(string key)
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool Remove(string key)
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool TryGetValue(string key, out object value)
//         {
//             throw new NotImplementedException();
//         }
//
//         public object this[string key]
//         {
//             get => throw new NotImplementedException();
//             set => throw new NotImplementedException();
//         }
//
//         public ICollection<string> Keys { get; }
//         public ICollection<object> Values { get; }
//     }
// }