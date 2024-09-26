using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PdfRectangleExtractor
{
	public class RectangleDetector
	{
		public List<RectangleCoordinates> DetectRectangles(PdfPage page)
		{
			var rectangles = new List<RectangleCoordinates>();

			// Listener para capturar os gráficos da página
			var listener = new CustomRenderListener(rectangles, page);

			// Processar o conteúdo da página
			PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
			processor.ProcessPageContent(page);

			return rectangles;
		}
	}
}
