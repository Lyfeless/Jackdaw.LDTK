using Foster.Framework;
using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

internal struct JsonElementProjectRoot() {
    [JsonPropertyName("bgColor")]
    public Color BackgroundColor { get; set; } = Color.Black;

    [JsonPropertyName("defs")]
    public JsonElementDefinitions Definitions { get; set; } = new();

    [JsonPropertyName("externalLevels")]
    public bool ExternalLevels { get; set; } = false;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("jsonVersion")]
    public string FormatVersion { get; set; } = string.Empty;

    [JsonPropertyName("levels")]
    public JsonElementLevel[] Levels { get; set; } = [];

    [JsonPropertyName("worldGridWidth")]
    public int? WorldGridWidth { get; set; } = null;

    [JsonPropertyName("worldGridHeight")]
    public int? WorldGridHeight { get; set; } = null;

    [JsonPropertyName("worldLayout")]
    [JsonConverter(typeof(JsonStringEnumConverter<JsonEnumWorldLayout>))]
    public JsonEnumWorldLayout? WorldLayout { get; set; } = null;

    [JsonPropertyName("worlds")]
    public JsonElementWorld[] Worlds { get; set; } = [];

    /*
        Skipped Info:
        - toc (Entity table of contents): Old option for entity lookups
    */
}

internal struct JsonElementDefinitions() {
    [JsonPropertyName("entities")]
    public JsonElementEntityDefinition[] Entities { get; set; } = [];

    [JsonPropertyName("enums")]
    public JsonElementEnumDefinition[] Enums { get; set; } = [];

    [JsonPropertyName("externalEnums")]
    public JsonElementEnumDefinition[] ExternalEnums { get; set; } = [];

    [JsonPropertyName("layers")]
    public JsonElementLayerDefinition[] Layers { get; set; } = [];

    [JsonPropertyName("tilesets")]
    public JsonElementTilesetDefinition[] Tilesets { get; set; } = [];

    /*
        Skipped Info:
        - levelFields (Global level field definitions): Fields defintions are empty
    */
}

internal struct JsonElementLayerDefinition() {
    [JsonPropertyName("__type")]
    [JsonConverter(typeof(JsonStringEnumConverter<JsonEnumLayerType>))]
    public JsonEnumLayerType Type { get; set; } = JsonEnumLayerType.Tiles;

    [JsonPropertyName("autoSourceLayerDefUid")]
    public int? AutoSourceLayerDefinitionID { get; set; } = null;

    [JsonPropertyName("displayOpacity")]
    public float Opacity { get; set; } = 1;

    [JsonPropertyName("gridSize")]
    public int TileSize { get; set; } = 1;

    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("intGridValues")]
    public JsonElementIntGridValue[] IntGridValues { get; set; } = [];

    [JsonPropertyName("intGridValuesGroups")]
    public JsonElementIntGridGroup[] IntGridGroups { get; set; } = [];

    [JsonPropertyName("parallaxFactorX")]
    public float ParallaxFactorX { get; set; } = 0;

    [JsonPropertyName("parallaxFactorY")]
    public float ParallaxFactorY { get; set; } = 0;

    [JsonPropertyName("parallaxScaling")]
    public bool ParallaxScaling { get; set; } = true;

    [JsonPropertyName("pxOffsetX")]
    public int OffsetX { get; set; } = 0;

    [JsonPropertyName("pxOffsetY")]
    public int OffsetY { get; set; } = 0;

    [JsonPropertyName("tilesetDefUid")]
    public int? TilesetID { get; set; } = null;

    [JsonPropertyName("uid")]
    public int ID { get; set; } = 0;

    /*
        Skipped Info:
        - autoTilesetDefUid: depricated
    */
}

internal struct JsonElementEntityDefinition() {
    [JsonPropertyName("color")]
    public Color Color { get; set; } = Color.Black;

    [JsonPropertyName("width")]
    public int Width { get; set; } = 0;

    [JsonPropertyName("height")]
    public int Height { get; set; } = 0;

    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("nineSliceBorders")]
    public int[] NineSliceBorders { get; set; } = [0, 0, 0, 0];

    [JsonPropertyName("pivotX")]
    public float PivotX { get; set; } = 0;

    [JsonPropertyName("pivotY")]
    public float PivotY { get; set; } = 0;

    [JsonPropertyName("tileRect")]
    public JsonElementTilesetRectangle? DisplayTile { get; set; } = null;

    [JsonPropertyName("tileRenderMode")]
    [JsonConverter(typeof(JsonStringEnumConverter<JsonEnumRenderMode>))]
    public JsonEnumRenderMode RenderMode { get; set; } = JsonEnumRenderMode.Stretch;

    [JsonPropertyName("tilesetId")]
    public int? TilesetID { get; set; } = null;

    [JsonPropertyName("uiTileRect")]
    public JsonElementTilesetRectangle? UIDisplayTile { get; set; } = null;

    [JsonPropertyName("uid")]
    public int ID { get; set; } = 0;

    /*
        Skipped Info:
        - tileId: depricated
    */
}

internal struct JsonElementTilesetDefinition() {
    [JsonPropertyName("__cWid")]
    public int GridWidth { get; set; } = 0;

    [JsonPropertyName("__cHei")]
    public int GridHeight { get; set; } = 0;

    [JsonPropertyName("pxWid")]
    public int WidthInPixels { get; set; } = 0;

    [JsonPropertyName("pxHei")]
    public int HeightInPixels { get; set; } = 0;

    [JsonPropertyName("customData")]
    public JsonElementTilesetCustomData[] CustomData { get; set; } = [];

    [JsonPropertyName("embedAtlas")]
    [JsonConverter(typeof(JsonStringEnumConverter<JsonEnumEmbedAtlas>))]
    public JsonEnumEmbedAtlas? InternalAtlas { get; set; } = null;

    [JsonPropertyName("enumTags")]
    public JsonElementTilesetEnumTag[] EnumTags { get; set; } = [];

    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("padding")]
    public int Padding { get; set; } = 0;

    [JsonPropertyName("relPath")]
    public string TexturePath { get; set; } = string.Empty;

    [JsonPropertyName("spacing")]
    public int Spacing { get; set; } = 0;

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];

    [JsonPropertyName("tagsSourceEnumUid")]
    public int? EnumTagID { get; set; } = null;

    [JsonPropertyName("tileGridSize")]
    public int TileSize { get; set; } = 1;

    [JsonPropertyName("uid")]
    public int ID { get; set; } = 0;

}

internal struct JsonElementTilesetRectangle() {
    [JsonPropertyName("x")]
    public int X { get; set; } = 0;

    [JsonPropertyName("y")]
    public int Y { get; set; } = 0;

    [JsonPropertyName("w")]
    public int Width { get; set; } = 0;

    [JsonPropertyName("h")]
    public int Height { get; set; } = 0;

    [JsonPropertyName("tilesetUid")]
    public int TilesetID { get; set; } = 0;
}

internal struct JsonElementEnumDefinition() {
    [JsonPropertyName("externalRelPath")]
    public string? ExternalPath { get; set; } = null;

    [JsonPropertyName("iconTilesetUid")]
    public int? TilesetID { get; set; } = null;

    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];

    [JsonPropertyName("uid")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("values")]
    public JsonElementEnumValueDefinition[] Values { get; set; } = [];

}

internal struct JsonElementEnumValueDefinition() {
    [JsonPropertyName("color")]
    public Color Color { get; set; } = Color.Black;

    [JsonPropertyName("id")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("tileRect")]
    public JsonElementTilesetRectangle? DisplayTile { get; set; } = null;



    /*
        Skipped Info:
        - tileId: depricated
        - __tileSrcRect: depricated
    */
}

internal struct JsonElementWorld() {
    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("levels")]
    public JsonElementLevel[] Levels { get; set; } = [];

    [JsonPropertyName("worldGridWidth")]
    public int WorldGridWidth { get; set; } = 0;

    [JsonPropertyName("worldGridHeight")]
    public int WorldGridHeight { get; set; } = 0;

    [JsonPropertyName("worldLayout")]
    [JsonConverter(typeof(JsonStringEnumConverter<JsonEnumWorldLayout>))]
    public JsonEnumWorldLayout WorldLayout { get; set; } = JsonEnumWorldLayout.Free;

}

internal struct JsonElementLevel() {
    [JsonPropertyName("__bgColor")]
    public Color BackgroundColor { get; set; } = Color.Black;

    [JsonPropertyName("__bgPos")]
    public JsonElementBackgroundPosition? BackgroundPosition { get; set; } = null;

    [JsonPropertyName("bgRelPath")]
    public string? BackgroundPath { get; set; } = null;

    [JsonPropertyName("__neighbours")]
    public JsonElementLevelNeighbor[] Neighbors { get; set; } = [];

    [JsonPropertyName("externalRelPath")]
    public string? ExternalDataPath { get; set; } = null;

    [JsonPropertyName("fieldInstances")]
    public FieldContainer Fields { get; set; } = new();

    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("layerInstances")]
    public JsonElementLayerInstance[] Layers { get; set; } = [];

    [JsonPropertyName("pxWid")]
    public int Width { get; set; } = 0;

    [JsonPropertyName("pxHei")]
    public int Height { get; set; } = 0;

    [JsonPropertyName("uid")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("worldDepth")]
    public int WorldDepth { get; set; } = 0;

    [JsonPropertyName("worldX")]
    public int WorldX { get; set; } = 0;

    [JsonPropertyName("worldY")]
    public int WorldY { get; set; } = 0;

}

internal struct JsonElementLayerInstance() {
    [JsonPropertyName("__cWid")]
    public int GridWidth { get; set; } = 0;

    [JsonPropertyName("__cHei")]
    public int GridHeight { get; set; } = 0;

    [JsonPropertyName("__gridSize")]
    public int TileSize { get; set; } = 0;

    [JsonPropertyName("__identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("__opacity")]
    public float Opacity { get; set; } = 1;

    [JsonPropertyName("__pxTotalOffsetX")]
    public int TotalOffsetX { get; set; } = 0;

    [JsonPropertyName("__pxTotalOffsetY")]
    public int TotalOffsetY { get; set; } = 0;

    [JsonPropertyName("__tilesetDefUid")]
    public int? TilesetID { get; set; } = null;

    [JsonPropertyName("__tilesetRelPath")]
    public string? TilesetPath { get; set; } = null;

    [JsonPropertyName("__type")]
    [JsonConverter(typeof(JsonStringEnumConverter<JsonEnumLayerType>))]
    public JsonEnumLayerType Type { get; set; } = JsonEnumLayerType.Tiles;

    [JsonPropertyName("autoLayerTiles")]
    public JsonElementTileInstance[] AutoLayerTiles { get; set; } = [];

    [JsonPropertyName("entityInstances")]
    public JsonElementEntityInstance[] Entities { get; set; } = [];

    [JsonPropertyName("gridTiles")]
    public JsonElementTileInstance[] Tiles { get; set; } = [];

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("intGridCsv")]
    public int[] IntGridElements { get; set; } = [];

    [JsonPropertyName("layerDefUid")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("levelId")]
    public int LevelID { get; set; } = 0;

    [JsonPropertyName("overrideTilesetUid")]
    public int? OverrideTilesetID { get; set; } = null;

    [JsonPropertyName("pxOffsetX")]
    public int OffsetX { get; set; } = 0;

    [JsonPropertyName("pxOffsetY")]
    public int OffsetY { get; set; } = 0;

    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;


    /*
        Skipped Info:
        - intGrid: depricated
    */
}

internal struct JsonElementTileInstance() {
    [JsonPropertyName("a")]
    public float Alpha { get; set; } = 1;

    [JsonPropertyName("f")]
    public int Flip { get; set; } = 0;

    [JsonPropertyName("px")]
    public int[] LevelPosition { get; set; } = [0, 0];

    [JsonPropertyName("src")]
    public int[] TilesetPosition { get; set; } = [0, 0];

    [JsonPropertyName("t")]
    public int TileID { get; set; } = 0;
}

internal struct JsonElementEntityInstance() {
    [JsonPropertyName("__grid")]
    public int[] GridPosition { get; set; } = [0, 0];

    [JsonPropertyName("__identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("__pivot")]
    public float[] Pivot { get; set; } = [0, 0];

    [JsonPropertyName("__smartColor")]
    public Color Color { get; set; } = Color.Black;

    [JsonPropertyName("__tags")]
    public string[] Tags { get; set; } = [];

    [JsonPropertyName("__tile")]
    public JsonElementTilesetRectangle? DisplayTile { get; set; } = null;

    [JsonPropertyName("__worldX")]
    public int? WorldX { get; set; } = null;

    [JsonPropertyName("__worldY")]
    public int? WorldY { get; set; } = null;

    [JsonPropertyName("defUid")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("fieldInstances")]
    public FieldContainer Fields { get; set; } = new();

    [JsonPropertyName("width")]
    public int Width { get; set; } = 0;

    [JsonPropertyName("height")]
    public int Height { get; set; } = 0;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("px")]
    public int[] LevelPosition { get; set; } = [0, 0];

}

internal struct JsonElementIntGridValue() {
    [JsonPropertyName("color")]
    public Color Color { get; set; } = Color.Black;

    [JsonPropertyName("groupUid")]
    public int GroupID { get; set; } = 0;

    [JsonPropertyName("identifier")]
    public string? Name { get; set; } = null;

    [JsonPropertyName("tile")]
    public JsonElementTilesetRectangle? Tile { get; set; } = null;

    [JsonPropertyName("value")]
    public int Value { get; set; } = 0;
}

internal struct JsonElementIntGridGroup() {
    [JsonPropertyName("color")]
    public Color? Color { get; set; } = null;

    [JsonPropertyName("identifier")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("uid")]
    public int ID { get; set; } = 0;
}

internal struct JsonElementTilesetCustomData() {
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("tileId")]
    public int TileID { get; set; } = 0;
}

internal struct JsonElementTilesetEnumTag() {
    [JsonPropertyName("enumValueId")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("tileIds")]
    public int[] IDs { get; set; } = [];
}

internal struct JsonElementBackgroundPosition() {
    [JsonPropertyName("cropRect")]
    public float[] CropBounds { get; set; } = [0, 0, 0, 0];

    [JsonPropertyName("scale")]
    public float[] Scale { get; set; } = [1, 1];

    [JsonPropertyName("topLeftPx")]
    public int[] Position { get; set; } = [0, 0];
}

internal struct JsonElementLevelNeighbor() {
    /* Possible options:
        - n: North
        - s: South
        - e: East
        - w: West
        - <: Lower Depth
        - >: Higher Depth
        - o: Same Depth
        - nw: North-West Corner
        - ne: North-East Corner
        - sw: Sout-West Corner
        - se: South-East Corner
    */
    [JsonPropertyName("dir")]
    public string Direction { get; set; } = "o";

    [JsonPropertyName("levelIid")]
    public string InstanceID { get; set; } = string.Empty;

    /*
        Skipped Info:
        - levelUid: depricated
    */
}

internal enum JsonEnumWorldLayout {
    Free, GridVania, LinearHorizontal, LinearVertical
}

internal enum JsonEnumLayerType {
    IntGrid, Entities, Tiles, AutoLayer
}

internal enum JsonEnumRenderMode {
    Cover, FitInside, Repeat, Stretch, FullSizeCropped, FullSizeUncropped, NineSlice
}

internal enum JsonEnumEmbedAtlas {
    LdtkIcons
}