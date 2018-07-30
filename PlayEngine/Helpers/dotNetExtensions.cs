using System;
using System.Text;

namespace PlayEngine.Helpers {
   public static class dotNetExtensions {
      public static bool Contains(this String source, String toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase) {
         return source != null && toCheck != null && source.IndexOf(toCheck, comparison) >= 0;
      }

      /// <summary>
      /// Returns <typeparamref name="T"/> instance of the <paramref name="byteArray"/>.
      /// </summary>
      /// <param name="byteArray"></param>
      /// <returns></returns>
      public static Object getObject(this byte[] byteArray, Type objectType) {
         switch (Type.GetTypeCode(objectType)) {
            case TypeCode.Boolean:
               return BitConverter.ToBoolean(byteArray, 0);
            case TypeCode.Byte:
               return byteArray[0];
            case TypeCode.Char:
               return Encoding.UTF8.GetChars(byteArray)[0];
            case TypeCode.Double:
               return BitConverter.ToDouble(byteArray, 0);
            case TypeCode.Int16:
               return BitConverter.ToInt16(byteArray, 0);
            case TypeCode.Int32:
               return BitConverter.ToInt32(byteArray, 0);
            case TypeCode.Int64:
               return BitConverter.ToInt64(byteArray, 0);
            case TypeCode.Single:
               return BitConverter.ToSingle(byteArray, 0);
            case TypeCode.UInt16:
               return BitConverter.ToUInt16(byteArray, 0);
            case TypeCode.UInt32:
               return BitConverter.ToUInt32(byteArray, 0);
            case TypeCode.UInt64:
               return BitConverter.ToUInt64(byteArray, 0);
         }

         return null;
      }

      /// <summary>
      /// Returns an initialized byte[] of the given <typeparamref name="T"/> object.
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public static byte[] getBytes<T>(this T obj) {
         switch (Type.GetTypeCode(obj.GetType())) {
            case TypeCode.Boolean:
               return BitConverter.GetBytes((bool)(object)obj);
            case TypeCode.Char:
               return Encoding.UTF8.GetBytes(new[] { (char)(object)obj });
            case TypeCode.Double:
               return BitConverter.GetBytes((double)(object)obj);
            case TypeCode.Int16:
               return BitConverter.GetBytes((short)(object)obj);
            case TypeCode.Int32:
               return BitConverter.GetBytes((int)(object)obj);
            case TypeCode.Int64:
               return BitConverter.GetBytes((long)(object)obj);
            case TypeCode.Single:
               return BitConverter.GetBytes((float)(object)obj);
            case TypeCode.UInt16:
               return BitConverter.GetBytes((ushort)(object)obj);
            case TypeCode.UInt32:
               return BitConverter.GetBytes((uint)(object)obj);
            case TypeCode.UInt64:
               return BitConverter.GetBytes((ulong)(object)obj);
         }

         return new byte[1];
      }
   }
}
