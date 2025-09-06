// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("IKOtopIgo6igIKOjohGnoMLpwSqSIKOAkq+kq4gk6iRVr6Ojo6eioWDyuM+3Pf13lL72zV5mgItgPqvPe+qehvA49UaerVlozY96xzz5RMAzC077hU95hNNnDfLflqoj2xprlJ5P/Oz+h0Y3SUgSf5RJe9ZqUzFD4+EOpbP0PZxn6PSoRzWhDTtLQs546+VDU/cnvDFR8jJVaZ3F73kGU8boA+KR7DoAIilDb5SwpYlxPx0sfobuHm2b42n3aGu9qiH+G3AptoZZJlOBOcGodIjo2NOFaqm3ugoEM/Hd6Uxevjj45W419XTgXWgtb4ZX/D9UjIsrtnRxEpiU186AfTHqfMWMSZud9KcJ8pwzZaslywkFpJkt5RmEiJZ8n3Sd4aCho6Kj");
        private static int[] order = new int[] { 1,1,12,12,8,6,13,9,11,13,11,13,12,13,14 };
        private static int key = 162;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
