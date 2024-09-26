using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PdfRectangleExtractor
{
	public class MyLocationTextExtractionStrategy : ITextExtractionStrategy
	{
		private List<TextChunk> textChunks = new List<TextChunk>();
		private float rectMinX;
		private float rectMaxY;

		public MyLocationTextExtractionStrategy(float rectMinX, float rectMaxY)
		{
			this.rectMinX = rectMinX;
			this.rectMaxY = rectMaxY;
		}

		public void EventOccurred(IEventData data, EventType type)
		{
			if (type == EventType.RENDER_TEXT)
			{
				TextRenderInfo renderInfo = (TextRenderInfo)data;
				string text = renderInfo.GetText();
				Vector start = renderInfo.GetBaseline().GetStartPoint();
				float x = start.Get(Vector.I1);
				float y = start.Get(Vector.I2);

				textChunks.Add(new TextChunk { Text = text, X = x, Y = y });
			}
		}

		public ICollection<EventType> GetSupportedEvents()
		{
			return null; // Retorna null para receber todos os eventos
		}

		public string GetResultantText()
		{
			// Não utilizado, mas é necessário implementar
			return string.Empty;
		}

		public List<TextLine> GetTextLines()
		{
			// Definir uma tolerância para agrupar linhas próximas (por exemplo, 2 unidades)
			float lineTolerance = 2.0f;

			var groupedLines = textChunks
				.GroupBy(c => c.Y, new LinePositionComparer(lineTolerance))
				.OrderByDescending(g => g.Key)
				.ToList();

			List<TextLine> textLines = new List<TextLine>();

			foreach (var lineGroup in groupedLines)
			{
				// Ordenar os chunks na linha da esquerda para a direita
				var orderedChunks = lineGroup.OrderBy(c => c.X);

				// Concatenar o texto dos chunks na mesma linha
				var lineText = string.Join(" ", orderedChunks.Select(c => c.Text));

				// Posição Y relativa ao topo do retângulo
				float relativeY = rectMaxY - lineGroup.Key;

				textLines.Add(new TextLine
				{
					RelativeY = relativeY,
					Text = lineText
				});
			}

			return textLines;
		}
	}

	// Comparador personalizado para agrupar linhas com base na posição Y com uma tolerância
	public class LinePositionComparer : IEqualityComparer<float>
	{
		private readonly float tolerance;

		public LinePositionComparer(float tolerance)
		{
			this.tolerance = tolerance;
		}

		public bool Equals(float y1, float y2)
		{
			return System.Math.Abs(y1 - y2) <= tolerance;
		}

		public int GetHashCode(float y)
		{
			// Agrupar valores dentro da tolerância na mesma chave
			return (int)(y / tolerance);
		}
	}
}
