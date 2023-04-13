﻿using System.Reflection;

namespace spaghetto.Stdlib.Interop
{
    public class Form
    {
        public static SClass CreateClass()
        {
            var @class = new SClass("Form");

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
