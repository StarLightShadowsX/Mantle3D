using UnityEditor;

public static class MiscHelperMethods_Editor
{
    public static SerializedProperty FindProperty(this SerializedProperty prop, string propertyName, bool backingField = false, string nestedPath = null)
    {
        string path =
            (string.IsNullOrEmpty(nestedPath) ? "" : nestedPath.EndsWith(".") ? nestedPath : nestedPath + ".")
            + (backingField ? "<" : "")
            + propertyName
            + (backingField ? ">k__BackingField" : "");

        return prop.FindPropertyRelative(path);
    }
    public static SerializedProperty FindProperty(this SerializedObject obj, string propertyName, bool backingField = false, string nestedPath = null)
    {
        string path =
            (string.IsNullOrEmpty(nestedPath) ? "" : nestedPath.EndsWith(".") ? nestedPath : nestedPath + ".")
            + (backingField ? "<" : "")
            + propertyName
            + (backingField ? ">k__BackingField" : "");

        return obj.FindProperty(path);
    }

    /// <summary>
    /// Adds the surrounding <>k__BackingField to a property name, to reference the backing field of an auto-property.
    /// </summary>
    /// <param name="propertyName">the input property name. Generally advised to use a "nameof()"</param>
    /// <returns>the identifier of the backing field for use in a FindProperty method.</returns>
    public static string BackingField(this string propertyName) => $"<{propertyName}>k__BackingField";

}