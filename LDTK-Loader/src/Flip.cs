namespace Jackdaw.Loader.LDTK;

public readonly struct TwoAxisFlip(bool flipX, bool flipY) {
    public readonly bool FlipX = flipX;
    public readonly bool FlipY = flipY;

    public TwoAxisFlip(int flipBits) : this(BitSet(flipBits, 0), BitSet(flipBits, 1)) { }

    static bool BitSet(int bits, int digit) => ((bits << digit) & 1) != 0;
}