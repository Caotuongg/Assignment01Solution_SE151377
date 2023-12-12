using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Ultilities
{
    public interface AutoGenId
    {
        public static string AutoGenerateId()
        {
            Guid id = Guid.NewGuid();
            string idString = id.ToString("N"); // Convert to a hexadecimal string without dashes
            return idString.Substring(0, Math.Min(20, idString.Length)); // Take the first 20 characters
        }
    }
}
