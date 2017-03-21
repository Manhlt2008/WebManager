using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace DataAccess
{
    public class DynamicBuilder<T>
    {
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private delegate T Load(IDataRecord dataRecord);
        private Load handler;

        private DynamicBuilder() { }

        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        public static DynamicBuilder<T> CreateBuilder(IDataRecord dataRecord)
        {
            DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();

            DynamicMethod method = new DynamicMethod("DynamicCreate", typeof(T), new Type[] { typeof(IDataRecord) }, typeof(T), true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder result = generator.DeclareLocal(typeof(T));
            if (IsSingleType(typeof(T)))
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, 0);
                generator.Emit(OpCodes.Callvirt, getValueMethod);
                generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(0));

                generator.Emit(OpCodes.Ret);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                for (int i = 0; i < dataRecord.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(dataRecord.GetName(i));
                    Label endIfLabel = generator.DefineLabel();

                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);

                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

                        generator.MarkLabel(endIfLabel);
                    }
                }

                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
            }

            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }

        public static bool IsSingleType(Type type)
        {
            return type.IsValueType || type.Name == "String";
        }
    }
}
