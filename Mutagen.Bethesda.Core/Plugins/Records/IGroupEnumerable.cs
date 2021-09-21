using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>  
    /// An interface for classes that contain Groups and can enumerate them.  
    /// </summary>  
    public interface IGroupEnumerable : IGroupGetterEnumerable
    {
        /// <summary>  
        /// Enumerates all contained Groups of the specified generic type  
        /// </summary>  
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <typeparam name="TMajor">Type of major record to enumerate</typeparam>
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>  
        /// <returns>Enumerable of all contained Groups</returns>  
        new IEnumerable<IGroupCommon<TMajor>> EnumerateGroups<TMajor>(bool throwIfUnknown = true)
            where TMajor : class, IMajorRecordCommon;
    }

    /// <summary>  
    /// An interface for classes that contain Major Record Getter interfaces and can enumerate them  
    /// </summary>  
    public interface IGroupGetterEnumerable
    {
        /// <summary>  
        /// Enumerates all contained Major Record Getters  
        /// </summary>  
        /// <returns>Enumerable of all contained Major Record Getters</returns>  
        IEnumerable<IGroupCommonGetter> EnumerateGroups();

        /// <summary>  
        /// Enumerates all contained Major Record Getters of the specified generic type  
        /// </summary>
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>  
        /// <returns>Enumerable of all applicable major records</returns>  
        IEnumerable<IGroupCommonGetter<T>> EnumerateGroups<T>(bool throwIfUnknown = true)
            where T : class, IMajorRecordCommonGetter;

        /// <summary>  
        /// Enumerates all contained Major Record Getters of the specified type  
        /// </summary>  
        /// <param name="type">Type to query and iterate</param> 
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>  
        /// <returns>Enumerable of all applicable major records</returns>  
        IEnumerable<IGroupCommonGetter> EnumerateGroups(Type type, bool throwIfUnknown = true);
    }
}
