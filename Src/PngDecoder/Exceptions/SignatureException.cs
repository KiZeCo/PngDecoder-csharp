namespace PngDecoder.Exceptions;
public class SignatureException(byte[] bytes)
    : Exception("the signature of this input stream does not have a valid starting sequence of bytes.")
{
    public readonly byte[] Bytes = bytes;
}
