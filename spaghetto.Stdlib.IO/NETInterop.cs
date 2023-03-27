﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace spaghetto.Stdlib.IO
{
    public class NETInterop
    {
        public static SClass CreateClass()
        {
            var @class = new SClass("NETInterop");

            @class.StaticTable.Add((new SString("getNativeValue"), new SNativeFunction(
               impl: (Scope scope, List<SValue> args) =>
               {
                   switch (args[0].BuiltinName) {
                       case SBuiltinType.Int:
                           return new SNativeObject((args[0] as SInt).Value);
                       case SBuiltinType.Float:
                           return new SNativeObject((args[0] as SFloat).Value);
                       case SBuiltinType.String:
                           return new SNativeObject((args[0] as SString).Value);
                   }

                   return new SNativeObject(null);
               },
               expectedArgs: new() { "val" }
            )));

            @class.StaticTable.Add((new SString("getType"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SString typeName) throw new Exception("Arg 0 must be string");

                    return new SNativeObject(GetType(typeName.Value));
                },
                expectedArgs: new() { "name" }
            )));

            @class.StaticTable.Add((new SString("getMethod"), new SNativeFunction(
                impl: (Scope scope, List<SValue> args) =>
                {
                    if (args[0] is not SNativeObject type) throw new Exception("Arg 0 must be native object");
                    if (args[1] is not SString methodName) throw new Exception("Arg 1 must be string");
                    if (args[2] is not SList argList) throw new Exception("Arg 2 must be list");

                    List<Type> argTypes = new();

                    foreach (var val in argList.Value) {
                        if (val is not SNativeObject argNativeObject || argNativeObject.Value is not Type argType) throw new Exception("Arg 1 entries must be native objects of type Type");
                        argTypes.Add(argType);
                    }

                    if (type.Value is not Type t) throw new Exception("Arg 0 native object type must be Type");

                    return new SNativeObject(t.GetMethod(methodName.Value, argTypes.ToArray()));
                },
                expectedArgs: new() { "type", "name", "argTypes" }
            )));

            @class.StaticTable.Add((new SString("invokeStatic"), new SNativeFunction(
               impl: (Scope scope, List<SValue> args) =>
               {
                   if (args[0] is not SNativeObject method) throw new Exception("Arg 0 must be native object");
                   if (args[1] is not SList argList) throw new Exception("Arg 1 must be list");

                   List<object> invocationArgs = new();

                   foreach(var val in argList.Value) {
                       if (val is not SNativeObject argNativeObject) throw new Exception("Arg 1 entries must be native objects");
                       invocationArgs.Add(argNativeObject.Value);
                   }

                   if (method.Value is not MethodInfo t) throw new Exception("Arg 0 native object type must be Method");

                   return new SNativeObject(t.Invoke(null, invocationArgs.ToArray()));
               },
               expectedArgs: new() { "type", "list of args" }
           )));

            return @class;
        }
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}