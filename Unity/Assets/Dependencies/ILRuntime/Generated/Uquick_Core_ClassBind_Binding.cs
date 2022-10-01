using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Uquick_Core_ClassBind_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Uquick.Core.ClassBind);
            args = new Type[]{};
            method = type.GetMethod("BindSelf", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindSelf_0);
            args = new Type[]{typeof(Uquick.Core.ClassData)};
            method = type.GetMethod("AddClass", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddClass_1);
            args = new Type[]{typeof(Uquick.Core.ClassData)};
            method = type.GetMethod("Active", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Active_2);

            field = type.GetField("scriptsToBind", flag);
            app.RegisterCLRFieldGetter(field, get_scriptsToBind_0);
            app.RegisterCLRFieldSetter(field, set_scriptsToBind_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_scriptsToBind_0, AssignFromStack_scriptsToBind_0);


        }


        static StackObject* BindSelf_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Uquick.Core.ClassBind instance_of_this_method = (Uquick.Core.ClassBind)typeof(Uquick.Core.ClassBind).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindSelf();

            return __ret;
        }

        static StackObject* AddClass_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Uquick.Core.ClassData @classData = (Uquick.Core.ClassData)typeof(Uquick.Core.ClassData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Uquick.Core.ClassBind instance_of_this_method = (Uquick.Core.ClassBind)typeof(Uquick.Core.ClassBind).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AddClass(@classData);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        static StackObject* Active_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Uquick.Core.ClassData @classData = (Uquick.Core.ClassData)typeof(Uquick.Core.ClassData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Uquick.Core.ClassBind instance_of_this_method = (Uquick.Core.ClassBind)typeof(Uquick.Core.ClassBind).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Active(@classData);

            return __ret;
        }


        static object get_scriptsToBind_0(ref object o)
        {
            return ((Uquick.Core.ClassBind)o).scriptsToBind;
        }

        static StackObject* CopyToStack_scriptsToBind_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Uquick.Core.ClassBind)o).scriptsToBind;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_scriptsToBind_0(ref object o, object v)
        {
            ((Uquick.Core.ClassBind)o).scriptsToBind = (Uquick.Core.ClassData[])v;
        }

        static StackObject* AssignFromStack_scriptsToBind_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Uquick.Core.ClassData[] @scriptsToBind = (Uquick.Core.ClassData[])typeof(Uquick.Core.ClassData[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Uquick.Core.ClassBind)o).scriptsToBind = @scriptsToBind;
            return ptr_of_this_method;
        }



    }
}
