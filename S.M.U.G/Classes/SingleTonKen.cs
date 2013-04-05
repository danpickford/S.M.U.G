using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S.M.U.G.Classes
{
    public sealed class SingleTonKen
    {
        private static readonly SingleTonKen instance = new SingleTonKen();

        private SingleTonKen() {}
        internal string AccessTokenResponse = null;

           public static SingleTonKen Instance
           {
              get 
              {
                 return instance; 
              }
           }
    }
}
