using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful
{
    public class ParamCollection : Dictionary<string, string>
    {
        public int? GetInt(string key)
        {
            if (ContainsKey(key))
            {
                if (int.TryParse(this[key], out int val))
                    return val;
            }
            return null;
        }

        public double? GetDouble(string key)
        {
            if (ContainsKey(key))
            {
                if (double.TryParse(this[key], NumberStyles.Number, CultureInfo.InvariantCulture, out double val))
                    return val;
            }
            return null;
        }
    }
}
