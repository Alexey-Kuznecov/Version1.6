

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace AIconBrowser.Services
{
    /// <summary>
    /// The serialization.
    /// </summary>
    /// [DebuggerStepThrough]
    public class Serialization
    {
        /// <summary>
        /// Binary serialization of these objects.
        /// Note: If an <c>object</c> inherits other objects, they must also be marked with the [<c>Serializable</c>] attribute.
        /// </summary>
        /// <param name="graph">Any <c>object</c> that is marked as serialize.</param>
        /// <param name="stream">Filename to serialize <c>object</c>.</param>
        public static void BinSerialize(object graph, string stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(stream, FileMode.OpenOrCreate))
            {
                bf.Serialize(fs, graph);
            }
        }

        /// <summary>
        /// Binary <c>deserialization</c> of <c>object</c> data from a file.
        /// </summary>
        /// <param name="graph">Returns the <c>object</c> that will need to be cast to the <c>object</c> that was serialized.</param>
        /// <param name="stream">Filename to deserialize <c>object</c>.</param>
        public static void BinDeserialize(out object graph, string stream)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream fs = new FileStream(stream, FileMode.Open))
            {
                AddAssamblyPlugin();
                graph = bf.Deserialize(fs);
            }
        }

        /// <summary>
        /// The add assambly plugin.
        /// </summary>
        private static void AddAssamblyPlugin()
        {
            XElement root = XElement.Load("../../Plugins/plugin.xml");
            var plugins = from element in root.Elements() select element;

            foreach (var item in plugins)
            {
                // AppDomainSetup setup = new AppDomainSetup();
                // setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\" + item.Attribute("Name").Value;
                Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Plugins\\" + item.Attribute("Assambly").Value);
                AppDomain.CurrentDomain.AppendPrivatePath(AppDomain.CurrentDomain.BaseDirectory + "Plugins\\" + item.Attribute("Name").Value);
                AppDomain.CurrentDomain.Load(asm.FullName);
            }
        }

        /// <summary>
        /// The my binder.
        /// </summary>
        internal sealed class MyBinder : SerializationBinder
        {
            /// <summary> The bind to type. </summary>
            /// <param name="assemblyName"> The assembly name. </param>
            /// <param name="typeName"> The type name. </param>
            /// <returns> The <see cref="Type"/>. </returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type ttd = null;
                try
                {
                    string toassname = assemblyName.Split(',')[0];
                    Assembly[] asmblies = AppDomain.CurrentDomain.GetAssemblies();

                    foreach (Assembly ass in asmblies)
                    {
                        if (ass.FullName.Split(',')[0] == toassname)
                        {
                            ttd = ass.GetType(typeName);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                return ttd;
            }
        }
    }
}
