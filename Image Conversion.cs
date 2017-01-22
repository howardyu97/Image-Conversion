using System;
using static System.Console;
using System.IO;
using System.Linq;

namespace Bme121.Pa3
{   
    static partial class Program
    {
        static void Main( )
        {
            string inputFile  = @"21_training.csv";
            string outputFile = @"21_training_edges.csv";
            int height;  // image height (number of rows)
            int width;  // image width (number of columns)
            
            // Read the input image from its csv file.
            FileStream inFile = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(inFile);
            
            //reading the first two lines of file and inputting values as height and width
            height = int.Parse(sr.ReadLine());
            width = int.Parse(sr.ReadLine());
            
            Color[ , ] inImage = new Color[height, width];
            Color[ , ] outImage = new Color[height, width];
            
            //changing pixel values into Color object array
            while(!sr.EndOfStream)
            {
                for(int row = 0; row < height; row++)
                {
                    int pixelCount = 0;
                    string line = sr.ReadLine();
                    string[] pixelVal = line.Split(',');
                    
                    for(int col = 0; col < width; col++)
                    {
                        if(pixelVal.Length - pixelCount >= 4)
                        {
                            inImage[row, col] = Color.FromArgb(int.Parse(pixelVal[pixelCount]), int.Parse(pixelVal[pixelCount+1]),
                                                               int.Parse(pixelVal[pixelCount+2]), int.Parse(pixelVal[pixelCount+3]));
                            pixelCount = pixelCount + 4;  
                        }
                    }
                }
            }
            
            
            // Generate the output image using Kirsch edge detection.
            for(int row1 = 0; row1 < height; row1++)
            {
                for(int col1 = 0; col1 < width; col1++)
                {
                    if(row1 == 0 || row1 == height - 1 || col1 == 0 || col1 == width - 1)
                    {
                        outImage[row1, col1] = inImage[row1, col1];
                    }
                    else
                    {
                        outImage[row1, col1] = GetKirschEdgeValue(inImage[row1-1, col1-1], inImage[row1-1, col1], inImage[row1-1, col1+1],
                                                                      inImage[row1, col1-1], inImage[row1, col1], inImage[row1, col1+1],
                                                                      inImage[row1+1, col1-1], inImage[row1+1, col1], inImage[row1+1, col1+1]);
                    }
                }
            }
            
            
            // Write the output image to its csv file.
            FileStream outFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(outFile);
            
            sw.WriteLine(height);
            sw.WriteLine(width);
            
            for(int row2 = 0; row2 < height; row2++)
            {
                for(int col2 = 0; col2 < width; col2++)
                {
                    if(col2 == width-1)
                    {
                        sw.WriteLine($"{outImage[row2, col2].A},{outImage[row2, col2].R},{outImage[row2, col2].G},{outImage[row2, col2].B}");
                    }
                    else
                    {
                        sw.Write($"{outImage[row2, col2].A},{outImage[row2, col2].R},{outImage[row2, col2].G},{outImage[row2, col2].B},");
                    }
                }
            }
            sw.WriteLine();
            
            //Closing FileStream, StreamWriter, and StreamReader
            sr.Dispose();
            inFile.Dispose();
            sw.Dispose();
            outFile.Dispose();
        }
        
        // This method computes the Kirsch edge-detection value for pixel color
        // at the centre location given the centre-location pixel color and the
        // colors of its eight neighbours.  These are numbered as follows.
        // The resulting color has the same alpha as the centre pixel, 
        // and Kirsch edge-detection intensities which are computed separately
        // for each of the red, green, and blue components using its eight neighbours.
        // c1 c2 c3
        // c4    c5
        // c6 c7 c8
        static Color GetKirschEdgeValue( 
            Color c1, Color c2,     Color c3, 
            Color c4, Color centre, Color c5, 
            Color c6, Color c7,     Color c8 )
        {
            int red = GetKirschEdgeValue(c1.R, c2.R, c3.R,
                                         c4.R,       c5.R,
                                         c6.R, c7.R, c8.R);

            int green = GetKirschEdgeValue(c1.G, c2.G, c3.G,
                                           c4.G,       c5.G,
                                           c6.G, c7.G, c8.G);
                                            
            int blue = GetKirschEdgeValue(c1.B, c2.B, c3.B,
                                          c4.B,       c5.B,
                                          c6.B, c7.B, c8.B);
                                             
            return(Color.FromArgb(255, red, green, blue));
        }
        
        // This method computes the Kirsch edge-detection value for pixel intensity
        // at the centre location given the pixel intensities of the eight neighbours.
        // These are numbered as follows.
        // i1 i2 i3
        // i4    i5
        // i6 i7 i8
        static int GetKirschEdgeValue
        ( 
            int i1, int i2, int i3, 
            int i4,         int i5, 
            int i6, int i7, int i8 )
        {
            int[] sum = new int[8];
            sum[0] = (i1+i2+i3)*5 + (i4+i5+i6+i7+i8)*(-3);
            sum[1] = (i2+i3+i5)*5 + (i4+i6+i7+i8+i1)*(-3);
            sum[2] = (i3+i5+i8)*5 + (i6+i7+i4+i1+i2)*(-3);
            sum[3] = (i5+i8+i7)*5 + (i1+i2+i3+i4+i6)*(-3);
            sum[4] = (i8+i6+i7)*5 + (i5+i1+i2+i3+i4)*(-3);
            sum[5] = (i6+i7+i4)*5 + (i1+i2+i3+i8+i5)*(-3);
            sum[6] = (i4+i6+i1)*5 + (i2+i3+i7+i5+i8)*(-3);
            sum[7] = (i4+i1+i2)*5 + (i3+i8+i5+i6+i7)*(-3);
            
            if(sum.Max() > 255) 
            {
                return 255;
            }
            else if(sum.Max() < 0) 
            {
                return 0;
            }
            else
            {
                return sum.Max();
            }
        }
    }
    
    // Implementation of part of System.Drawing.Color.
    // This is needed because .Net Core doesn't seem to include the assembly 
    // containing System.Drawing.Color even though docs.microsoft.com claims 
    // it is part of the .Net Core API.
    struct Color
    {
        int alpha;
        int red;
        int green;
        int blue;
        
        public int A { get { return alpha; } }
        public int R { get { return red;   } }
        public int G { get { return green; } }
        public int B { get { return blue;  } }
        
        public static Color FromArgb( int alpha, int red, int green, int blue )
        {
            Color result = new Color( );
            result.alpha = alpha;
            result.red   = red;
            result.green = green;
            result.blue  = blue;
            return result;
        }
    }
}
