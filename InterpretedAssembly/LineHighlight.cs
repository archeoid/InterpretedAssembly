using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
namespace InterpretedAssembly
{

    public class LineHighlightRenderer : IBackgroundRenderer
    {
        static Pen pen;
        static SolidColorBrush brush;
        int currentLine { get; set; }
        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }
        public LineHighlightRenderer()
        {
            stopHighlight();
            brush = new SolidColorBrush(Color.FromRgb(90, 90, 90));
            pen = new Pen(brush, 1);
        }
        public void stopHighlight()
        {
            currentLine = -1;
        }
        public void setLine(int i)
        {
            currentLine = i;
        }
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
                  
            foreach (var v in textView.VisualLines)
            {
                Rect rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();
                int linenum = v.FirstDocumentLine.LineNumber - 1;

                if (linenum == currentLine)
                    drawingContext.DrawRectangle(brush, pen, new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
            }
        }
    }
    class LineHighlight
    {
    }
}
