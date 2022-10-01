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
    unsafe class Uquick_Core_ClassData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Uquick.Core.ClassData);

            field = type.GetField("classNamespace", flag);
            app.RegisterCLRFieldGetter(field, get_classNamespace_0);
            app.RegisterCLRFieldSetter(field, set_classNamespace_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_classNamespace_0, AssignFromStack_classNamespace_0);
            field = type.GetField("className", flag);
            app.RegisterCLRFieldGetter(field, get_className_1);
            app.RegisterCLRFieldSetter(field, set_className_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_className_1, AssignFromStack_className_1);
            field = type.GetField("activeAfter", flag);
            app.RegisterCLRFieldGetter(field, get_activeAfter_2);
            app.RegisterCLRFieldSetter(field, set_activeAfter_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_activeAfter_2, AssignFromStack_activeAfter_2);

            app.RegisterCLRCreateArrayInstance(type, s => new Uquick.Core.ClassData[s]);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_classNamespace_0(ref object o)
        {
            return ((Uquick.Core.ClassData)o).classNamespace;
        }

        static StackObject* CopyToStack_classNamespace_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Uquick.Core.ClassData)o).classNamespace;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_classNamespace_0(ref object o, object v)
        {
            ((Uquick.Core.ClassData)o).classNamespace = (System.String)v;
        }

        static StackObject* AssignFromStack_classNamespace_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @classNamespace = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Uquick.Core.ClassData)o).classNamespace = @classNamespace;
            return ptr_of_this_method;
        }

        static object get_className_1(ref object o)
        {
            return ((Uquick.Core.ClassData)o).className;
        }

        static StackObject* CopyToStack_className_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Uquick.Core.ClassData)o).className;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_className_1(ref object o, object v)
        {
            ((Uquick.Core.ClassData)o).className = (System.String)v;
        }

        static StackObject* AssignFromStack_className_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @className = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Uquick.Core.ClassData)o).className = @className;
            return ptr_of_this_method;
        }

        static object get_activeAfter_2(ref object o)
        {
            return ((Uquick.Core.ClassData)o).activeAfter;
        }

        static StackObject* CopyToStack_activeAfter_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Uquick.Core.ClassData)o).activeAfter;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_activeAfter_2(ref object o, object v)
        {
            ((Uquick.Core.ClassData)o).activeAfter = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_activeAfter_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @activeAfter = ptr_of_this_method->Value == 1;
            ((Uquick.Core.ClassData)o).activeAfter = @activeAfter;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new Uquick.Core.ClassData();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
