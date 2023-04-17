namespace SouvlakGUI.Drawables;

public class GraphDrawable : IDrawable
{
    public Models.Graph? Graph { get; set; } = null;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float height = dirtyRect.Height;
        float width = dirtyRect.Width;

        Color singleEdgeColor = Colors.DimGray;
        Color multiEdgeColor = Colors.DarkBlue;

        float basicStrokeSize = 3;
        float boldStrokesize = 5;

        if (Graph == null)
        {
            canvas.FontSize = 24;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.DrawString("Please select graph / run algoritm to see preview", width/2, height/2, HorizontalAlignment.Center);
        }
        else
        {
            float heightOffset = 30;
            float widthOffset = 30;
            float heightScale = (1.0f / ((float) this.Graph.graph.Max(ver => ver.position.Y)) * height) - (2 * heightOffset);
            float widthScale = (1.0f / ((float) this.Graph.graph.Max(ver => ver.position.X)) * width) - (2 * widthOffset);

            Func<float, float> scaleX = x => x * widthScale + widthOffset;
            Func<float, float> scaleY = y => y * heightScale + heightOffset;

            canvas.StrokeColor = singleEdgeColor;
            canvas.StrokeSize = basicStrokeSize;

            // Draw edges
            // Yes this loop draws every edge twice. Too bad
            for (int startIdx = 0; startIdx < Graph.graph.Count; startIdx++)
            {
                float startX = scaleX(Graph.graph[startIdx].position.X);
                float startY = scaleY(Graph.graph[startIdx].position.Y);

                foreach (var edge in Graph.graph[startIdx].edgeList)
                {
                    if (edge.count > 1)
                    {
                        canvas.StrokeColor = multiEdgeColor;
                        canvas.StrokeSize = boldStrokesize;

                        float stopX = scaleX(Graph.graph[edge.targetIdx].position.X);
                        float stopY = scaleY(Graph.graph[edge.targetIdx].position.Y);
                        canvas.DrawLine(startX, startY, stopX, stopY);

                        canvas.StrokeColor = singleEdgeColor;
                        canvas.StrokeSize = basicStrokeSize;
                    }
                    else
                    {
                        float stopX = scaleX(Graph.graph[edge.targetIdx].position.X);
                        float stopY = scaleY(Graph.graph[edge.targetIdx].position.Y);
                        canvas.DrawLine(startX, startY, stopX, stopY);
                    }
                }
            }

            float radius = 20;
            canvas.FontSize = 18;
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.StrokeColor = Colors.Black;
            canvas.FillColor = Colors.Orange;

            // Draw vertices
            for (int idx = 0; idx < Graph.graph.Count; idx++)
            {
                float x = scaleX(Graph.graph[idx].position.X);
                float y = scaleY(Graph.graph[idx].position.Y);
                
                PathF path = new PathF();
                path.AppendCircle(x, y, radius);
                canvas.FillPath(path);
                canvas.DrawPath(path);

                canvas.DrawString(idx.ToString(), x-radius, y-radius, 2*radius, 2*radius, HorizontalAlignment.Center, VerticalAlignment.Center);
            }

        }

        
    }
}
