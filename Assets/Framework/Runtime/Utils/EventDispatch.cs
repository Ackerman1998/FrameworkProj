using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


    public class EventDispatch
    {
        static Dictionary<string, Node> eventDict = new Dictionary<string, Node>();
        public static void AddEventListener(string typeName, string methodName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                object obj = Activator.CreateInstance(type);
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    if (method.Name.Equals(methodName) && !eventDict.ContainsKey(methodName))
                    {
                        Node node = new Node(obj, method);
                        eventDict.Add(methodName, node);
                        Debug.Log("Method " + typeName + "." + methodName + " register success!");
                    }
                }
            }
            else
            {
                    Debug.Log("Type : " + typeName + " is not exist!");
            }
        }

        public static void DispatchEvent(string methodName, params object[] objs)
        {
            if (eventDict.ContainsKey(methodName))
            {
                eventDict[methodName].Run(objs);
            }
            else
            {
                Debug.Log("Method : " + methodName + " is not exist!");
            }
        }
        public static void ClearDict()
        {
            eventDict.Clear();
        }
    }
    class Node
    {
        public object parent;
        public MethodInfo method;
        public Node(object obj, MethodInfo mtd)
        {
            this.parent = obj;
            this.method = mtd;
        }
        public void Run(params object[] objs)
        {
            this.method.Invoke(parent, objs);
        }
    }

