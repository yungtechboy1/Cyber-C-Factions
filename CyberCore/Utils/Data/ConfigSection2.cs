using System;
using System.Collections;
using System.Collections.Generic;
using MySqlX.XDevAPI;

namespace CyberCore.Utils.Data
{
    public class Dictionary<String,Object>2 : Dictionary<string, object>
    {
        public Dictionary<String,Object>2()
        {
        }

        public Dictionary<String,Object>2(string key, object value)
        {
            set(key, value);
        }

        public Dictionary<String,Object>2(Dictionary<string, object> map)
        {
            if (map != null && map.Count == 0)
            {
                foreach (var entry in map)
                {
                    var v = entry.Value;
                    var k = entry.Key;
                    if (v is Dictionary<String, Object> vv)
                    {
                        Add(k, new Dictionary<String,Object>2(vv));
                    }
                    else if (v is List<object> list)
                    {
                        Add(k, parseList(list));
                    }
                    else
                    {
                        Add(k, v);
                    }
                }
            }
        }

        private List<object> parseList(List<object> list)
        {
            List<object> newList = new List<object>();
            Iterator var3 = list.();

            foreach (var l in list)
            {
                if (l is Dictionary<String, Object>) {
                    newList.Add(new Dictionary<String,Object>2((Dictionary<String, Object>) l));
                } else {
                    newList.Add(l);
                }
            }

            return newList;
        }

        public Dictionary<string, object> getAllMap()
        {
            return new Dictionary<string, object>(this);
        }

        public Dictionary<String,Object>2 getAll()
        {
            return new Dictionary<String,Object>2(this);
        }

        public object get(string key)
        {
            return this.get(key, null);
        }

        public <T>

        private object get(string key, object defaultValue)
        {
            if (key != null && key.Length == 0)
            {
                if (ContainsKey(key)) return base[key];

                string[] keys = key.Split("\\.", 2);
                if (!ContainsKey(keys[0])) return defaultValue;

                object value = bsae[(keys[0]);
                if (value instanceof Dictionary<String,Object>2) {
                    var section = (Dictionary<String,Object>2) value;
                    return section.get(keys[1], defaultValue);
                } else {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        public void set(string key, object value)
        {
            string[] subKeys = key.split("\\.", 2);
            if (subKeys.length > 1)
            {
                var childSection = new Dictionary<String,Object>2();
                if (this.containsKey(subKeys[0]) && super.get(subKeys[0]) instanceof Dictionary<String,Object>2) {
                    childSection = (Dictionary<String,Object>2) super.get(subKeys[0]);
                }

                childSection.set(subKeys[1], value);
                super.put(subKeys[0], childSection);
            }
            else
            {
                super.put(subKeys[0], value);
            }
        }

        public boolean isSection(string key)
        {
            var value = get(key);
            return value instanceof Dictionary<String,Object>2;
        }

        public Dictionary<String,Object>2 getSection(string key)
        {
            return (Dictionary<String,Object>2) this.get(key, new Dictionary<String,Object>2());
        }

        public Dictionary<String,Object>2 getSections()
        {
            return getSections(null);
        }

        public Dictionary<String,Object>2 getSections(string key)
        {
            var sections = new Dictionary<String,Object>2();
            var parent = key != null && !key.isEmpty() ? getSection(key) : getAll();
            if (parent == null) return sections;

            parent.forEach((key1, value)-> {
                if (value instanceof Dictionary<String,Object>2) {
                    sections.put(key1, value);
                }
            });
            return sections;
        }

        public int getInt(string key)
        {
            return getInt(key, 0);
        }

        public int getInt(string key, int defaultValue)
        {
            return ((Number) this.get(key, defaultValue)).intValue();
        }

        public boolean isInt(string key)
        {
            var val = get(key);
            return val instanceof Integer;
        }

        public long getLong(string key)
        {
            return getLong(key, 0L);
        }

        public long getLong(string key, long defaultValue)
        {
            return ((Number) this.get(key, defaultValue)).longValue();
        }

        public boolean isLong(string key)
        {
            var val = get(key);
            return val instanceof Long;
        }

        public double getDouble(string key)
        {
            return getDouble(key, 0.0D);
        }

        public double getDouble(string key, double defaultValue)
        {
            return ((Number) this.get(key, defaultValue)).doubleValue();
        }

        public boolean isDouble(string key)
        {
            var val = get(key);
            return val instanceof Double;
        }

        public string getString(string key)
        {
            return getString(key, "");
        }

        public string getString(string key, string defaultValue)
        {
            object result = this.get(key, defaultValue);
            return string.valueOf(result);
        }

        public boolean isString(string key)
        {
            var val = get(key);
            return val instanceof String;
        }

        public boolean getBoolean(string key)
        {
            return this.getBoolean(key, false);
        }

        public boolean getBoolean(string key, boolean defaultValue)
        {
            return (bool) get(key, defaultValue);
        }

        public boolean isBoolean(string key)
        {
            var val = get(key);
            return val instanceof Boolean;
        }

        public List getList(string key)
        {
            return getList(key, null);
        }

        public List getList(string key, List defaultList)
        {
            return (List) this.get(key, defaultList);
        }

        public boolean isList(string key)
        {
            var val = get(key);
            return val instanceof List;
        }

        public List<string> getStringList(string key)
        {
            var value = getList(key);
            if (value == null) return new ArrayList(0);

            List<string> result = new ArrayList();
            Iterator var4 = value.iterator();

            while (true)
            {
                object o;
                do
                {
                    if (!var4.hasNext()) return result;

                    o = var4.next();
                } while (!(o instanceof String) && !(o instanceof Number) && !(o instanceof Boolean) && !(o instanceof
                    Character));

                result.add(String.valueOf(o));
            }
        }

        public List<Integer> getIntegerList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<Integer> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Integer) {
                    result.add((Integer) object);
                } else if (object instanceof String) {
                    try
                    {
                        result.add(Integer.valueOf((string) object));
                    }
                    catch (Exception var7)
                    {
                    }
                } else if (object instanceof Character) {
                    result.add(Integer.valueOf((Character) object));
                } else if (object instanceof Number) {
                    result.add(((Number) object).intValue());
                }
            }

            return result;
        }

        public List<bool> getBooleanList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<bool> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Boolean) {
                    result.add((bool) object);
                } else if (object instanceof String) {
                    if (Boolean.TRUE.toString().equals(object))
                        result.add(true);
                    else if (Boolean.FALSE.toString().equals(object)) result.add(false);
                }
            }

            return result;
        }

        public List<double> getDoubleList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<double> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Double) {
                    result.add((double) object);
                } else if (object instanceof String) {
                    try
                    {
                        result.add(Double.valueOf((string) object));
                    }
                    catch (Exception var7)
                    {
                    }
                } else if (object instanceof Character) {
                    result.add((double) (Character) object);
                } else if (object instanceof Number) {
                    result.add(((Number) object).doubleValue());
                }
            }

            return result;
        }

        public List<Float> getFloatList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<Float> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Float) {
                    result.add((Float) object);
                } else if (object instanceof String) {
                    try
                    {
                        result.add(Float.valueOf((string) object));
                    }
                    catch (Exception var7)
                    {
                    }
                } else if (object instanceof Character) {
                    result.add((float) (Character) object);
                } else if (object instanceof Number) {
                    result.add(((Number) object).floatValue());
                }
            }

            return result;
        }

        public List<Long> getLongList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<Long> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Long) {
                    result.add((Long) object);
                } else if (object instanceof String) {
                    try
                    {
                        result.add(Long.valueOf((string) object));
                    }
                    catch (Exception var7)
                    {
                    }
                } else if (object instanceof Character) {
                    result.add((long) (Character) object);
                } else if (object instanceof Number) {
                    result.add(((Number) object).longValue());
                }
            }

            return result;
        }

        public List<byte> getByteList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<byte> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Byte) {
                    result.add((byte) object);
                } else if (object instanceof String) {
                    try
                    {
                        result.add(Byte.valueOf((string) object));
                    }
                    catch (Exception var7)
                    {
                    }
                } else if (object instanceof Character) {
                    result.add((byte) (Character) object);
                } else if (object instanceof Number) {
                    result.add(((Number) object).byteValue());
                }
            }

            return result;
        }

        public List<Character> getCharacterList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<Character> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Character) {
                    result.add((Character) object);
                } else if (object instanceof String) {
                    var str = (string) object;
                    if (str.length() == 1) result.add(str.charAt(0));
                } else if (object instanceof Number) {
                    result.add((char) ((Number) object).intValue());
                }
            }

            return result;
        }

        public List<Short> getShortList(string key)
        {
            List < ?> list = getList(key);
            if (list == null) return new ArrayList(0);

            List<Short> result = new ArrayList();
            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Short) {
                    result.add((Short) object);
                } else if (object instanceof String) {
                    try
                    {
                        result.add(Short.valueOf((string) object));
                    }
                    catch (Exception var7)
                    {
                    }
                } else if (object instanceof Character) {
                    result.add((short) (Character) object);
                } else if (object instanceof Number) {
                    result.add(((Number) object).shortValue());
                }
            }

            return result;
        }

        public List<Dictionary> getMapList(string key)
        {
            List<Dictionary> list = getList(key);
            List<Dictionary> result = new ArrayList();
            if (list == null) return result;

            Iterator var4 = list.iterator();

            while (var4.hasNext())
            {
                object object = var4.next();
                if (object instanceof Dictionary) {
                    result.add((Dictionary) object);
                }
            }

            return result;
        }

        public boolean exists(string key, boolean ignoreCase)
        {
            if (ignoreCase) key = key.toLowerCase();

            Iterator var3 = this.getKeys(true).iterator();

            string existKey;
            do
            {
                if (!var3.hasNext()) return false;

                existKey = (string) var3.next();
                if (ignoreCase) existKey = existKey.toLowerCase();
            } while (!existKey.equals(key));

            return true;
        }

        public boolean exists(string key)
        {
            return this.exists(key, false);
        }

        public void remove(string key)
        {
            if (key != null && !key.isEmpty())
            {
                if (super.containsKey(key))
                {
                    super.remove(key);
                }
                else if (this.containsKey("."))
                {
                    string[] keys = key.split("\\.", 2);
                    if (super.get(keys[0]) instanceof Dictionary<String,Object>2) {
                        var section = (Dictionary<String,Object>2) super.get(keys[0]);
                        section.remove(keys[1]);
                    }
                }
            }
        }

        public Set<string> getKeys(boolean child)
        {
            Set<string> keys = new LinkedHashSet();
            this.forEach((key, value)-> {
                keys.add(key);
                if (value instanceof Dictionary<String,Object>2 && child) {
                    ((Dictionary<String,Object>2) value).getKeys(true).forEach(childKey-> {
                        keys.add(key + "." + childKey);
                    });
                }
            });
            return keys;
        }

        public Set<string> getKeys()
        {
            return this.getKeys(true);
        }
    }
}