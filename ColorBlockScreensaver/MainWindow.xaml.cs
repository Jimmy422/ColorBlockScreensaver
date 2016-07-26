#region copyright
// C# Screensaver Template Copyright (c) 2015 Wm. Barrett Simms wbsimms.com
// Base template modified by James Keith
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
#endregion
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorBlockScreensaver
{
	public partial class MainWindow : Window
	{
        Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);
        Random randomGenerator = new Random();

        double colorHueThreshold = 0.5;
        double sweepSpeed;
        double blockTimeOffset;

        bool autoDark;
        bool changeColors;
        bool displayHexCodes;
        bool animationMode;
        bool animationBounce;

        int blockPixelOffset;
        int runoffBlocks;
        int blockWidth;
        int fontSize;

        public MainWindow()
		{
			InitializeComponent();
            loadSettings();
            this.Loaded += MainWindow_Loaded;

		}

        /// <summary>
        /// Loads the settings saved under the screensaver preferences
        /// saved registry key. Also performs try/catch on the registry
        /// keys to determine if valid values were inputted for each
        /// parameter.
        /// </summary>
        private void loadSettings()
        {
            RegistryKey newKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\ColorBlockScreensaver");
            if (newKey == null)
            {
                autoDark = false;
                changeColors = true;
                displayHexCodes = false;
                animationMode = false;
                animationBounce = true;
                sweepSpeed = 15;
                blockPixelOffset = 100;
                runoffBlocks = 8;
                blockWidth = 125;
                fontSize = 36;
                blockTimeOffset = 0.75;
            }
            else
            {
                autoDark = Convert.ToBoolean(newKey.GetValue("autoDark"));
                changeColors = Convert.ToBoolean(newKey.GetValue("changeColors"));
                displayHexCodes = Convert.ToBoolean(newKey.GetValue("hexCodes"));
                animationMode = Convert.ToBoolean(newKey.GetValue("animMode"));
                animationBounce = Convert.ToBoolean(newKey.GetValue("animBounce"));

                try
                {
                    sweepSpeed = Convert.ToDouble(newKey.GetValue("sweepSpeed"));
                }
                catch
                {
                    MessageBox.Show("The sweep speed property is not valid. Please reconfigure the screensaver with a valid number.", "Screensaver Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                try
                {
                    blockPixelOffset = Convert.ToInt32(newKey.GetValue("blockPixelOffset"));
                }
                catch
                {
                    MessageBox.Show("The block pixel offset property is not valid. Please reconfigure the screensaver with a valid number.", "Screensaver Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                try
                {
                    runoffBlocks = Convert.ToInt32(newKey.GetValue("runoffBlocks"));
                }
                catch
                {
                    MessageBox.Show("The runoff blocks property is not valid. Please reconfigure the screensaver with a valid number.", "Screensaver Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                try
                {
                    blockWidth = Convert.ToInt32(newKey.GetValue("blockWidth"));
                }
                catch
                {
                    MessageBox.Show("The block width property is not valid. Please reconfigure the screensaver with a valid number.", "Screensaver Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                try
                {
                    fontSize = Convert.ToInt32(newKey.GetValue("fontSize"));
                }
                catch
                {
                    MessageBox.Show("The font size property is not valid. Please reconfigure the screensaver with a valid number.", "Screensaver Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }

                try
                {
                    blockTimeOffset = Convert.ToDouble(newKey.GetValue("blockTimeOffset"));
                }
                catch
                {
                    MessageBox.Show("The block time offset property is not valid. Please reconfigure the screensaver with a valid number.", "Screensaver Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
        }

        /// <summary>
        /// Calculates the maximum amount of color bars to display, including "runoff" blocks -
        /// the extra blocks to fill dead space in the screen
        /// </summary>
        /// <param name="screenHeight">The height of the screen, in pixels</param>
        /// <returns></returns>
        private int calculateMaxColorBlocks(int screenHeight)
        {
            return (screenHeight / blockWidth) + runoffBlocks;
        }

        /// <summary>
        /// Calls on the random number generator to generate a byte
        /// array of 3 different colors (Red, Green, and Blue)
        /// </summary>
        /// <param name="colorArray">A byte array with a minimum size of 3 bytes</param>
        private void generateRandomColors(byte[] colorArray)
        {
            int currentHour = DateTime.Now.Hour;

            if(autoDark && (currentHour > 20 || currentHour < 8))
            {
                colorArray[0] = (byte)randomGenerator.Next(0, 32);
                colorArray[1] = (byte)randomGenerator.Next(0, 32);
                colorArray[2] = (byte)randomGenerator.Next(0, 32);
            }
            else
            {
                colorArray[0] = (byte)randomGenerator.Next(0, 255);
                colorArray[1] = (byte)randomGenerator.Next(0, 255);
                colorArray[2] = (byte)randomGenerator.Next(0, 255);
            }
            
        }

        /// <summary>
        /// A formula from StackOverflow question #1855884
        /// for determining a value from 0 to 1 on how "light" or "dark" a color is.
        /// </summary>
        /// <param name="colorArray">A byte array with a minimum size of 3 bytes, storing RGB color information</param>
        /// <returns></returns>
        private double getAverageColorValue(byte[] colorArray)
        {
            return (1 - (0.299 * colorArray[0] + 0.587 * colorArray[1] + 0.114 * colorArray[2]) / 255);
        }

        /// <summary>
        /// Sets the text in a text block to be centered and size 36
        /// </summary>
        /// <param name="textBlockToSetup">The text block to apply the parameters to</param>
        private void setupTextBlock(TextBlock textBlockToSetup)
        {
            textBlockToSetup.FontSize = fontSize;
            textBlockToSetup.VerticalAlignment = VerticalAlignment.Center;
            textBlockToSetup.HorizontalAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Sets the text in a textblock to be the HTML hexadecimal
        /// code for a color
        /// </summary>
        /// <param name="colorArray">The color, stored in a byte array</param>
        /// <param name="textBlock">The text block to set the text in</param>
        private void setTextBlockHexText(byte[] colorArray, TextBlock textBlock)
        {
            textBlock.Text = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(colorArray[0], colorArray[1], colorArray[2]));
        }

        /// <summary>
        /// Determines if the text in a textblock should be white or black
        /// depending on the hue
        /// </summary>
        /// <param name="colorArray">The array of colors to make the determination on</param>
        /// <param name="hueThreshold">The dark/light threshold, default is 0.5</param>
        /// <param name="textBlock">The textblock to set the color in</param>
        private void determineAndSetTextColorValue(byte[] colorArray, double hueThreshold, TextBlock textBlock)
        {
            if (getAverageColorValue(colorArray) > hueThreshold)
            {
                textBlock.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Sets up the animation for each block
        /// </summary>
        /// <param name="doubleAnimation">The animation created for each block</param>
        /// <param name="screenWidth">The width of the screen</param>
        /// <param name="blockOffset">The pixel offset of each block added during each loop</param>
        /// <param name="blockTimeOffset">The time offset of each block added during each loop</param>
        private void setupAnimation(DoubleAnimation doubleAnimation, int screenWidth, int blockOffset, double blockTimeOffset)
        {
            if(animationMode) //Fade in and out rather than sweep
            {
                doubleAnimation.From = 0;
                doubleAnimation.To = 1;
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(blockTimeOffset));
                if(animationBounce)
                {
                    doubleAnimation.AutoReverse = true;
                }
                else
                {
                    doubleAnimation.AutoReverse = false;
                }
                
            }
            else
            {
                doubleAnimation.From = (-screenWidth) - blockOffset;
                doubleAnimation.To = (screenWidth);
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(blockTimeOffset));
                if (animationBounce)
                {
                    doubleAnimation.AutoReverse = true;
                }
                else
                {
                    doubleAnimation.AutoReverse = false;
                }
            }
            
        }

        /// <summary>
        /// Called when the window has loaded, this is the "meat" of the screensaver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
            byte[] randomColorArray = new byte[3];

            generateRandomColors(randomColorArray);

            int screenWidth = (int)this.MainCanvas.RenderSize.Width;
            int screenHeight = (int)this.MainCanvas.RenderSize.Height;

            int maximumGeneratedRectangles = calculateMaxColorBlocks(screenHeight);

            int rectangleGeneratePositionY = (screenHeight/3);
            int rectangleGeneratePositionX = -(screenWidth / 2) - ((runoffBlocks / 2)* blockWidth);

            int blockOffset = blockWidth; //These two variables offset the blocks during creation
            double blockTimeDelayOffset = sweepSpeed; //to prevent overlap during animation 15.0f is default

            System.Console.WriteLine("Max Generated Rectangles: " + maximumGeneratedRectangles);

            for (int i = 0; i < maximumGeneratedRectangles; i++)
            {
                System.Console.WriteLine("Time Delay in creation: " + blockTimeDelayOffset);
                generateRandomColors(randomColorArray);

                Color randomGeneratedColor = Color.FromRgb(randomColorArray[0], randomColorArray[1], randomColorArray[2]);

                Grid newRectangle = new Grid(); //We use a grid because rectangles can't contain text to display the hexadecimal code inside
                newRectangle.Background = new SolidColorBrush(randomGeneratedColor);
                newRectangle.Width = screenWidth;
                newRectangle.Height = blockWidth;

                TextBlock newColorHexTextBlock = new TextBlock();
                if(displayHexCodes)
                {
                    setupTextBlock(newColorHexTextBlock);
                    determineAndSetTextColorValue(randomColorArray, colorHueThreshold, newColorHexTextBlock);
                    setTextBlockHexText(randomColorArray, newColorHexTextBlock);
                }
                

                DoubleAnimation screenSweepAnimation = new DoubleAnimation();
                setupAnimation(screenSweepAnimation, screenWidth, blockOffset, blockTimeDelayOffset);
                if (animationMode) //Fade in and out rather than sweep
                {
                    screenSweepAnimation.Completed += (a, b) => //This is used to change the color once the block has completed two sweeps
                    {
                        if (changeColors)
                        {
                            generateRandomColors(randomColorArray);

                            randomGeneratedColor = Color.FromRgb(randomColorArray[0], randomColorArray[1], randomColorArray[2]);

                            newRectangle.Background = new SolidColorBrush(randomGeneratedColor);


                            if (displayHexCodes)
                            {
                                determineAndSetTextColorValue(randomColorArray, colorHueThreshold, newColorHexTextBlock);
                                setTextBlockHexText(randomColorArray, newColorHexTextBlock);
                            }
                        }

                        setupAnimation(screenSweepAnimation, screenWidth, blockOffset, blockTimeDelayOffset);

                        newRectangle.BeginAnimation(OpacityProperty, screenSweepAnimation);

                        System.Console.WriteLine("Time Delay In Completed: " + blockTimeDelayOffset);
                    };
                    setupAnimation(screenSweepAnimation, screenWidth, blockOffset, blockTimeDelayOffset);
                    newRectangle.BeginAnimation(OpacityProperty, screenSweepAnimation);
                }
                else
                {
                    screenSweepAnimation.Completed += (a, b) => //This is used to change the color once the block has completed two sweeps
                    {
                        if (changeColors)
                        {
                            generateRandomColors(randomColorArray);

                            randomGeneratedColor = Color.FromRgb(randomColorArray[0], randomColorArray[1], randomColorArray[2]);

                            newRectangle.Background = new SolidColorBrush(randomGeneratedColor);


                            if (displayHexCodes)
                            {
                                determineAndSetTextColorValue(randomColorArray, colorHueThreshold, newColorHexTextBlock);
                                setTextBlockHexText(randomColorArray, newColorHexTextBlock);
                            }
                        }

                        setupAnimation(screenSweepAnimation, screenWidth, blockOffset, blockTimeDelayOffset);

                        newRectangle.BeginAnimation(Canvas.LeftProperty, screenSweepAnimation);
                    };
                    setupAnimation(screenSweepAnimation, screenWidth, blockOffset, blockTimeDelayOffset);
                    newRectangle.BeginAnimation(Canvas.LeftProperty, screenSweepAnimation);
                }
                

                Canvas.SetTop(newRectangle, rectangleGeneratePositionY);

                if (animationMode) //Fade in and out rather than sweep
                {
                    Canvas.SetLeft(newRectangle, rectangleGeneratePositionX);
                }
                else
                {

                }

                RotateTransform blockAngle = new RotateTransform(45, (screenWidth / 2), (screenHeight / 2));
                newRectangle.RenderTransform = blockAngle;

                if (displayHexCodes)
                {
                    newRectangle.Children.Add(newColorHexTextBlock);
                }

                MainCanvas.Children.Add(newRectangle);

                rectangleGeneratePositionX += blockWidth + blockPixelOffset;
                blockOffset += blockWidth;

                

                blockTimeDelayOffset += blockTimeOffset;
            }

            WindowState = WindowState.Maximized;
			Mouse.OverrideCursor = Cursors.None;
		}

        /// <summary>
        /// When the mouse is clicked, exit the screensaver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Application.Current.Shutdown();
		}


        /// <summary>
        /// When a key is pressed, exit the screensaver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			Application.Current.Shutdown();
		}

        /// <summary>
        /// When the mouse is moved more than 20 pixels, exit the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (OriginalLocation.X == int.MaxValue & OriginalLocation.Y == int.MaxValue)
            {
                OriginalLocation = e.GetPosition(null);
            }

            if (Math.Abs(e.GetPosition(null).X - OriginalLocation.X) > 20 | Math.Abs(e.GetPosition(null).Y - OriginalLocation.Y) > 20)
            {
               Application.Current.Shutdown();
            }

        }
    }
}
