
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The insert data column.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <returns> The objects. </returns>
    public delegate object InsertColumnData(string path);
}