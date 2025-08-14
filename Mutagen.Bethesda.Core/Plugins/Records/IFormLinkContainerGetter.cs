namespace Mutagen.Bethesda.Plugins.Records;

/// <summary>
/// An interface for classes that contain FormKeys and can enumerate them.
/// </summary>
public interface IFormLinkContainer : IFormLinkContainerGetter
{
    /// <summary>
    /// Swaps out all links to point to new FormKeys
    /// </summary>
    void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping);
}

/// <summary>
/// An interface for classes that contain FormKeys and can enumerate them.
/// </summary>
public interface IFormLinkContainerGetter
{
    /// <summary>
    /// Enumeration of all contained FormLinks within object and subobjects
    /// </summary>
    IEnumerable<IFormLinkGetter> EnumerateFormLinks();

    /// <summary>
    /// Enumerate of all contained FormLinks within object and subobjects that point to a specific type of record.
    /// </summary>
    IEnumerable<IFormLinkGetter<TMajorGetter>> EnumerateFormLinks<TMajorGetter>()
        where TMajorGetter : class, IMajorRecordGetter;
}