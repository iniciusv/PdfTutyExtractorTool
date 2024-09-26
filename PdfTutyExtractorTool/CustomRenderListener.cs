using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;

namespace PdfRectangleExtractor
{
	// Listener personalizado para capturar os comandos gráficos
	public class CustomRenderListener : IEventListener
	{
		private List<RectangleCoordinates> _rectangles;
		private PdfPage _page;

		public CustomRenderListener(List<RectangleCoordinates> rectangles, PdfPage page)
		{
			_rectangles = rectangles;
			_page = page;
		}

		public void EventOccurred(IEventData data, EventType eventType)
		{
			if (eventType == EventType.RENDER_PATH)
			{
				PathRenderInfo renderInfo = (PathRenderInfo)data;

				// Obter as operações de desenho (MOVETO, LINETO, etc.)
				var path = renderInfo.GetPath();

				// Checar se o caminho é um retângulo
				foreach (Subpath subpath in path.GetSubpaths())
				{
					var segments = subpath.GetSegments();
					if (segments.Count == 4) // Verificar se são 4 segmentos (retângulo)
					{
						float minX = float.MaxValue, minY = float.MaxValue;
						float maxX = float.MinValue, maxY = float.MinValue;

						foreach (var segment in segments)
						{
							if (segment is Line line)
							{
								foreach (var point in line.GetBasePoints())
								{
									float x = (float)point.GetX();
									float y = (float)point.GetY();

									// Atualizar as coordenadas mínimas e máximas
									if (x < minX) minX = x;
									if (y < minY) minY = y;
									if (x > maxX) maxX = x;
									if (y > maxY) maxY = y;
								}
							}
						}

						// Verificar se já existe um retângulo com as mesmas coordenadas
						bool exists = _rectangles.Any(r =>
							r.LowerLeftX == minX && r.LowerLeftY == minY &&
							r.UpperRightX == maxX && r.UpperRightY == maxY);

						if (!exists)
						{
							// Extrair o texto dentro do retângulo, incluindo posições relativas
							var textLines = ExtractTextFromRectangle(minX, minY, maxX, maxY);

							// Adicionar o retângulo encontrado à lista
							_rectangles.Add(new RectangleCoordinates
							{
								LowerLeftX = minX,
								LowerLeftY = minY,
								UpperRightX = maxX,
								UpperRightY = maxY,
								TextLines = textLines
							});
						}
					}
				}
			}
		}

		private List<TextLine> ExtractTextFromRectangle(float minX, float minY, float maxX, float maxY)
		{
			var rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
			var filter = new TextRegionEventFilter(rect);

			var strategy = new MyLocationTextExtractionStrategy(minX, maxY);

			var filteredStrategy = new FilteredEventListener();
			filteredStrategy.AttachEventListener(strategy, filter);

			PdfCanvasProcessor parser = new PdfCanvasProcessor(filteredStrategy);
			parser.ProcessPageContent(_page);

			return strategy.GetTextLines();
		}

		public ICollection<EventType> GetSupportedEvents()
		{
			return null;
		}
	}
}
