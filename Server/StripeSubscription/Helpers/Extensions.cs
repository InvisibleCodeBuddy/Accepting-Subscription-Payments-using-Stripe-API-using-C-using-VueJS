using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StripeSubscription.Helpers
{
    public static class Extensions
    {
        public static T ToInstance<T>(this IEnumerable<KeyValuePair<string, string>> source)
        {
            var dictionary = source.ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);
            var result = Activator.CreateInstance<T>();
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                if (dictionary.TryGetValue(field.Name, out var value))
                    field.SetValue(result, value);
            return result;
        }
    }
}
