using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

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

    public FieldType Type = FieldType.INVALID;
    public string Name = string.Empty;
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

    public readonly bool Is(FieldType type) => type == Type;

    public readonly int AsInt() => MatchOrDefault(valInt, defaultInt, FieldType.INT);
    public readonly float AsFloat() => MatchOrDefault(valFloat, defaultFloat, FieldType.FLOAT);
    public readonly bool AsBool() => MatchOrDefault(valBool, defaultBool, FieldType.BOOL);
    public readonly string AsString() => MatchOrDefault(valString, defaultString, FieldType.STRING);
    public readonly Color AsColor() => MatchOrDefault(valColor, defaultColor, FieldType.COLOR);
    public readonly string AsEnum() => MatchOrDefault(valString, defaultString, FieldType.ENUM);
    public readonly T AsEnum<T>() where T : struct {
        string enumValue = AsEnum();
        if (!Enum.TryParse(enumValue, out T cast)) {
            Log.Warning($"LDTKLoader: Cannot cast field value {enumValue} to enum type {typeof(T)},returning default.");
            return default;
        }
        return cast;
    }
    public readonly string AsFilePath() => MatchOrDefault(valString, defaultString, FieldType.FILE_PATH);
    public readonly FieldTile AsTile() => MatchOrDefault(valTile, defaultTile, FieldType.TILE);
    public readonly FieldEntity AsEntity() => MatchOrDefault(valEntity, defaultEntity, FieldType.ENTITY);
    public readonly Point2 AsPoint() => MatchOrDefault(valPoint, defaultPoint, FieldType.POINT);

    public readonly int[] AsIntArray() => [.. AsArray().Select(e => e.AsInt())];
    public readonly float[] AsFloatArray() => [.. AsArray().Select(e => e.AsFloat())];
    public readonly bool[] AsBoolArray() => [.. AsArray().Select(e => e.AsBool())];
    public readonly string[] AsStringArray() => [.. AsArray().Select(e => e.AsString())];
    public readonly Color[] AsColorArray() => [.. AsArray().Select(e => e.AsColor())];
    public readonly string[] AsEnumArray() => [.. AsArray().Select(e => e.AsEnum())];
    public readonly T[] AsEnumArray<T>() where T : struct => [.. AsArray().Select(e => e.AsEnum<T>())];
    public readonly string[] AsFilePathArray() => [.. AsArray().Select(e => e.AsFilePath())];
    public readonly FieldTile[] AsTileArray() => [.. AsArray().Select(e => e.AsTile())];
    public readonly FieldEntity[] AsEntityArray() => [.. AsArray().Select(e => e.AsEntity())];
    public readonly Point2[] AsPointArray() => [.. AsArray().Select(e => e.AsPoint())];

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