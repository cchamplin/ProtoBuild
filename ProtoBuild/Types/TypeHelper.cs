using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.Types
{
    public static class TypeHelper
    {
        public static Dictionary<Type, object> _constructorCache;
        static TypeHelper()
        {
            _constructorCache = new Dictionary<Type, object>();
        }
        public delegate object ConstructorDelegate();
        public delegate object ConstructorDelegate2(params object[] args);
        public static ConstructorDelegate GetConstructor(Type type)
        {
            object tDel;
            if (_constructorCache.TryGetValue(type, out tDel))
            {
                return (ConstructorDelegate)tDel;
            }

            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
            if (ci == null)
            {
                ci = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            }
            if (ci != null)
            {
                DynamicMethod method = new System.Reflection.Emit.DynamicMethod("__Ctor", type, Type.EmptyTypes, typeof(TypeHelper).Module, true);
                ILGenerator gen = method.GetILGenerator();
                gen.Emit(System.Reflection.Emit.OpCodes.Newobj, ci);
                gen.Emit(System.Reflection.Emit.OpCodes.Ret);
                tDel = (ConstructorDelegate)method.CreateDelegate(typeof(ConstructorDelegate));
                _constructorCache.Add(type, tDel);
                return (ConstructorDelegate)tDel;
            }
            return () => Activator.CreateInstance(type);
        }
        public static ConstructorDelegate2 GetConstructor(Type type, Type[] arguments)
        {
            object tDel;
            if (_constructorCache.TryGetValue(type, out tDel))
            {
                return (ConstructorDelegate2)tDel;
            }
            ConstructorInfo ci = type.GetConstructor(arguments);
            if (ci == null)
            {
                ci = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, arguments, null);
            }
            if (ci != null)
            {
                
                DynamicMethod method = new System.Reflection.Emit.DynamicMethod("__Ctor", type, new Type[] { typeof(object[]) }, typeof(TypeHelper).Module, true);
                ILGenerator gen = method.GetILGenerator();
                for (int x = 0; x < arguments.Length; x++)
                {
                    gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                    gen.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, x);
                    gen.Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref);
                    Cast(gen, arguments[x], null);
                }
                gen.Emit(System.Reflection.Emit.OpCodes.Newobj, ci);
                gen.Emit(System.Reflection.Emit.OpCodes.Ret);
                tDel = (ConstructorDelegate2)method.CreateDelegate(typeof(ConstructorDelegate2));
                _constructorCache.Add(type, tDel);
                return (ConstructorDelegate2)tDel;
            }
            return null;//() => Activator.CreateInstance(type);
        }
        public static void Cast(ILGenerator il, Type type, LocalBuilder addr)
        {
            if (type == typeof(object)) { }
            else if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
                if (addr != null)
                {
                    il.Emit(OpCodes.Stloc, addr);
                    il.Emit(OpCodes.Ldloca_S, addr);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }
    }
    // Todo dynamic members
    internal class DynanmicInstance : DynamicObject
    {
        public DynanmicInstance()
        {

        }
        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }
    }
    public class PropertyAccessor
    {
        public delegate void PropertySetter(object instance, object value);
        public delegate object PropertyGetter(object instance);
        public Type type;
        public PropertySetter setter;
        public PropertyGetter getter;
        public PropertyAccessor(Type t, PropertyInfo pi)
        {
            this.type = pi.PropertyType;
            DynamicMethod method = new System.Reflection.Emit.DynamicMethod("__setter" + pi.Name, typeof(void), new Type[] { typeof(Object), typeof(Object) }, t, true);

            ILGenerator gen = method.GetILGenerator();
            LocalBuilder loc = t.IsValueType ? gen.DeclareLocal(t) : null;
            gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
            TypeHelper.Cast(gen, t, loc);
            gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
            TypeHelper.Cast(gen, pi.PropertyType, null);
            gen.EmitCall(t.IsValueType ? OpCodes.Call : OpCodes.Callvirt, pi.GetSetMethod(), null);
            gen.Emit(System.Reflection.Emit.OpCodes.Ret);
            setter = (PropertySetter)method.CreateDelegate(typeof(PropertySetter));




            method = new System.Reflection.Emit.DynamicMethod("__getter" + pi.Name, typeof(object), new Type[] { typeof(object) }, t, true);

            gen = method.GetILGenerator();
            loc = t.IsValueType ? gen.DeclareLocal(t) : null;
            gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
            TypeHelper.Cast(gen, t, loc);
            gen.EmitCall(t.IsValueType ? OpCodes.Call : OpCodes.Callvirt, pi.GetGetMethod(), null);
            if (pi.PropertyType.IsValueType)
            {
                gen.Emit(OpCodes.Box, pi.PropertyType);
            }
            //TypeHelper.Cast(gen, typeof(object), null);
            gen.Emit(System.Reflection.Emit.OpCodes.Ret);
            getter = (PropertyGetter)method.CreateDelegate(typeof(PropertyGetter));


        }
        public PropertyAccessor(Type t, FieldInfo fi)
        {
            this.type = fi.FieldType;
            DynamicMethod method = new System.Reflection.Emit.DynamicMethod("__setter" + fi.Name, typeof(void), new Type[] { typeof(Object), typeof(Object) }, t, true);

            ILGenerator gen = method.GetILGenerator();
            LocalBuilder loc = t.IsValueType ? gen.DeclareLocal(t) : null;
            gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
            TypeHelper.Cast(gen, t, loc);
            gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
            TypeHelper.Cast(gen, fi.FieldType, null);
            gen.Emit(System.Reflection.Emit.OpCodes.Stfld, fi);
            gen.Emit(System.Reflection.Emit.OpCodes.Ret);
            setter = (PropertySetter)method.CreateDelegate(typeof(PropertySetter));


            method = new System.Reflection.Emit.DynamicMethod("__getter" + fi.Name, typeof(object), new Type[] { typeof(object) }, t, true);

            gen = method.GetILGenerator();
            loc = t.IsValueType ? gen.DeclareLocal(t) : null;
            //gen.Emit(System.Reflection.Emit.OpCodes.Nop);
            gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
            TypeHelper.Cast(gen, t, loc);
            gen.Emit(OpCodes.Ldfld, fi);
            if (fi.FieldType.IsValueType)
            {
                gen.Emit(OpCodes.Box, fi.FieldType);
            }
            gen.Emit(System.Reflection.Emit.OpCodes.Ret);
            getter = (PropertyGetter)method.CreateDelegate(typeof(PropertyGetter));


        }
    }
}
