﻿#if INIT_JE
using System;
using Malee.List;
using System.Linq;
using UnityEngine;
using System.Reflection;
using ILRuntime.CLR.Utils;
using ILRuntime.Reflection;
using ILRuntime.CLR.TypeSystem;
using UnityEngine.Serialization;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;


namespace Uquick.Core
{
    [HelpURL("https://xgamedev.uoyou.com/classbind-v0-6.html")]
    public class ClassBind : MonoBehaviour
    {
        [FormerlySerializedAs("ScriptsToBind")]
        public ClassData[] scriptsToBind = new ClassData[1];

        /// <summary>
        /// Bind itself, call it after when instantiating a prefab with ClassBind in main solution
        /// 激活ClassBind，在主工程Instantiate带有ClassBind的prefab后调用
        /// </summary>
        public void BindSelf()
        {
            ClassBindMgr.DoBind(this);
        }

        /// <summary>
        /// Add class
        /// </summary>
        /// <param name="classData"></param>
        /// <returns></returns>
        public object AddClass(ClassData classData)
        {
            //添加脚本
            string classType =
                $"{classData.classNamespace + (string.IsNullOrEmpty(classData.classNamespace) ? "" : ".")}{classData.className}";
            if (!InitUquick.Appdomain.LoadedTypes.TryGetValue(classType, out var type))
            {
                Log.PrintError($"自动绑定{name}出错：{classType}不存在，已跳过");
                return null;
            }

            Type t = type.ReflectionType; //获取实际属性
            classData.ClassType = t;
            Type baseType =
                t.BaseType is ILRuntimeWrapperType wrapperType
                    ? wrapperType.RealType
                    : t.BaseType; //这个地方太坑了 你一旦热更工程代码写的骚 就会导致ILWrapperType这个问题出现 一般人还真不容易发现这个坑
            Type monoType = typeof(MonoBehaviour);

            //JBehaviour需自动赋值一个值
            bool isMono = t.IsSubclassOf(monoType) || (baseType != null && baseType.IsSubclassOf(monoType));
            bool needAdapter = baseType != null &&
                               baseType.GetInterfaces().Contains(typeof(CrossBindingAdaptorType));

            ILTypeInstance instance = isMono
                ? new ILTypeInstance(type as ILType, false)
                : InitUquick.Appdomain.Instantiate(classType);

            instance.CLRInstance = instance;

            /*
             * 这里是ClassBind的灵魂，我都佩服我自己这么写，所以别乱改这块
             * 非mono的跨域继承用特殊的，就是用Uquick提供的一个mono脚本，来显示字段，里面存ILTypeInstance
             * 总之Uquick牛逼
             * ClassBind只支持挂以下2种热更类型：纯热更类型，继承了Mono的类型（无论是主工程多重继承后跨域还是跨域后热更工程多重继承都可以）
             * 主工程多重继承后再跨域多重继承的应该还不支持
             */
            //主工程多重继承后跨域继承的生成适配器后用这个
            if (needAdapter && isMono && baseType != typeof(MonoBehaviourAdapter.Adaptor))
            {
                Type adapterType = Type.GetType(baseType.FullName ?? string.Empty);
                if (adapterType == null)
                {
                    Log.PrintError($"{t.FullName}, need to generate adapter");
                    return null;
                }

                //直接反射赋值一波了
                var clrInstance = gameObject.AddComponent(adapterType) as MonoBehaviour;

                var clrILInstance = t.GetFields(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .First(f => f.Name == "instance" && f.FieldType == typeof(ILTypeInstance));
                var clrAppDomain = t.GetFields(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .First(f => f.Name == "appdomain" && f.FieldType == typeof(AppDomain));
                if (!(clrInstance is null))
                {
                    clrInstance.enabled = false;
                    clrILInstance.SetValue(clrInstance, instance);
                    clrAppDomain.SetValue(clrInstance, InitUquick.Appdomain);
                    instance.CLRInstance = clrInstance;
                    classData.ClrInstance = (CrossBindingAdaptorType)clrInstance;
                }
            }
            //直接继承Mono的，热更工程多层继承mono的，非继承mono的，或不需要继承的，用这个
            else
            {
                //挂个适配器到编辑器（直接继承mono，非继承mono，无需继承，都可以用这个）
                var clrInstance = gameObject.AddComponent<MonoBehaviourAdapter.Adaptor>();
                clrInstance.enabled = false;
                clrInstance.ILInstance = instance;
                clrInstance.AppDomain = InitUquick.Appdomain;
                classData.ClrInstance = clrInstance;


                //是MonoBehaviour继承，需要指定CLRInstance
                if (isMono)
                {
                    instance.CLRInstance = clrInstance;
                }

                //判断类型
                clrInstance.isMonoBehaviour = isMono;

                classData.Added = true;

                //JBehaviour额外处理
                var go = t.GetField("_gameObject", BindingFlags.Public);
                go?.SetValue(clrInstance.ILInstance, gameObject);
            }

            if (isMono)
            {
                var m = type.GetConstructor(Extensions.EmptyParamList);
                if (m != null)
                {
                    InitUquick.Appdomain.Invoke(m, instance, null);
                }
            }

            return instance;
        }


        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="classData"></param>
        public void SetVal(ClassData classData)
        {
            string classType =
                $"{classData.classNamespace + (classData.classNamespace == "" ? "" : ".")}{classData.className}";
            Type t = classData.ClassType; //获取实际属性
            var clrInstance = classData.ClrInstance;
            //绑定数据
            classData.BoundData = false;
            var fields = classData.fields.ToArray();
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                              BindingFlags.Static | BindingFlags.FlattenHierarchy;

            foreach (ClassField field in fields)
            {
                if (field.fieldType == ClassField.FieldType.NotSupported) continue;

                object obj = null;
                try
                {
                    if (field.fieldType == ClassField.FieldType.Number)
                    {
                        var fieldType = t.GetField(field.fieldName, bindingAttr)?.FieldType ??
                                        (t.BaseType?.GetField(field.fieldName, bindingAttr)?.FieldType ??
                                         (t.GetProperty(field.fieldName, bindingAttr)?.PropertyType ??
                                          t.BaseType?.GetProperty(field.fieldName, bindingAttr)?.PropertyType));
                        fieldType = fieldType is ILRuntimeWrapperType wrapperType ? wrapperType.RealType : fieldType;

                        if (fieldType == typeof(SByte))
                        {
                            obj = SByte.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Byte))
                        {
                            obj = Byte.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Int16))
                        {
                            obj = Int16.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(UInt16))
                        {
                            obj = UInt16.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Int32))
                        {
                            obj = Int32.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(UInt32))
                        {
                            obj = UInt32.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Int64))
                        {
                            obj = Int64.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(UInt64))
                        {
                            obj = UInt64.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Single))
                        {
                            obj = Single.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Decimal))
                        {
                            obj = Decimal.Parse(field.value);
                            classData.BoundData = true;
                        }
                        else if (fieldType == typeof(Double))
                        {
                            obj = Double.Parse(field.value);
                            classData.BoundData = true;
                        }
                    }

                    else if (field.fieldType == ClassField.FieldType.String)
                    {
                        obj = field.value;
                        classData.BoundData = true;
                    }
                    else if (field.fieldType == ClassField.FieldType.Bool)
                    {
                        field.value = field.value.ToLower();
                        obj = field.value == "true";
                        classData.BoundData = true;
                    }

                    if (field.fieldType == ClassField.FieldType.GameObject)
                    {
                        GameObject go = field.gameObject;
                        if (go == null)
                        {
                            try
                            {
                                go = field.value == "${this}"
                                    ? gameObject
                                    : GameObject.Find(field.value);
                                if (go == null) //找父物体
                                {
                                    go = FindSubGameObject(field);
                                    if (go == null) //如果父物体还不存在
                                    {
                                        continue;
                                    }
                                }
                            }
                            catch (Exception) //找父物体（如果抛出空异常）
                            {
                                go = FindSubGameObject(field);
                                if (go == null) //如果父物体还不存在
                                {
                                    continue;
                                }
                            }
                        }

                        obj = go;
                        classData.BoundData = true;
                    }
                    else if (field.fieldType == ClassField.FieldType.UnityComponent)
                    {
                        GameObject go = field.gameObject;
                        if (go == null)
                        {
                            try
                            {
                                if (field.value.Contains("."))
                                {
                                    field.value =
                                        field.value.Remove(field.value.IndexOf(".", StringComparison.Ordinal));
                                }

                                go = field.value == "${this}"
                                    ? gameObject
                                    : GameObject.Find(field.value);
                                if (go == null) //找父物体
                                {
                                    go = FindSubGameObject(field);
                                    if (go == null) //如果父物体还不存在
                                    {
                                        continue;
                                    }
                                }
                            }
                            catch (Exception) //找父物体（如果抛出空异常）
                            {
                                go = FindSubGameObject(field);
                                if (go == null) //如果父物体还不存在
                                {
                                    continue;
                                }
                            }
                        }

                        void SetField(Type fieldType)
                        {
                            fieldType = fieldType is ILRuntimeWrapperType wrapperType
                                ? wrapperType.RealType
                                : fieldType;

                            if (fieldType is ILRuntimeType ilType) //如果在热更中
                            {
                                var components = go.GetComponents<CrossBindingAdaptorType>();
                                foreach (var c in components)
                                {
                                    if (c.ILInstance.Type.CanAssignTo(ilType.ILType))
                                    {
                                        obj = c.ILInstance;
                                        classData.BoundData = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                var component = go.GetComponent(fieldType);
                                if (component != null)
                                {
                                    obj = component;
                                    classData.BoundData = true;
                                }
                            }
                        }

                        var tp = t.GetField(field.fieldName, bindingAttr);
                        if (tp == null) tp = t.BaseType?.GetField(field.fieldName, bindingAttr);
                        if (tp != null)
                        {
                            SetField(tp.FieldType);
                        }
                        else
                        {
                            var pi = t.GetProperty(field.fieldName, bindingAttr);
                            if (pi == null) pi = t.BaseType?.GetProperty(field.fieldName, bindingAttr);
                            if (pi != null)
                            {
                                SetField(pi.PropertyType);
                            }
                            else
                            {
                                Log.PrintError(
                                    $"自动绑定{name}出错：{classType}.{field.fieldName}赋值出错：{field.fieldName}不存在");
                            }
                        }
                    }
                    else if (field.fieldType == ClassField.FieldType.HotUpdateResource)
                    {
                        obj = AssetMgr.Load(field.value);
                        classData.BoundData = true;
                    }
                }
                catch (Exception except)
                {
                    Log.PrintError(
                        $"自动绑定{name}出错：{classType}.{field.fieldName}获取值{field.value}出错：{except.Message}，已跳过,{except.StackTrace}");
                }

                //如果有数据再绑定
                if (classData.BoundData)
                {
                    void BindVal(MemberInfo mi)
                    {
                        try
                        {
                            switch (mi)
                            {
                                case null:
                                    throw new NullReferenceException();
                                case FieldInfo info:
                                    info.SetValue(clrInstance.ILInstance, obj);
                                    break;
                                case PropertyInfo inf:
                                    inf.SetValue(clrInstance.ILInstance, obj);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.PrintError(
                                $"自动绑定{name}出错：{classType}.{field.fieldName}赋值出错：{e.Message}，已跳过");
                        }
                    }

                    var fi = t.GetField(field.fieldName, bindingAttr);
                    if (fi == null) fi = t.BaseType?.GetField(field.fieldName, bindingAttr);
                    if (fi != null)
                    {
                        BindVal(fi);
                    }
                    else
                    {
                        var pi = t.GetProperty(field.fieldName, bindingAttr);
                        if (pi == null) pi = t.BaseType?.GetProperty(field.fieldName, bindingAttr);
                        BindVal(pi);
                    }
                }
            }
        }

        /// <summary>
        /// Active
        /// </summary>
        /// <param name="classData"></param>
        public void Active(ClassData classData)
        {
            string classType =
                $"{classData.classNamespace + (classData.classNamespace == "" ? "" : ".")}{classData.className}";
            Type t = classData.ClassType; //获取实际属性
            var clrInstance = classData.ClrInstance;
            //是否激活
            if (classData.activeAfter)
            {
                if (classData.BoundData == false && classData.fields != null &&
                    classData.fields.Count > 0)
                {
                    Log.PrintError($"自动绑定{name}出错：{classType}没有成功绑定数据，自动激活成功，但可能会抛出空异常！");
                }

                //Mono类型能设置enabled
                if (clrInstance.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    ((MonoBehaviour)clrInstance).enabled = true;
                }

                //不管是啥类型，直接invoke这个awake方法
                var flags = BindingFlags.Default | BindingFlags.Public
                                                | BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                                BindingFlags.NonPublic | BindingFlags.Static;
                var awakeMethod = clrInstance.GetType().GetMethod("Awake",flags);
                if (awakeMethod == null)
                {
                    awakeMethod = t.GetMethod("Awake", flags);
                }
                else
                {
                    awakeMethod.Invoke(clrInstance, null);
                    classData.Activated = true;
                }

                if (awakeMethod == null)
                {
                    Log.PrintError($"{t.FullName}不包含Awake方法，无法激活，已跳过");
                }
                else if (!classData.Activated)
                {
                    awakeMethod.Invoke(t, null);
                }

                classData.Activated = true;
            }

            Remove();
        }

        /// <summary>
        /// Remove cb
        /// </summary>
        private void Remove()
        {
            //添加后删除
            Destroy(this);
        }


        private GameObject FindSubGameObject(ClassField field)
        {
            if (field.value.Contains("/")) //如果有父级
            {
                try
                {
                    var parent =
                        GameObject.Find(field.value.Substring(0,
                            field.value.IndexOf('/'))); //寻找父物体
                    var go = parent.transform
                        .Find(field.value.Substring(field.value.IndexOf('/') + 1))
                        .gameObject;
                    return go;
                }
                catch
                {
                    Log.PrintError($"自动绑定{name}出错：{field.value}对象被隐藏或不存在，无法获取，已跳过");
                }
            }
            else
            {
                Log.PrintError($"自动绑定{name}出错：{field.value}对象被隐藏或不存在，无法获取，已跳过");
            }

            return null;
        }
    }


    [Serializable]
    public class ClassData
    {
        [FormerlySerializedAs("Namespace")] public string classNamespace = "HotUpdateScripts";
        [FormerlySerializedAs("Class")] public string className = "";
        [FormerlySerializedAs("ActiveAfter")] public bool activeAfter = true;

        [FormerlySerializedAs("Fields")] [Reorderable(elementNameProperty = "fieldName")]
        public FieldList fields = new FieldList();

        public bool BoundData { get; set; }
        public bool Added { get; set; }
        public bool Activated { get; set; }

        public CrossBindingAdaptorType ClrInstance { get; set; }
        public Type ClassType { get; set; }
    }

    [Serializable]
    public class ClassField
    {
        public enum FieldType
        {
            Number,
            String,
            Bool,
            GameObject,
            UnityComponent,
            HotUpdateResource,
            NotSupported
        }

        public FieldType fieldType;

        [Tooltip("需要赋值的键的名字")] public string fieldName;

        [Header("For Normal Value and Hot Update Resource Field Type")]
        [Tooltip("非GameObject和UnityComponent的fieldType的值")]
        public string value;

        [Header("For GameObject or Unity Component Field Type")]
        [Tooltip("如果fieldType是GameObject或UnityComponent，此处可填，否则无效")]
        public GameObject gameObject;
    }

    [Serializable]
    public class FieldList : ReorderableArray<ClassField>
    {
    }

    /// <summary>
    /// Ignore the following field/property while matching fields in the editor
    /// 在编辑器下进行自动匹配时忽略该字段/属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ClassBindIgnoreAttribute : Attribute
    {
    }
}
#endif