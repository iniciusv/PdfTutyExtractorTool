using System;
using System.IO;
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace PdfRectangleExtractor
{
	class Program
	{
		static void Main(string[] args)
		{
			// Caminho para o arquivo PDF
			string pdfPath = @"C:\Users\vinic\Downloads\TutyTRAMPO\Austin Metro\teste.pdf";

			// Verificar se o arquivo existe
			if (!File.Exists(pdfPath))
			{
				Console.WriteLine("O arquivo PDF não foi encontrado.");
				return;
			}

			// Abrir o PDF
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath));

			// Instancia o detector de retângulos
			RectangleDetector detector = new RectangleDetector();

			// Iterar sobre as páginas do PDF
			for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
			{
				Console.WriteLine($"Página {pageNum}:");

				var page = pdfDoc.GetPage(pageNum);

				// Processar o conteúdo da página para detectar retângulos
				var rectangles = detector.DetectRectangles(page);

				// Exibir as coordenadas e o texto dos retângulos detectados
				foreach (var rect in rectangles)
				{
					Console.WriteLine($"Retângulo encontrado!");
					Console.WriteLine($"Canto Inferior Esquerdo: ({rect.LowerLeftX}, {rect.LowerLeftY}), Canto Superior Direito: ({rect.UpperRightX}, {rect.UpperRightY})");

					Console.WriteLine("Linhas de texto e posições relativas:");
					foreach (var line in rect.TextLines)
					{
						Console.WriteLine($"Posição Y Relativa: {line.RelativeY}, Texto: {line.Text}");
					}
				}

				Console.WriteLine();
			}

			pdfDoc.Close();
		}
	}
}
