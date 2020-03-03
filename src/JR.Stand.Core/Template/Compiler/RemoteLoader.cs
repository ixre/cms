using System;
using System.Reflection;

namespace JR.DevFw.Template.Compiler
{
    /// <summary>
    /// Factory class to create objects exposing IRemoteInterface
    /// </summary>
    internal class RemoteLoader : MarshalByRefObject
    {
        private Type _Type;

        public RemoteLoader()
        {
        }

        /// <summary> Factory method to create an instance of the type whose name is specified,
        /// using the named assembly file and the constructor that best matches the specified parameters. </summary>
        /// <param name="assemblyFile"> The name of a file that contains an assembly where the type named typeName is sought. </param>
        /// <param name="typeName"> The name of the preferred type. </param>
        /// <param name="constructArgs"> An array of arguments that match in number, order, and type the parameters of the constructor to invoke, or null for default constructor. </param>
        /// <returns> The return value is the created object represented as ILiveInterface. </returns>
        public object Create(string assemblyFile, string typeName, object[] constructArgs)
        {
            if (_Type == null)
            {
                string asmName = System.IO.Path.GetFileNameWithoutExtension(assemblyFile);

                AppDomain.CurrentDomain.Load(asmName);

                Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                Assembly asm = null;

                foreach (Assembly a in asms)
                {
                    if (System.IO.Path.GetFileName(a.Location)
                        .Equals(assemblyFile, StringComparison.CurrentCultureIgnoreCase))
                    {
                        asm = a;
                    }
                }

                Type[] types = asm.GetTypes();

                Type matchType = null;

                foreach (Type type in types)
                {
                    if (type.FullName == typeName)
                    {
                        matchType = type;
                        break;
                    }

                    if (type.BaseType != null)
                    {
                        if (type.BaseType.FullName == typeName)
                        {
                            matchType = type;
                            break;
                        }
                    }

                    if (type.GetInterface(typeName) != null)
                    {
                        matchType = type;
                        break;
                    }
                }

                if (matchType == null)
                {
                    return null;
                }

                _Type = matchType;
            }

            return Activator.CreateInstance(_Type);
        }
    }
}