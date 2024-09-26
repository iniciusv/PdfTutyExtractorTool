using System.Collections.Generic;

namespace PdfRectangleExtractor
{
	public class RectangleCoordinates
	{
		public float LowerLeftX { get; set; }
		public float LowerLeftY { get; set; }
		public float UpperRightX { get; set; }
		public float UpperRightY { get; set; }

		// Lista de linhas de texto dentro do retângulo
		public List<TextLine> TextLines { get; set; } = new List<TextLine>();
	}
}
