namespace dOhAuth
{
    public sealed class SingleTonKen
    {
        private static readonly SingleTonKen instance = new SingleTonKen();

        private SingleTonKen() {}
        public string AccessTokenResponse = null;

           public static SingleTonKen Instance
           {
              get 
              {
                 return instance; 
              }
           }
    }
}
