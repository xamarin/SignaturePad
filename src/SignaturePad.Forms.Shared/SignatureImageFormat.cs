using System;

namespace SignaturePad.Forms
{
	public enum SignatureImageFormat
	{
		Png,
		Jpeg,

		[Obsolete ("Use Jpeg instead.")]
		Jpg = Jpeg
	}
}
