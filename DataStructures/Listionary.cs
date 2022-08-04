using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures
{
    public class Listionary<Key, Value>: IEnumerable<Value> where Key : notnull
    {
        public Dictionary<Key, List<Value>>? Dict { get; protected set; } = null;

        public bool Add(Key k, Value v)
        {
            if(Dict == null)
                Dict = new();

            if(!Dict.TryGetValue(k, out var list))
            {
                list = new List<Value>();
                Dict.Add(k, list);
            }

            list.Add(v);

            return true;
        }

        public bool Add(Key k, List<Value> v)
        {
            if (Dict == null)
                Dict = new();

            if (!Dict.TryGetValue(k, out var list))
            {
                Dict.Add(k, v);
                return true;
            }

            list.AddRange(v);

            return true;
        }

        public bool Remove(Key k, Value v)
        {
            if (Dict == null)
                return false;

            if (!Dict.TryGetValue(k, out var list))
                return false;

            return list.Remove(v);
        }

        public bool TryGet(Key k, out List<Value>? v)
        {
            if (Dict == null)
            {
                v = null;
                return false;
            }

            if (!Dict.TryGetValue(k, out var list))
            {
                v = null;
                return false;
            }

            v = list;
            return true;
        }

        public List<Value>? Get(Key k)
        {
            if (Dict == null)
            {
                return null;
            }

            if (!Dict.TryGetValue(k, out var list))
            {
                return null;
            }

            return list;
        }

        public IEnumerator<Value> GetEnumerator()
        {
            if(Dict == null)
                yield break;

            foreach (List<Value> valueList in Dict.Values)
            {
                foreach (Value v in valueList)
                {
                    yield return v;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
