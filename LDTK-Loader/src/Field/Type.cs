namespace Jackdaw.Loader.LDTK;

/// <summary>
/// The types of variables supported by variable fields.
/// </summary>
public enum FieldType {
    INVALID,

    INT,
    FLOAT,
    BOOL,
    STRING,
    COLOR,
    ENUM,
    FILE_PATH,
    TILE,
    ENTITY,
    POINT,
    ARRAY
}