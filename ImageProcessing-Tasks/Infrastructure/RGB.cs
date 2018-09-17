namespace ImageProcessing_Tasks.Infrastructure
{
    public struct RGB
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public RGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static RGB operator-(RGB first, RGB second)
        {
            return new RGB(first.R - second.R, first.G - second.G, first.B - second.B);
        }
    }
}
