// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SUqz9p6b1W9xEwAO4a/6SKy7/old72xPXWBrZEfrJeuaYGxsbGhtbh8hq+rbqVbxo1pMpWatPSrCUw8bMTAaF9+TUOWV2jGimiFd6Yvs3I4fkKEmpokuUaFi3Mvcogsw/b9UKgNawAFSxWXahSD7Qq1ZGi3vRh9YaOAXay+LWMl81ZrgL6i6qxr8/2++CCMG3AhD0mIZwGz9QEIIWK0BwKqKSBsjQ1kGw50NHqULLubjCY3f72xibV3vbGdv72xsbdemg+xFj4oejOxZpzijFRjG+8W9LR4Szcui9gwwizY//1FGC8ZbWFFubAAaOaOipWHHWw5wtD4eCS/gOHC4E+4qNJstIljHwFVkkAY924hn743Mk4orC+gKTkDHbXXoiG9ubG1s");
        private static int[] order = new int[] { 9,9,13,7,11,7,7,12,12,9,13,13,12,13,14 };
        private static int key = 109;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
