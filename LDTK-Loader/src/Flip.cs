namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Information for flipping a element on both the x and y axis.
/// </summary>
/// <param name="flipX">If the element should be flipped on the x axis.</param>
/// <param name="flipY">If the element should be flipped on the y axis.</param>
public readonly struct TwoAxisFlip(bool flipX, bool flipY) {
    /// <summary>
    /// If the element is flipped on the x axis.
    /// </summary>
    public readonly bool FlipX = flipX;

    /// <summary>
    /// If the element is flipped on the y axis.
    /// </summary>
    public readonly bool FlipY = flipY;

    /// <summary>
    /// If the element isn't being flipped.
    /// </summary>
    public readonly bool Neither = !flipX && !flipY;

    /// <summary>
    /// If the element is flipped on both axis.
    /// </summary>
    public readonly bool Both = flipX && flipY;

    /// <summary>
    /// Information for flipping a element on both the x and y axis.
    /// </summary>
    /// <param name="flipBits">Flip information packed into an int, with the first two bits representing the x and y flip values.</param>
    public TwoAxisFlip(int flipBits) : this(BitSet(flipBits, 0), BitSet(flipBits, 1)) { }

    static bool BitSet(int bits, int digit) => ((bits << digit) & 1) != 0;
}