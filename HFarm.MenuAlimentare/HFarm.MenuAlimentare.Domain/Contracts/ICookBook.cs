using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFarm.MenuAlimentare.Domain.Contracts
{
    public interface ICookBook : IEnumerable<KeyValuePair<string, string>>
    {
        string Get(string dish);

        void Set(string dish, string dishType);

        void Remove(string dish);

        void CleanAndSaveCookBook();

        void Clear();
    }
}
