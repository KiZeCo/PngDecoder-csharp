namespace PngDecoder.Models;

/// <summary>
/// All the data blocks which exist a PNG file
/// </summary>
internal enum PngChunkType : int
{
    IHDR = 0x52444849,
    cHRM = 0x4D524863,
    gAMA = 0x414D4167,
    sRGB = 0x42475273,
    sBIT = 0x54494273,
    PLTE = 0x45544C50,
    bKGD = 0x44474B62,
    hIST = 0x54534968,
    tRNS = 0x534E5274,
    oFFs = 0x7346466F,
    pHYs = 0x73594870,
    sCAL = 0x4C414373,
    IDAT = 0x54414449,
    tIME = 0x454D4974,
    tEXt = 0x74584574,
    zTXt = 0x7458547A,
    fRAc = 0x63415266,
    gIFg = 0x67464967,
    gIFt = 0x74464967,
    gIFx = 0x78464967,
    IEND = 0x444E4549,
    iTXt = 0x74585469,
}
