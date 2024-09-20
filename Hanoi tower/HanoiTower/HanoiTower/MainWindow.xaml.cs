using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HanoiTower
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Private members

		HanoiTowerClass tower = new HanoiTowerClass();
		private int handPosition = 0;
		private int fromPosition = 0;
		private int holdStep = 0;

		private DispatcherTimer timer = new DispatcherTimer();
		private int seconds = 0;
		private int miliseconds = 0;

		List<Rectangle> rects = new List<Rectangle>();
		SpeechSynthesizer speaker = new SpeechSynthesizer();

		Image hand = new Image();
		Image diyCursor = new Image();
		Image welldone = new Image();

		#endregion

		public MainWindow()
		{
			InitializeComponent();
		}

		#region Private methods

		private void RedrawTower(List<int> tower, int columns)
		{
			int j = 0;
			for (int i = tower.Count - 1; i >= 0; i--)
			{
				Canvas.SetTop(rects[tower[i] - 1], Constant.BottomTop - Constant.StepThickness * ++j);
				Canvas.SetLeft(rects[tower[i] - 1], Constant.ColumnWidth * columns - rects[tower[i] - 1].Width / 2 + Constant.Offset);
			}
		}

		private void RedrawEnvironment()
		{
			List<int> towerLeft = tower.GetTowerLeft();
			List<int> towerMiddle = tower.GetTowerMiddle();
			List<int> towerRight = tower.GetTowerRight();

			RedrawTower(towerLeft, 1);
			RedrawTower(towerMiddle, 2);
			RedrawTower(towerRight, 3);

			Canvas.SetLeft(hand, Constant.ColumnWidth * (handPosition + 1));
			Canvas.SetTop(hand, Constant.HandTop);

			if (holdStep > 0)
			{
				Canvas.SetTop(rects[holdStep - 1], Constant.HandTop);
				Canvas.SetLeft(rects[holdStep - 1], (handPosition + 1) * Constant.ColumnWidth - rects[holdStep - 1].Width / 2);
			}
		}

		private Towers GetTowerFromPos(int pos)
		{
			if (pos == 0) return Towers.Left;
			else if (pos == 1) return Towers.Middle;
			else return Towers.Right;
		}

		private void WellDone()
		{
			timer.Stop();
			//say well done
			speaker.SpeakAsyncCancelAll();
			speaker.SpeakAsync("Well done!");

			DoubleAnimation d = new DoubleAnimation(-300, 20, new Duration(new TimeSpan(0, 0, 1)));
			Canvas.SetLeft(welldone, 135);
			Canvas.SetTop(welldone, -300);
			welldone.Visibility = System.Windows.Visibility.Visible;

			welldone.BeginAnimation(TopProperty, d);
		}

		private void InitEnvironment()
		{
			hand.Source = new BitmapImage(new Uri(@"/HanoiTower;component/Images/cursor_hand.png", UriKind.Relative));
			hand.Visibility = System.Windows.Visibility.Hidden;
			welldone.Source = new BitmapImage(new Uri(@"/HanoiTower;component/Images/wd1.png", UriKind.Relative));
			welldone.Visibility = System.Windows.Visibility.Hidden;
			canvasMain.Children.Add(hand);
			canvasMain.Children.Add(welldone);
			rects.Add(step1);
			rects.Add(step2);
			rects.Add(step3);
			rects.Add(step4);
			rects.Add(step5);
			rects.Add(step6);

			timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
			timer.Tick += new EventHandler(timer_Tick);
		}

		private void StartNewGame(int steps)
		{
			welldone.Visibility = System.Windows.Visibility.Hidden;
			timer.Stop();
			lblTime.Content = "TIME: 0";
			seconds = 0;
			miliseconds = 0;
			holdStep = 0;
			handPosition = 0;
			tower.Init(steps);
			for (int i = 0; i < steps; i++)
				rects[i].Visibility = System.Windows.Visibility.Visible;
			for (int i = steps; i < Constant.MaxSteps; i++)
				rects[i].Visibility = System.Windows.Visibility.Hidden;
			RedrawEnvironment();
			hand.Visibility = System.Windows.Visibility.Visible;
		}

		#endregion

		#region Events

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (welldone.Visibility == System.Windows.Visibility.Visible && !(e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6)) return;

			if (e.Key == Key.A || e.Key == Key.Left) //move left
			{
				if (handPosition > 0)
					handPosition--;
				else
					handPosition = 2;
			}
			else if (e.Key == Key.D || e.Key == Key.Right) //move right
			{
				if (handPosition > 1)
					handPosition = 0;
				else
					handPosition++;
			}
			else if (e.Key == Key.J || e.Key == Key.S || e.Key == Key.Down) // hold/drop 
			{
				if (!timer.IsEnabled)
					timer.Start();
				if (holdStep == 0)
				{
					fromPosition = handPosition;
					holdStep = tower.GetStep(GetTowerFromPos(fromPosition));
				}
				else if (holdStep > 0)
				{
					bool successMove = tower.Move(GetTowerFromPos(fromPosition), GetTowerFromPos(handPosition));
					if (successMove)
					{
						holdStep = 0;
					}
					else
					{
						speaker.SpeakAsyncCancelAll();
						speaker.SpeakAsync("Can not drop there.");
					}
				}
			}
			else if (e.Key == Key.D3)
			{
				StartNewGame(3);
			}
			else if (e.Key == Key.D4)
			{
				StartNewGame(4);
			}
			else if (e.Key == Key.D5)
			{
				StartNewGame(5);
			}
			else if (e.Key == Key.D6)
			{
				StartNewGame(6);
			}

			RedrawEnvironment();
			if (tower.IsComplete())
			{
				WellDone();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			speaker.SpeakAsyncCancelAll();
			speaker.SpeakAsync("Welcome to Hanoi tower, press number 3 to 6 to start new game.");
			InitEnvironment();
			StartNewGame(3);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			miliseconds++;
			if (miliseconds == 100)
			{
				if (seconds < 1000)
					seconds++;
				miliseconds = 0;
			}
			if (seconds > 999)
				lblTime.Content = "More than 999s";
			else
				lblTime.Content = string.Format("TIME: {0}:{1}", seconds, miliseconds);
		}

		#endregion
	}
}
