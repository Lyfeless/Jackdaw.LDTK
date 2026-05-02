using Foster.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

public class FieldContainerJsonConverter : JsonConverter<FieldContainer> {
    ref struct FieldPropertyArgs() {
        public string Type = string.Empty;
        public string Name = string.Empty;
        public Utf8JsonReader Data = new();

        public readonly bool Ready =>
            Type != string.Empty &&
            Name != string.Empty &&
            Data.TokenType != JsonTokenType.None;
    };

    const string propertyNameType = "__type";
    const string propertyNameIdentifier = "__identifier";
    const string propertyNameValue = "__value";

    const string typeNameEnumPrefix = "LocalEnum.";
    const string typeNameArrayPrefix = "Array<";

    const string typeNameInt = "Int";
    const string typeNameFloat = "Float";
    const string typeNameBool = "Bool";
    const string typeNameString = "String";
    const string typeNameColor = "Color";
    const string typeNameFilePath = "FilePath";
    const string typeNameTile = "Tile";
    const string typeNameEntity = "EntityRef";
    const string typeNamePoint = "Point";

    const string typeNameTileX = "x";
    const string typeNameTileY = "y";
    const string typeNameTileWidth = "w";
    const string typeNameTileHeight = "h";
    const string typeNameTileTileset = "tilesetUid";

    const string typeNamePointX = "cx";
    const string typeNamePointY = "cy";

    const string typeNameEntityEntity = "entityIid";
    const string typeNameEntityLayer = "layerIid";
    const string typeNameEntityLevel = "levelIid";
    const string typeNameEntityWorld = "worldIid";

    public override FieldContainer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        FieldContainer container = new();

        reader.Read();

        while (reader.TokenType != JsonTokenType.EndArray) {
            if (reader.TokenType == JsonTokenType.StartObject) {
                Field field = FieldReadSingleField(reader);
                if (field.Type != FieldType.INVALID) {
                    container.Fields.Add(field.Name, field);
                }
            }

            reader.TrySkip();
            reader.Read();
        }

        return container;
    }

    public override void Write(Utf8JsonWriter writer, FieldContainer value, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }

    static Field FieldReadSingleField(Utf8JsonReader reader) {
        FieldPropertyArgs args = new();

        while (reader.TokenType != JsonTokenType.EndObject) {
            reader.Read();
            string property = reader.GetString() ?? string.Empty;

            reader.Read();

            switch (property) {
                case propertyNameIdentifier: args.Name = reader.GetString() ?? string.Empty; break;
                case propertyNameType: args.Type = reader.GetString() ?? string.Empty; break;
                case propertyNameValue: args.Data = reader; break;
            }
            if (args.Ready) { return CreateField(args); }

            if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.StartArray) {
                reader.TrySkip();
                reader.Read();
            }
        }

        return new();
    }

    static Field CreateField(FieldPropertyArgs args) {
        if (args.Type.StartsWith(typeNameArrayPrefix)) { return CreateArray(args); }
        if (args.Type.StartsWith(typeNameEnumPrefix)) { return CreateEnum(args); }

        return args.Type switch {
            typeNameInt => CreateInt(args),
            typeNameFloat => CreateFloat(args),
            typeNameBool => CreateBool(args),
            typeNameString => CreateString(args),
            typeNameColor => CreateColor(args),
            typeNameFilePath => CreateFilePath(args),
            typeNameTile => CreateTile(args),
            typeNameEntity => CreateEntity(args),
            typeNamePoint => CreatePoint(args),
            _ => new()
        };
    }

    static Field CreateArray(FieldPropertyArgs args) {
        Utf8JsonReader reader = args.Data;
        string type = args.Type.Substring(typeNameArrayPrefix.Length, args.Type.Length - typeNameArrayPrefix.Length - 1);

        // Skip array start
        reader.Read();

        List<Field> children = [];

        for (int i = 0; reader.TokenType != JsonTokenType.EndArray; ++i) {
            children.Add(CreateField(new() {
                Name = $"{args.Name}[{i}]",
                Type = type,
                Data = reader
            }));
            if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.StartArray) {
                reader.TrySkip();
                // reader.Read();
            }
            reader.Read();
        }

        return Field.FromArray(args.Name, [.. children]);
    }

    static Field CreateEnum(FieldPropertyArgs args) {
        string enumName = args.Type[typeNameEnumPrefix.Length..];
        return Field.FromEnum(args.Name, args.Data.GetString() ?? string.Empty, enumName);
    }

    static Field CreateInt(FieldPropertyArgs args) => Field.FromInt(args.Name, args.Data.GetInt32());
    static Field CreateFloat(FieldPropertyArgs args) => Field.FromFloat(args.Name, args.Data.GetSingle());
    static Field CreateBool(FieldPropertyArgs args) => Field.FromBool(args.Name, args.Data.GetBoolean());
    static Field CreateString(FieldPropertyArgs args) => Field.FromString(args.Name, args.Data.GetString() ?? string.Empty);
    static Field CreateFilePath(FieldPropertyArgs args) => Field.FromFilePath(args.Name, args.Data.GetString() ?? string.Empty);
    static Field CreateColor(FieldPropertyArgs args) => Field.FromColor(args.Name, Color.FromHexStringRGB(args.Data.GetString() ?? Color.White.ToHexStringRGB()));

    static Field CreateTile(FieldPropertyArgs args) {
        Utf8JsonReader reader = args.Data;

        int x = 0;
        int y = 0;
        int width = 0;
        int height = 0;
        int tileset = 0;

        reader.Read();

        while (reader.TokenType != JsonTokenType.EndObject) {
            string property = reader.GetString() ?? string.Empty;
            reader.Read();
            int value = reader.GetInt32();

            switch (property) {
                case typeNameTileX: x = value; break;
                case typeNameTileY: y = value; break;
                case typeNameTileWidth: width = value; break;
                case typeNameTileHeight: height = value; break;
                case typeNameTileTileset: tileset = value; break;
            }

            reader.Read();
        }

        return Field.FromTile(args.Name, new() {
            Tileset = tileset,
            Rect = new(x, y, width, height)
        });
    }

    static Field CreatePoint(FieldPropertyArgs args) {
        Utf8JsonReader reader = args.Data;


        int x = 0;
        int y = 0;

        reader.Read();

        while (reader.TokenType != JsonTokenType.EndObject) {
            string property = reader.GetString() ?? string.Empty;
            reader.Read();
            int value = reader.GetInt16();

            switch (property) {
                case typeNamePointX: x = value; break;
                case typeNamePointY: y = value; break;
            }

            reader.Read();
        }

        return Field.FromPoint(args.Name, new(x, y));
    }

    static Field CreateEntity(FieldPropertyArgs args) {
        Utf8JsonReader reader = args.Data;

        reader.Read();

        Guid entity = Guid.Empty;
        Guid layer = Guid.Empty;
        Guid level = Guid.Empty;
        Guid world = Guid.Empty;

        while (reader.TokenType != JsonTokenType.EndObject) {
            string? property = reader.GetString() ?? string.Empty;
            reader.Read();
            string? value = reader.GetString() ?? string.Empty;
            switch (property) {
                case typeNameEntityEntity: entity = new(value); break;
                case typeNameEntityLayer: layer = new(value); break;
                case typeNameEntityLevel: level = new(value); break;
                case typeNameEntityWorld: world = new(value); break;
            }

            reader.Read();
        }

        return Field.FromEntity(args.Name, new() {
            EntityID = entity,
            LayerID = layer,
            LevelID = level,
            WorldID = world
        });
    }
}