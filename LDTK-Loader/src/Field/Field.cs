using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// A variable value on an entity or level. Can represent any of the types in <see cref="FieldType">.
/// </summary>
public struct Field() {
    const int defaultInt = 0;
    const float defaultFloat = 0;
    const bool defaultBool = false;
    static readonly string defaultString = string.Empty;
    static readonly Color defaultColor = Color.White;
    static readonly FieldTile defaultTile = new();
    static readonly FieldEntity defaultEntity = new();
    static readonly Point2 defaultPoint = Point2.Zero;
    static readonly Field[] defaultArray = [];

    /// <summary>
    /// The type of data the field is storing.
    /// </summary>
    public FieldType Type = FieldType.INVALID;

    /// <summary>
    /// The field's name id.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// The name id of the enum the field is using. Empty for any type besides an enum value.
    /// </summary>
    public string EnumName = string.Empty;

    int valInt = defaultInt;
    float valFloat = defaultFloat;
    bool valBool = defaultBool;
    string valString = defaultString;
    Color valColor = defaultColor;
    FieldTile valTile = defaultTile;
    FieldEntity valEntity = defaultEntity;
    Point2 valPoint = defaultPoint;
    Field[] valArray = defaultArray;

    /// <summary>
    /// If the given type matches the type of data the field stores.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>If the field is a given type.</returns>
    public readonly bool Is(FieldType type) => type == Type;

    /// <summary>
    /// Get an int value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing an int.</returns>
    public readonly int AsInt() => MatchOrDefault(valInt, defaultInt, FieldType.INT);

    /// <summary>
    /// Get a float value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a float.</returns>
    public readonly float AsFloat() => MatchOrDefault(valFloat, defaultFloat, FieldType.FLOAT);

    /// <summary>
    /// Get a boolean value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a boolean.</returns>
    public readonly bool AsBool() => MatchOrDefault(valBool, defaultBool, FieldType.BOOL);

    /// <summary>
    /// Get a string value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a string.</returns>
    public readonly string AsString() => MatchOrDefault(valString, defaultString, FieldType.STRING);

    /// <summary>
    /// Get a color value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a color.</returns>
    public readonly Color AsColor() => MatchOrDefault(valColor, defaultColor, FieldType.COLOR);

    /// <summary>
    /// Get an enum value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing an enum.</returns>
    public readonly string AsEnum() => MatchOrDefault(valString, defaultString, FieldType.ENUM);

    /// <summary>
    /// Get an enum value from the field.
    /// </summary>
    /// <typeparam name="T">The enum type to convert the value to.</typeparam>
    /// <returns>The field's value, or a default if it isn't storing an enum.</returns>
    public readonly T AsEnum<T>() where T : struct {
        string enumValue = AsEnum();
        if (!Enum.TryParse(enumValue, out T cast)) {
            Log.Warning($"LDTKLoader: Cannot cast field value {enumValue} to enum type {typeof(T)},returning default.");
            return default;
        }
        return cast;
    }

    /// <summary>
    /// Get a file path value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a file path.</returns>
    public readonly string AsFilePath() => MatchOrDefault(valString, defaultString, FieldType.FILE_PATH);

    /// <summary>
    /// Get a tile value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a tile.</returns>
    public readonly FieldTile AsTile() => MatchOrDefault(valTile, defaultTile, FieldType.TILE);

    /// <summary>
    /// Get an entity value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing an entity.</returns>
    public readonly FieldEntity AsEntity() => MatchOrDefault(valEntity, defaultEntity, FieldType.ENTITY);

    /// <summary>
    /// Get a point2 value from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a point2.</returns>
    public readonly Point2 AsPoint() => MatchOrDefault(valPoint, defaultPoint, FieldType.POINT);

    /// <summary>
    /// Get an array of int values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing an int array.</returns>
    public readonly int[] AsIntArray() => [.. AsArray().Select(e => e.AsInt())];

    /// <summary>
    /// Get an array of float values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a float array.</returns>
    public readonly float[] AsFloatArray() => [.. AsArray().Select(e => e.AsFloat())];

    /// <summary>
    /// Get an array of boolean values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a boolean array.</returns>
    public readonly bool[] AsBoolArray() => [.. AsArray().Select(e => e.AsBool())];

    /// <summary>
    /// Get an array of string values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a string array.</returns>
    public readonly string[] AsStringArray() => [.. AsArray().Select(e => e.AsString())];

    /// <summary>
    /// Get an array of color values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a color array.</returns>
    public readonly Color[] AsColorArray() => [.. AsArray().Select(e => e.AsColor())];

    /// <summary>
    /// Get an array of enum values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing an enum array.</returns>
    public readonly string[] AsEnumArray() => [.. AsArray().Select(e => e.AsEnum())];

    /// <summary>
    /// Get an array of enum values from the field.
    /// </summary>
    /// <typeparam name="T">The enum type to convert the value to.</typeparam>
    /// <returns>The field's value, or a default if it isn't storing an enum array.</returns>
    public readonly T[] AsEnumArray<T>() where T : struct => [.. AsArray().Select(e => e.AsEnum<T>())];

    /// <summary>
    /// Get an array of file path values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a file path array.</returns>
    public readonly string[] AsFilePathArray() => [.. AsArray().Select(e => e.AsFilePath())];

    /// <summary>
    /// Get an array of tile values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a tile array.</returns>
    public readonly FieldTile[] AsTileArray() => [.. AsArray().Select(e => e.AsTile())];

    /// <summary>
    /// Get an array of entity values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing an entity array.</returns>
    public readonly FieldEntity[] AsEntityArray() => [.. AsArray().Select(e => e.AsEntity())];

    /// <summary>
    /// Get an array of point2 values from the field.
    /// </summary>
    /// <returns>The field's value, or a default if it isn't storing a point2 array.</returns>
    public readonly Point2[] AsPointArray() => [.. AsArray().Select(e => e.AsPoint())];

    /// <summary>
    /// Get an array fields from the field.
    /// </summary>
    /// <returns>The field's array, or a default if it isn't array.</returns>
    readonly Field[] AsArray() => MatchOrDefault(valArray, defaultArray, FieldType.ARRAY);

    internal static Field FromInt(string name, int value) => new() {
        Type = FieldType.INT,
        Name = name,
        valInt = value
    };

    internal static Field FromFloat(string name, float value) => new() {
        Type = FieldType.FLOAT,
        Name = name,
        valFloat = value
    };

    internal static Field FromBool(string name, bool value) => new() {
        Type = FieldType.BOOL,
        Name = name,
        valBool = value
    };

    internal static Field FromString(string name, string value) => new() {
        Type = FieldType.STRING,
        Name = name,
        valString = value
    };

    internal static Field FromColor(string name, Color value) => new() {
        Type = FieldType.COLOR,
        Name = name,
        valColor = value
    };

    internal static Field FromEnum(string name, string value, string enumName) => new() {
        Type = FieldType.ENUM,
        Name = name,
        valString = value,
        EnumName = enumName
    };

    internal static Field FromFilePath(string name, string value) => new() {
        Type = FieldType.FILE_PATH,
        Name = name,
        valString = value
    };

    internal static Field FromTile(string name, FieldTile value) => new() {
        Type = FieldType.TILE,
        Name = name,
        valTile = value
    };

    internal static Field FromEntity(string name, FieldEntity value) => new() {
        Type = FieldType.ENTITY,
        Name = name,
        valEntity = value
    };

    internal static Field FromPoint(string name, Point2 value) => new() {
        Type = FieldType.POINT,
        Name = name,
        valPoint = value
    };

    internal static Field FromArray(string name, Field[] value) => new() {
        Type = FieldType.ARRAY,
        Name = name,
        valArray = value
    };

    readonly T MatchOrDefault<T>(T value, T fallback, FieldType type) {
        if (!Is(type)) {
            Log.Warning($"LDTKLoader: Field {Name} is type {Type}, doesn't match requested type of {type}. Returning fallback.");
            return fallback;
        }
        return value;
    }

    public override readonly string ToString() => $"{Name} {Type} {Type switch {
        FieldType.INT => valInt,
        FieldType.FLOAT => valFloat,
        FieldType.BOOL => valBool,
        FieldType.STRING => valString,
        FieldType.COLOR => valColor,
        FieldType.ENUM => valString,
        FieldType.FILE_PATH => valString,
        FieldType.TILE => valTile,
        FieldType.ENTITY => valEntity,
        FieldType.POINT => valPoint,
        FieldType.ARRAY => string.Join(", ", AsArray()),
        _ => "EMPTY"
    }}";
}