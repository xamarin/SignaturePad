using SignaturePad.Forms;

namespace Samples.Xam.Forms
{
    /// <summary>
    /// We need to inherit, otherwise there will be a XamlParseException on iOS
    /// </summary>
    public class SignaturePad : SignaturePadView
    {
    }
}
