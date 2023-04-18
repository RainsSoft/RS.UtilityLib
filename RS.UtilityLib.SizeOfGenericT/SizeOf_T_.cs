using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;


namespace RS.UtilityLib.SizeOfGenericT
{
    /// <summary>
    /// A type is an unmanaged type if it's any of the following types: [ 如果类型是以下任一类型，则类型为非托管类型：]
    ///sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint, char, float, double, decimal, or bool
    ///Any enum type [任何枚举类型]
    ///Any pointer type[任何指针类型]
    ///Any user-defined struct type that contains fields of unmanaged types only. [仅包含非托管类型字段的任何用户定义结构类型。]
    ///You can use the unmanaged constraint to specify that a type parameter is a non-pointer, non-nullable unmanaged type. [可以使用非托管约束指定类型参数是非指针、不可为空的非托管类型。]
    /// </summary>
    public unsafe static class SizeOf_T_
    {// Extensions {
        /// <summary>
        /// 如果是泛型class类，通常返回IntPtr.Size-[值4]，如果是StructLayout(LayoutKind...)结构布局且checkClassStructLayout==false，则与Marshal.SizeOf(class)返回的值不一样
        /// 如果是泛型struct结构体,则与Marshal.SizeOf(new T()/default(T))返回结果一致
        /// </summary>
        /// <typeparam name="T">具体类型</typeparam>
        /// <param name="checkClassStructLayout">检查class的StructLayout内存布局,如果为true且泛型类型是class,则尝试取Marshal.SizeOf(class)的尺寸</param>
        /// <param name="AOT"></param>
        /// <returns></returns>
        public static int SizeOf<T>(bool checkClassStructLayout = false, bool AOT = false) {
            return SizeOf(typeof(T), null, checkClassStructLayout, AOT);
        }

        public static int SizeOf(this Type type, FieldInfo field, bool checkClassStructLayout, bool AOT = false) {
            if (IsGenericType(type)) {
                return SizeOfInGenericMap(type);
            }
            bool isUnManaged = type.IsUnManaged();
            byte canMarshalSize = 0;
            if (isUnManaged) {
                canMarshalSize = 1;
                try {
                    int sz = System.Runtime.InteropServices.Marshal.SizeOf(type);
                    canMarshalSize = 2;
                    return sz;
                }
                catch (Exception ee) {
                    //不能使用Marshal.SizeOf,但是计算出来后可以加入缓存                    
                }
            }
            else {
                if (checkClassStructLayout &&
                    (type.IsExplicitLayout || type.IsLayoutSequential)) {
                    try {
                        int sz = System.Runtime.InteropServices.Marshal.SizeOf(type);
                        //canMarshalSize = 2;
                        return sz;
                    }
                    catch (Exception ee) {
                        //不能使用Marshal.SizeOf,但是计算出来后可以加入缓存                    
                    }
                }
            }
            if (AOT) {
                //静态编译
                if (type.IsPrimitive) {
                    if (type == typeof(byte) || type == typeof(SByte) || type == typeof(bool))
                        return 1;

                    if (type == typeof(short) || type == typeof(ushort) || type == typeof(char))
                        return 2;

                    if (type == typeof(int) || type == typeof(uint) || type == typeof(float))
                        return 4;

                    if (type == typeof(long) || type == typeof(ulong) || type == typeof(double))
                        ////这里指针可能有问题，如果是32位系统呢(4)？64位系统(8)
                        //||type == typeof(IntPtr) 
                        //||type == typeof(UIntPtr))
                        return 8;
                    if (type == typeof(IntPtr)) {
                        return IntPtr.Size;
                    }
                    if (type == typeof(UIntPtr)) {
                        return UIntPtr.Size;
                    }
                }
                else {
                    if (type.IsValueType) {
                        //return (from fld in
                        //            type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        //        select fld.FieldType.Size(fld)).Sum();
                        int sizeVT = 0;
                        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var fld in fields) {
                            sizeVT += fld.FieldType.SizeOf(fld, true);
                        }
                        if (canMarshalSize == 1) {
                            //IsUnManaged使用Marshal.SizeOf计算错误,这里计算出来后可以加入缓存   
                            genericSizes.Add(type, sizeVT);

                        }
                        return sizeVT;
                    }

                    if (field != null) {
                        object[] attributes = field.GetCustomAttributes(typeof(MarshalAsAttribute), false);
                        if (attributes != null && attributes.Length > 0) {

                            var marshal = (MarshalAsAttribute)attributes[0];
                            if (type.IsArray)
                                return marshal.SizeConst * type.GetElementType().SizeOf(null,checkClassStructLayout, AOT);//Size();
                            if (type == typeof(string))
                                return marshal.SizeConst;
                        }
                    }
                }
                // 1.值类型泛型类，可以用
                //whereT : unmanaged 和sizeof(T)配合使用
                //或者直接使用 Unsafe.Sizeof(T)
                //2.非值类型泛型类，可用
                //Marshal.SizeOf(T)
                //计算T类型占用的内存大小，如果T是一个引用类型，那么计算的是该引用类型本身占用的内存大小（通常是一个指针大小），而不是引用类型指向的对象的大小
                return IntPtr.Size;
                //throw new Exception(type.ToString() + " is not IsUnManaged type,cann't SizeOf(...) ");
                // we don't know what   real size is so assume it's 16 bytes.
                // Microsoft recommends that structs shouldn't be bigger than this
                // anyway so it's unlikely that types will be bigger than this.
                return 16;
            }
            else {
                //支持动态编译
                var dynamicMethod = new DynamicMethod("SizeOf", typeof(int), Type.EmptyTypes);
                var generator = dynamicMethod.GetILGenerator();

                generator.Emit(OpCodes.Sizeof, type);
                generator.Emit(OpCodes.Ret);

                var function = (Func<int>)dynamicMethod.CreateDelegate(typeof(Func<int>));
                int sz = function();
                if (canMarshalSize == 1) {
                    //IsUnManaged使用Marshal.SizeOf计算错误,这里计算出来后可以加入缓存   
                    genericSizes.Add(type, sz);
                }
                return sz;
            }
        }
        /*
        static int Size(this Type type) {
            return Size(type, null);
        }

        static int Size(this Type type, FieldInfo field) {
#if SILVERLIGHT || WINDOWS_PHONE
			if ( type.IsPrimitive )
			{
				if ( type == typeof( byte ) || type == typeof( SByte ) || type == typeof( bool ) )
					return 1;

				if ( type == typeof( short ) || type == typeof( ushort ) || type == typeof( char ) )
					return 2;

				if ( type == typeof( int ) || type == typeof( uint ) || type == typeof( float ) )
					return 4;

				if ( type == typeof( long ) || type == typeof( ulong ) || type == typeof( double ) || type == typeof( IntPtr ) || type == typeof( UIntPtr ) )
					return 8;
			}
			else
			{
                if (type.IsValueType) {
                    //return (from fld in
                    //            type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    //        select fld.FieldType.Size(fld)).Sum();
                    int sizeVT = 0;
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var fld in fields) {
                        sizeVT += fld.FieldType.SizeOf(fld, true);
                    }
                    return sizeVT;
                }

				if ( field != null )
				{
					var attributes = field.GetCustomAttributes( typeof( MarshalAsAttribute ), false );
					var marshal = (MarshalAsAttribute)attributes[ 0 ];
					if ( type.IsArray )
						return marshal.SizeConst * type.GetElementType().Size();
					if ( type == typeof( string ) )
						return marshal.SizeConst;
				}
			}
#endif
            return Marshal.SizeOf(type);
        }

        */
        #region Fields
  

        #endregion

        
        /// <summary>
        /// A lookup of type sizes. Used instead of Marshal.SizeOf() which has additional
        /// overhead, but also is compatible with generic functions for simplified code.
        /// </summary>
        readonly static Dictionary<Type, int> genericSizes = new Dictionary<Type, int>()
        {
            { typeof(bool),     sizeof(bool) },
            { typeof(float),    sizeof(float) },
            { typeof(double),   sizeof(double) },
            { typeof(sbyte),    sizeof(sbyte) },
            { typeof(byte),     sizeof(byte) },
            { typeof(short),    sizeof(short) },
            { typeof(ushort),   sizeof(ushort) },
            { typeof(int),      sizeof(int) },
            { typeof(uint),     sizeof(uint) },
            { typeof(ulong),    sizeof(ulong) },
            { typeof(long),     sizeof(long) },
            //补充
            { typeof(char), sizeof(char) },
            { typeof(decimal), sizeof(decimal) },
			// common value types
			{ typeof(IntPtr), IntPtr.Size },
            { typeof(UIntPtr), UIntPtr.Size },
            { typeof(Guid), 16 },
            { typeof(DateTime), 8 },
            { typeof(TimeSpan), 8 }
        };

        /// <summary>
        /// Get the wire-size (in bytes) of a type supported by flatbuffers.
        /// </summary>
        /// <param name="t">The type to get the wire size of</param>
        /// <returns></returns>
        static int SizeOfInGenericMapT<T>() {
            return genericSizes[typeof(T)];
        }
        static int SizeOfInGenericMap(Type type) {
            return genericSizes[type];
        }
        public static bool IsStruct(Type type) {
            return type.IsValueType && !type.IsEnum;
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        /// <summary>
        /// Checks if the Type provided is supported as scalar value
        /// </summary>
        /// <typeparam name="T">The Type to check</typeparam>
        /// <returns>True if the type is a scalar type that is supported, falsed otherwise</returns>
        static bool IsGenericType<T>() {
            return genericSizes.ContainsKey(typeof(T));
        }
        static bool IsGenericType(Type type) {
            return genericSizes.ContainsKey(type);
        }

        #region Unmanaged
        private static Dictionary<Type, bool> _unmanagedCachedTypes = new Dictionary<Type, bool>();
#if NET_4_0
        
        private class UnmanagedChecker<T> where T : unmanaged
        {
            //需要.net4.0
        }
        public static bool IsUnmanaged<T>() {
            return IsUnmanaged(typeof(T));
        }
        public static bool IsUnmanaged(Type type) {
            //https://github.com/Sonozuki/NovaEngine/blob/main/NovaEngine/Extensions/TypeExtensions.cs
            try {
                // if   type is not unmanaged, MakeGenericType will throw
                //   due 到 type restrictions 于 UnmanagedChecker
                typeof(UnmanagedChecker<>).MakeGenericType(type);
                _unmanagedCachedTypes.Add(type, true);
                return true;
            }
            catch {
                _unmanagedCachedTypes.Add(type, false);
                return false;
            }
        }
        
#else
        public static bool IsUnManaged(this Type t) {
            bool result;
            if (_unmanagedCachedTypes.TryGetValue(t, out result))
                return result;

            else if (t.IsPrimitive || t.IsPointer || t.IsEnum)
                result = true;
            else if (t.IsGenericType || !t.IsValueType || Nullable.GetUnderlyingType(t) != null) {
                //增加Nullable判断
                result = false;
            }
            else {
                // result = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).All(x => x.FieldType.IsUnManaged());
                var fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                result = true;
                foreach (var fieldInfo in fieldInfos) {
                    if (!fieldInfo.FieldType.IsUnManaged()) {
                        result = false;
                        break;
                    }
                }
            }

            _unmanagedCachedTypes.Add(t, result);
            return result;
            //https://github.com/dotnet/dotNext/blob/master/src/DotNext/Reflection/TypeExtensions.cs
            //if(type.IsGenericType ||
            //  type.IsGenericTypeDefinition ||
            //  type.IsGenericParameter) {
            //    // foreach(var attribute in type.GetCustomAttributesData()) {
            //    //if(attribute.AttributeType.FullName == IsUnmanagedAttributeName)
            //    //    return true;
            //    foreach(var attribute in type.GetCustomAttributes(true)) {
            //        if((attribute as Attribute).GetType().FullName == IsUnmanagedAttributeName) {
            //            return true;
            //        }
            //    }
            //    return false;
            //}
            //if(type.IsPrimitive || type.IsPointer || type.IsEnum) {
            //    return true;
            //}
            //if(type.IsValueType) {
            //    // check all fields
            //    foreach(var field in type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)) {
            //        if(!field.FieldType.IsUnmanaged())
            //            return false;

            //    }
            //    return true;
            //}
            //return false;
        }
#endif
        #endregion

        #region Layout
        //public static List<object> GetStructFields(object obj) {
        //    List<object> fields = new List<object>();
        //    Type type = obj.GetType();
        //    foreach(var v in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
        //        Type fieldType = v.FieldType;
        //        object value = v.GetValue(obj);
        //        if(Attribute.IsDefined(v, typeof(MemoryAttribute)) && ((MemoryAttribute)v.GetCustomAttribute(typeof(MemoryAttribute))).Ignore)
        //            continue;
        //        if(IsStruct(fieldType)) {
        //            fields.AddRange(GetStructFields(value));

        //        }
        //        else if(fieldType.IsArray) {

        //            Array arr = (Array)value;

        //            for(int i = 0; i < arr.Length; i++) {
        //                object arrayValue = arr.GetValue(i);

        //                if(!isAllowedType(fieldType.GetElementType())) {
        //                    fields.AddRange(GetStructFields(arrayValue));
        //                    continue;
        //                }
        //                fields.Add(arrayValue);
        //            }
        //        }
        //        else if(fieldType.IsEnum) {
        //            Type enumType = fieldType.GetEnumUnderlyingType();
        //            fields.Add(Convert.ChangeType(value, enumType));
        //        }
        //        else {
        //            fields.Add(value);
        //        }
        //    }
        //    return fields;
        //}
        //static bool isAllowedType(Type type) {

        //    return !type.IsGenericType && !type.IsArray && !type.IsEnum && !IsStruct(type);
        //}

        //public static int GetStructFieldSize(object obj) {
        //    int size = 0;
        //    List<object> fields = GetStructFields(obj);
        //    for(int i = 0; i < fields.Count; i++) {
        //        size += Marshal.SizeOf(fields[i]);
        //    }
        //    return size;
        //}
        //
        /// <summary>
        /// 结构元素
        /// </summary>
        public struct StructElement
        {
            public IntPtr name;
            public TypeCode type;
            public short offset;
            public short size;

            public string Name {
                get {
                    return Marshal.PtrToStringAnsi(name);
                }
            }
        }
       
        readonly static int[] size = new int[]
        {
            0, 0, 0,
            sizeof(bool), sizeof(char), sizeof(sbyte), sizeof(byte),
            sizeof(short), sizeof(ushort), sizeof(int), sizeof(uint),
            sizeof(long), sizeof(ulong), sizeof(float), sizeof(double),
            sizeof(decimal), sizeof(DateTime), 0, sizeof(int) + sizeof(IntPtr)
        };

        public static StructElement[] GetLayout(this Type type, out int size) {
            if (!type.IsUnManaged()) {
                size = 0;
                return null;
            }

            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            int len = fieldInfos.Length;
            StructElement[] layout = new StructElement[len];
            size = 0;
            for (int i = 0; i < len; i++) {
                var fieldInfo = fieldInfos[i];
                //引用局部变量指的是在变量声明时使用ref关键字（或者使用ref readonly表示未只读），表示这个变量是另一个变量的引用，而不是值对象的赋值，或者引用类型的地址，这个引用可以理解为一个别名，操作这个别名对象与操作原始对象无异！　
                //ref var e = ref layout[i];
                //e.name = Marshal.StringToHGlobalAnsi(fieldInfo.Name);
                //e.type = GetTypeCode(fieldInfo.FieldType);
                //e.offset = (short)Marshal.OffsetOf(type, fieldInfo.Name);
                //e.size = (short)GetSize(fieldInfo.FieldType);
                //Debug.Assert(e.size != 0);
                //size += e.size;             
                layout[i].name = Marshal.StringToHGlobalAnsi(fieldInfo.Name);
                layout[i].type = GetTypeCode(fieldInfo.FieldType);
                layout[i].offset = (short)Marshal.OffsetOf(type, fieldInfo.Name);
                layout[i].size = (short)GetSize(fieldInfo.FieldType);
                Debug.Assert(layout[i].size != 0);
                size += layout[i].size;
            }

            return layout;
        }

        public static TypeCode GetTypeCode(Type type) {
            if (type == typeof(IntPtr)) {
                return TypeCode.Int64;
            }
            else if (type == typeof(UIntPtr)) {
                return TypeCode.UInt64;
            }

            return Type.GetTypeCode(type);
        }

        static int GetSize(Type type) {
            return size[(int)GetTypeCode(type)];
        }

        public static bool ShouldExport(this Type t) {
            if (t.IsByRef || t.IsPointer) {
                return false;
            }

            return true;
        }

        private static Dictionary<Type, string> _defaultDictionary = new Dictionary<System.Type, string>
        {
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(bool), "bool"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(char), "char"},
            {typeof(string), "string"},
            {typeof(object), "object"},
            {typeof(void), "void"}
        };

        public static string GetFriendlyName(this Type type, Dictionary<Type, string> translations) {
            if (translations.ContainsKey(type))
                return translations[type];
            else if (type.IsArray)
                return GetFriendlyName(type.GetElementType(), translations) + "[]";
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0].GetFriendlyName() + "?";
            else if (type.IsGenericType)
                //return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x)).ToArray()) + ">";
                return type.Name.Split('`')[0] + "<" + string.Join(", ", Select(type.GetGenericArguments(), x => GetFriendlyName(x)).ToArray()) + ">";
            else
                return type.FullName.Replace("+", ".");
        }

        public static string GetFriendlyName(this Type type) {
            return type.GetFriendlyName(_defaultDictionary);
        }
        static List<string> Select(Type[] args, Func<Type, string> func) {
            List<string> os = new List<string>();
            //int counter = 0;
            foreach (var element in args) {
                //yield return selector(element, counter);
                os.Add(func(element));
                //counter++;
            }
            return os;
        }
        #endregion

        #region calcul obj size
        //public static long GetSizeInSerializeBytes(object obj) {
        //            if(memoryBinaryFormatterSerialize) {
        //                var ms = new MemoryStream();
        //                var formatter = new BinaryFormatter();
        //                formatter.Serialize(ms, obj);
        //#if DEBUG
        //                //使用序列化计算出来的是不准确的，比实际大很多             
        //                //ms.Position = 0;
        //                //byte[] buf = ms.ToArray();
        //#endif
        //                return ms.Length;
        //            }
        //            else {
        //                  比实际要大一点
        //                  return MemoryUsage.SizeInBytes(obj);
        //             }
        //}
        ///// <summary>
        /////此方法将为您提供对象实例大小的近似值（以 bytes1 为单位）。
        /////如果您想知道包含字符串属性的类的大小，则需要（在 32 位系统上）：
        /////8 个字节用于内部数据
        /////4 个字节用于字符串引用
        /////4 字节未使用空间（达到内存管理器可以处理的最小 16 字节）
        /////通常包含整数属性的类需要：
        /////8 个字节用于内部数据 2。
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static unsafe int CalcObjSize(object obj, bool runtimeTypeHandle) {
        //    if(runtimeTypeHandle) {
        //        RuntimeTypeHandle th = obj.GetType().TypeHandle;
        //        int size = *(*(int**)&th + 1);
        //        //结果不正常
        //        return size;
        //    }
        //    return GetObjectSize(obj);
        //}
        //static int GetObjectSize(object obj) {
        //    FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //    int size = 0;
        //    foreach(FieldInfo field in fields) {
        //        size += GetFieldSize(obj, field, field.FieldType);
        //    }
        //    return size;
        //}
        //private static int GetFieldSize(object obj, FieldInfo field, Type type) {
        //    //结果不正常
        //    if(type.IsValueType) {
        //        return Marshal.SizeOf(type);
        //    }
        //    else if(type.IsArray) {
        //        object value = field.GetValue(obj);
        //        //Array arr = (Array)value;
        //        return GetFieldSize(obj, field, type.GetElementType()) * ((Array)value).Length;
        //    }
        //    else {
        //        return GetObjectSize(Activator.CreateInstance(type));
        //    }
        //}
        #endregion
    }
}
