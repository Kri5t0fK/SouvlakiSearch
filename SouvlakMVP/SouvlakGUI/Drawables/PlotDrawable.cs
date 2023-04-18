using Microsoft.Maui.Graphics;

namespace SouvlakGUI.Drawables;

public class PlotDrawable : IDrawable
{
    public List<float> bestWeights = new List<float>();
    public List<float> medianWeights = new List<float>();
    public List<float> worstWeights = new List<float>();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float height = dirtyRect.Height;
        float width = dirtyRect.Width;

        Color bestColor = Colors.DarkGreen;
        Color medianColor = Colors.DarkOrange;
        Color worstColor = Colors.DarkRed;
        Color AxisColor = Colors.Black;

        float basicStrokeSize = 3;
        float boldStrokesize = 4;

        int numOfWei = bestWeights.Count;

        if (numOfWei == 0 ) 
        {
            canvas.FontSize = 24;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.DrawString("Please select graph / run algoritm to see preview", width / 2, height / 2, HorizontalAlignment.Center);
        }
        else
        {
            float heightOffset = 30;
            float widthOffset = 30;

            float arrowSize = 10;

            (float low, float hig) areaYLims = (heightOffset, height - heightOffset);
            (float low, float hig) areaXLims = (widthOffset, width - widthOffset);
            
            canvas.StrokeColor = AxisColor;
            canvas.StrokeSize = boldStrokesize;

            // Axis lines
            canvas.DrawLine(areaXLims.low, areaYLims.low, areaXLims.low, areaYLims.hig + 1);
            canvas.DrawLine(areaXLims.low - 1, areaYLims.hig, areaXLims.hig, areaYLims.hig);

            // X axis arrow
            canvas.DrawLine(areaXLims.hig, areaYLims.hig - 1, areaXLims.hig - arrowSize, areaYLims.hig + arrowSize);
            canvas.DrawLine(areaXLims.hig, areaYLims.hig + 1, areaXLims.hig - arrowSize, areaYLims.hig - arrowSize);

            // Y axis arrow
            canvas.DrawLine(areaXLims.low + 1, areaYLims.low, areaXLims.low - arrowSize, areaYLims.low + arrowSize);
            canvas.DrawLine(areaXLims.low - 1, areaYLims.low, areaXLims.low + arrowSize, areaYLims.low + arrowSize);

            // Axis scale limits
            float minWeight = bestWeights.Min();
            float maxWeight = worstWeights.Max();
            (float low, float hig) yAxisLims = (minWeight / maxWeight < 0.25 ? 0 : minWeight, maxWeight);
            (float low, float hig) xAxisLims = (0, numOfWei);

            // Fonts and others
            canvas.FontSize = 15;
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.StrokeColor = Colors.Black;
            canvas.FillColor = Colors.Orange;

            // Y axis scale
            if (yAxisLims.low == yAxisLims.hig)
            {
                canvas.DrawString(yAxisLims.low.ToString(), areaXLims.low - 2 * arrowSize, areaYLims.hig / 2 + arrowSize, HorizontalAlignment.Center);
            }
            else
            {
                canvas.DrawString(yAxisLims.hig.ToString(), areaXLims.low - 2 * arrowSize, areaYLims.low + 2 * arrowSize, HorizontalAlignment.Center);
                canvas.DrawString(yAxisLims.low.ToString(), areaXLims.low - 2 * arrowSize, areaYLims.hig - arrowSize, HorizontalAlignment.Center);
            }

            // X axis scale
            canvas.DrawString(xAxisLims.hig.ToString(), areaXLims.hig - 2 * arrowSize, areaYLims.hig + 2 * arrowSize, HorizontalAlignment.Center);
            canvas.DrawString(xAxisLims.low.ToString(), areaXLims.low + arrowSize, areaYLims.hig + 2 * arrowSize, HorizontalAlignment.Center);

            // X data for plot
            List<float> xDataList = new List<float>();
            float start = areaXLims.low + arrowSize;
            float end = areaXLims.hig - 2 * arrowSize;
            xDataList = Enumerable.Range(0, numOfWei).Select(i => start + (i / (float)(numOfWei - 1)) * (end - start)).ToList();

            // Data point radius 
            float radius = new List<int>() { 6 - 2*(int)Math.Log(numOfWei, 10), 1 }.Max();

            // Random bullshit
            float yPlotableAxisLen = areaYLims.hig - areaYLims.low - 3 * arrowSize;
            float den = maxWeight - yAxisLims.low;

            canvas.StrokeSize = basicStrokeSize;

            float prevXData = 0;
            float prevYDataBest = 0;
            float prevYDataMedian = 0;
            float prevYDataWorst = 0;

            for (int i = 0; i < numOfWei; i++)
            {
                float xData = xDataList[i];
                float yDataBest;
                float yDataMedian;
                float yDataWorst;

                if (yAxisLims.low == yAxisLims.hig)
                {
                    yDataBest = areaYLims.hig / 2 + arrowSize;
                    yDataMedian = areaYLims.hig / 2 + arrowSize;
                    yDataWorst = areaYLims.hig / 2 + arrowSize;
                }
                else
                {
                    yDataBest = areaYLims.hig - arrowSize - ((bestWeights[i] - yAxisLims.low) / den) * yPlotableAxisLen;
                    yDataMedian = areaYLims.hig - arrowSize - ((medianWeights[i] - yAxisLims.low) / den) * yPlotableAxisLen;
                    yDataWorst = areaYLims.hig - arrowSize - ((worstWeights[i] - yAxisLims.low) / den) * yPlotableAxisLen;
                }
                if (i > 0)
                {
                    canvas.StrokeColor = worstColor;
                    canvas.DrawLine(xData, yDataWorst, prevXData, prevYDataWorst);
                    canvas.StrokeColor = medianColor;
                    canvas.DrawLine(xData, yDataMedian, prevXData, prevYDataMedian);
                    canvas.StrokeColor = bestColor;
                    canvas.DrawLine(xData, yDataBest, prevXData, prevYDataBest);
                }

                canvas.StrokeColor = worstColor;
                canvas.DrawCircle(xData, yDataWorst, radius);
                canvas.StrokeColor = medianColor;
                canvas.DrawCircle(xData, yDataMedian, radius);
                canvas.StrokeColor = bestColor;
                canvas.DrawCircle(xData, yDataBest, radius);

                prevXData = xData;
                prevYDataBest = yDataBest;
                prevYDataMedian = yDataMedian;
                prevYDataWorst = yDataWorst;

            }
        }
        
    }
}
