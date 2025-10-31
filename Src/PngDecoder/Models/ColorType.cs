namespace PngDecoder.Models;
[Flags]
public enum ColorType : byte
{
    Greyscale = 0,
    PaletteUsed = 1,
    Color = 2,
    Palette = PaletteUsed | Color,
    AlphaUsed = 4,
    RGBA = Color | AlphaUsed,
}
