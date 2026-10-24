using System.Runtime.InteropServices;
using System.Text;

namespace Sudzekai.WebOs.Core
{
    public static unsafe class AllocationUtilities
    {
        /// <summary>
        /// Выделяет массив UTF8-строк (null-terminated) в CoTaskMem.
        /// Возвращает null при пустом списке.
        /// </summary>
        public static nint* AllocStringArray(IEnumerable<string>? items, out int count)
        {
            var list = items?.ToArray() ?? [];
            count = list.Length;

            if (count == 0) return null;

            var arr = AllocArray<nint>(count, out _);

            for (int i = 0; i < count; i++)
                arr[i] = AllocString(list[i], out _);

            return arr;
        }

        /// <summary>
        /// Выделяет null-terminated UTF8-строку в CoTaskMem.
        /// Пустая/нулевая строка => nint.Zero, длина 0.
        /// </summary>
        public static nint AllocString(string? value, out int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                length = 0;
                return nint.Zero;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            length = bytes.Length;

            nint ptr = Marshal.AllocCoTaskMem(bytes.Length + 1);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteByte(ptr, bytes.Length, 0);

            return ptr;
        }

        /// <summary>
        /// Выделяет массив из count элементов типа T в CoTaskMem (T должен быть unmanaged).
        /// Возвращает null при count == 0.
        /// </summary>
        public static T* AllocArray<T>(int count, out int bytes) where T : unmanaged
        {
            bytes = count * sizeof(T);

            if (count == 0) return null;

            return (T*)Marshal.AllocCoTaskMem(bytes);
        }

        /// <summary>
        /// Копирует managed-массив в свежевыделенный CoTaskMem-блок.
        /// </summary>
        public static T* CopyArray<T>(ReadOnlySpan<T> source, out int count) where T : unmanaged
        {
            count = source.Length;

            if (count == 0) return null;

            var dest = AllocArray<T>(count, out _);

            for (int i = 0; i < count; i++)
                dest[i] = source[i];

            return dest;
        }

        /// <summary>
        /// Копирует содержимое unmanaged-массива обратно в managed.
        /// </summary>
        public static T[] ToArray<T>(T* source, int count) where T : unmanaged
        {
            if (source == null || count <= 0) return [];

            var result = new T[count];

            for (int i = 0; i < count; i++)
                result[i] = source[i];

            return result;
        }

        /// <summary>
        /// Освобождает массив UTF8-строк: каждую строку, затем сам массив.
        /// Безопасен при null / count == 0.
        /// </summary>
        public static void FreeStringArray(nint* arrayPtr, int count)
        {
            if (arrayPtr == null) return;

            for (int i = 0; i < count; i++)
                Marshal.FreeCoTaskMem(arrayPtr[i]);

            Marshal.FreeCoTaskMem((nint)arrayPtr);
        }

        public static void FreeIfAllocated(ref nint obj)
        {
            if (obj != nint.Zero)
            {
                Marshal.FreeCoTaskMem(obj);
                obj = nint.Zero;
            }
        }

        /// <summary>
        /// Читает UTF8-строку из unmanaged-памяти по указателю и длине (в байтах).
        /// null/нулевой указатель или length &lt;= 0 => пустая строка.
        /// </summary>
        public static string ToString(nint ptr, int length)
        {
            if (ptr == nint.Zero || length <= 0)
                return string.Empty;

            return Encoding.UTF8.GetString((byte*)ptr, length);
        }

        /// <summary>
        /// Читает массив UTF8-строк (null-terminated) из unmanaged-памяти в managed-массив.
        /// </summary>
        public static string[] ToStringArray(nint* arrayPtr, int count)
        {
            if (arrayPtr == null || count <= 0)
                return [];

            var result = new string[count];

            for (int i = 0; i < count; i++)
            {
                var ptr = arrayPtr[i];
                if (ptr == nint.Zero)
                    continue;

                int len = 0;
                
                while (((byte*)ptr)[len] != 0) 
                    len++;

                if (len == 0)
                    continue;


                result[i] = Encoding.UTF8.GetString((byte*)ptr, len);
            }    
            
            return result;
        }

        /// <summary>
        /// Читает байты из unmanaged-памяти в byte[]. null/нулевой указатель => пустой массив.
        /// </summary>
        public static byte[] ToBytes(nint ptr, int length)
        {
            if (ptr == nint.Zero || length <= 0)
                return [];

            var result = new byte[length];
            Marshal.Copy(ptr, result, 0, length);
            return result;
        }

        /// <summary>
        /// Копирует byte[] в свежевыделенный CoTaskMem-блок. null/пустой => nint.Zero, длина 0.
        /// </summary>
        public static nint CopyToUnmanaged(byte[]? bytes, out int length)
        {
            length = bytes?.Length ?? 0;
            if (length == 0) return nint.Zero;

            var ptr = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(bytes!, 0, ptr, length);
            return ptr;
        }
    }
}