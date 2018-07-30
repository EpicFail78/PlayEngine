using System;

namespace PlayEngine.Helpers {
   public static class dotNetExtensions {
      public static bool Contains(this String source, String toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase) {
         return source != null && toCheck != null && source.IndexOf(toCheck, comparison) >= 0;
      }
   }
}
