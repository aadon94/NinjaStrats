//
// Copyright (C) 2018, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	public class CandlestickPattern : Indicator
	{
		private Brush					downColor				= Brushes.DimGray;
		private CandleStickPatternLogic	logic;
		private int 					numPatternsFound;
		private TextPosition			textBoxPosition			= TextPosition.BottomRight;
		private Brush					textColor				= Brushes.DimGray;
		private Brush					upColor					= Brushes.DimGray;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description			= Custom.Resource.NinjaScriptIndicatorDescriptionCandlestickPattern;
				Name				= Custom.Resource.NinjaScriptIndicatorNameCandlestickPattern;
				Calculate			= Calculate.OnBarClose;
				IsOverlay			= true;
				DrawOnPricePanel	= true;
				DisplayInDataBox	= false;
				IsAutoScale			= false;
				PaintPriceMarkers	= false;
				Pattern				= ChartPattern.MorningStar;
				ShowAlerts			= true;
				ShowPatternCount	= true;
				TrendStrength		= 4;
				TextFont			= new Gui.Tools.SimpleFont() { Size = 14 };

				downColor			= Brushes.DimGray;
				upColor				= Brushes.DimGray;
				textColor			= Brushes.DimGray;

				AddPlot(Brushes.Transparent, Custom.Resource.CandlestickPatternFound);
			}
			else if (State == State.Configure)
			{
				if (Calculate == Calculate.OnEachTick || Calculate == Calculate.OnPriceChange)
					Calculate	= Calculate.OnBarClose;
			}
			else if (State == State.DataLoaded)
				logic = new CandleStickPatternLogic(this, TrendStrength);
			else if (State == State.Historical)
			{
				if (ChartControl != null)
				{
					downColor	= ChartControl.Properties.AxisPen.Brush; // Get the color of the chart axis brush
					textColor	= ChartControl.Properties.ChartText;
				}

				if (((SolidColorBrush)downColor).Color == ((SolidColorBrush)upColor).Color)
					upColor		= Brushes.Transparent;
				else
					upColor		= Brushes.DimGray;
			}
		}

		protected override void OnBarUpdate()
		{
			PatternFound[0] = (logic.Evaluate(Pattern) ? 1 : 0);

			switch (Pattern)
			{
				case ChartPattern.BearishBeltHold:
					{
						#region Bearish Belt Hold
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrushes[1]			= upColor;
								CandleOutlineBrushes[1] = downColor;
								BarBrush				= downColor;
							}

							Draw.Text(this, "Bearish Belt Hold" + CurrentBar, false, " Bearish Belt\nHold # " + ++numPatternsFound, 0, Math.Max(High[0], High[1]), 40, textColor,
								TextFont, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.BearishEngulfing:
					{
						#region Bearish Engulfing
						if (PatternFound[0] == 1)
						{
							BarBrushes[1]	= upColor;
							BarBrush		= downColor;

							Draw.Text(this, "Bearish Engulfing" + CurrentBar, false, " Bearish\nEngulfing # " + ++numPatternsFound, 0, Math.Max(High[0], High[1]), 50, textColor,
								TextFont, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.BearishHarami:
					{
						#region Bearish Harami
						if (PatternFound[0] == 1)
						{
							BarBrushes[1]	= upColor;
							BarBrush		= downColor;

							Draw.Text(this, "Bearish Harami" + CurrentBar, false, "Bearish\nHarami # " + ++numPatternsFound, 0, Math.Max(High[0], High[1]), 40, textColor,
								TextFont, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.BearishHaramiCross:
					{
						#region Bearish Harami Cross
						if (PatternFound[0] == 1)
						{
							BarBrush		= downColor;
							BarBrushes[1]	= upColor;

							Draw.Text(this, "Bearish Harami Cross" + CurrentBar, false, "Bearish Harami\nCross # " + ++numPatternsFound, 0, Math.Max(High[0], High[1]), 40, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.BullishBeltHold:
					{
						#region Bullish Belt Hold
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrushes[1]			= downColor;
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
							}

							Draw.Text(this, "Bullish Belt Hold" + CurrentBar, false, "Bullish Belt\nHold # " + ++numPatternsFound, 0, Math.Min(Low[0], Low[1]), -10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);

						}
						#endregion
						break;
					}
				case ChartPattern.BullishEngulfing:
					{
						#region Bullish Engulfing
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrushes[1]			= downColor;
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
							}

							Draw.Text(this, "Bullish Engulfing" + CurrentBar, false, "Bullish\nEngulfing # " + ++numPatternsFound, 0, Math.Min(Low[0], Low[1]), -10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.BullishHarami:
					{
						#region Bullish Harami
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrushes[1]			= downColor;
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
							}

							Draw.Text(this, "Bullish Harami" + CurrentBar, false, "Bullish\nHarami # " + ++numPatternsFound, 0, Math.Min(Low[0], Low[1]), -10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.BullishHaramiCross:
					{
						#region Bullish Harami Cross
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrushes[1]			= downColor;
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
							}

							Draw.Text(this, "Bullish Harami Cross" + CurrentBar, false, "Bullish\nHarami\nCross # " + ++numPatternsFound, 0, Math.Min(Low[0], Low[1]), -10, textColor,
								TextFont, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.DarkCloudCover:
					{
						#region Dark Cloud Cover
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrushes[1]			= upColor;
								CandleOutlineBrushes[1] = downColor;
								BarBrush				= downColor;
							}

							Draw.Text(this, "Dark Cloud Cover" + CurrentBar, false, "Dark Cloud\nCover # " + ++numPatternsFound, 1, Math.Max(High[0], High[1]), 50, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.Doji:
					{
						#region Doji
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
							}

							int yOffset = Close[0] > Close[Math.Min(1, CurrentBar)] ? 20 : -20;

							Draw.Text(this, "Doji Text" + CurrentBar, true, "Doji\n# " + ++numPatternsFound, 0, (yOffset > 0 ? High[0] : Low[0]), yOffset, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.DownsideTasukiGap:
					{
						#region Downside Tasuki Gap
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
								BarBrushes[1]			= downColor;
								BarBrushes[2]			= downColor;
							}

							Draw.Text(this, "Downside Tasuki Gap" + CurrentBar, false, "Downside Tasuki\n Gap # " + ++numPatternsFound, 1, MAX(High, 3)[0], 10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.EveningStar:
					{
						#region Evening Star
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								if (Close[0] > Open[0])
								{
									BarBrush				= upColor;
									CandleOutlineBrushes[0] = downColor;
								}
								else
									BarBrush = downColor;

								if (Close[1] > Open[1])
								{
									BarBrushes[1]			= upColor;
									CandleOutlineBrushes[1] = downColor;
								}
								else
									BarBrushes[1] = downColor;

								if (Close[2] > Open[2])
								{
									BarBrushes[2]			= upColor;
									CandleOutlineBrushes[2] = downColor;
								}
								else
									BarBrushes[2]			= downColor;
							}

							Draw.Text(this, "Evening Star Text" + CurrentBar, false, "Evening\nStar # " + ++numPatternsFound, 1, MAX(High, 3)[0], 40, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.FallingThreeMethods:
					{
						#region Falling Three Methods
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush = downColor;
								BarBrushes[4] = downColor;

								int x = 1;
								while (x < 4)
								{
									if (Close[x] > Open[x])
									{
										BarBrushes[x]			= upColor;
										CandleOutlineBrushes[x] = downColor;
									}
									else
										BarBrushes[x]			= downColor;
									x++;
								}
							}

							Draw.Text(this, "Falling Three Methods" + CurrentBar, false, "Falling Three\nMethods # " + ++numPatternsFound, 2, Math.Max(High[0], High[4]), 40, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.Hammer:
					{
						#region Hammer
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								if (Close[0] > Open[0])
								{
									BarBrush				= upColor;
									CandleOutlineBrushes[0] = downColor;
								}
								else
									BarBrush				= downColor;
							}

							Draw.Text(this, "Hammer" + CurrentBar, false, "Hammer\n # " + ++numPatternsFound, 0, Low[0], -20, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.HangingMan:
					{
						#region Hanging Man
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								if (Close[0] > Open[0])
								{
									BarBrush				= upColor;
									CandleOutlineBrushes[0] = downColor;
								}
								else
									BarBrush				= downColor;
							}

							Draw.Text(this, "Hanging Man" + CurrentBar, false, "Hanging\nMan # " + ++numPatternsFound, 0, Low[0], -20, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.InvertedHammer:
					{
						#region Inverted Hammer
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								if (Close[0] > Open[0])
								{
									BarBrush				= upColor;
									CandleOutlineBrushes[0] = downColor;
								}
								else
									BarBrush				= downColor;
							}

							Draw.Text(this, "Inverted Hammer" + CurrentBar, false, "Inverted\nHammer\n # " + ++numPatternsFound, 0, Low[0] - 2 * TickSize, 20, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.MorningStar:
					{
						#region Morning Star
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								if (Close[0] > Open[0])
								{
									BarBrush				= upColor;
									CandleOutlineBrushes[0] = downColor;
								}
								else
									BarBrush				= downColor;

								if (Close[1] > Open[1])
								{
									BarBrushes[1]			= upColor;
									CandleOutlineBrushes[1] = downColor;
								}
								else
									BarBrushes[1] = downColor;


								if (Close[2] > Open[2])
								{
									BarBrushes[2]			= upColor;
									CandleOutlineBrushes[2] = downColor;
								}
								else
									BarBrushes[2]			= downColor;
							}

							Draw.Text(this, "Morning Star Text" + CurrentBar, false, "Morning\nStar # " + ++numPatternsFound, 1, MIN(Low, 3)[0], -20, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.PiercingLine:
					{
						#region Piercing Line
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								CandleOutlineBrushes[1] = downColor;
								BarBrushes[1]			= upColor;
								BarBrush				= downColor;
							}

							Draw.Text(this, "Piercing Line" + CurrentBar, false, "Piercing\nLine # " + ++numPatternsFound, 1, Low[0], -10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}

						#endregion
						break;
					}
				case ChartPattern.RisingThreeMethods:
					{
						#region Rising Three Methods
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush = upColor;
								CandleOutlineBrushes[0]	= downColor;
								BarBrushes[4]			= upColor;
								CandleOutlineBrushes[4]	= downColor;

								int x = 1;
								while (x < 4)
								{
									if (Close[x] > Open[x])
									{
										BarBrushes[x]			= upColor;
										CandleOutlineBrushes[x] = downColor;
									}
									else
										BarBrushes[x]	= downColor;
									x++;
								}
							}

							Draw.Text(this, "Rising Three Methods" + CurrentBar, false, " Rising Three\nMethods #" + ++numPatternsFound, 2, MIN(Low, 5)[0], -10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.ShootingStar:
					{
						#region Shooting Star
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
								BarBrush = downColor;

							Draw.Text(this, "Shooting Star" + CurrentBar, false, "Shooting\nStar # " + ++numPatternsFound, 0, High[0], 30, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);

						}
						#endregion
						break;
					}
				case ChartPattern.StickSandwich:
					{
						#region Stick Sandwich
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush				= downColor;
								BarBrushes[1]			= upColor;
								CandleOutlineBrushes[1] = downColor;
								BarBrushes[2]			= downColor;
							}

							Draw.Text(this, "Stick Sandwich" + CurrentBar, false, "Stick\nSandwich\n  # " + ++numPatternsFound, 1, MAX(High, 3)[0], 50, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.ThreeBlackCrows:
					{
						#region Three Black Crows
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush		= downColor;
								BarBrushes[1]	= downColor;
								BarBrushes[2]	= downColor;
							}

							Draw.Text(this, "Three Black Crows" + CurrentBar, false, "Three Black\nCrows # " + ++numPatternsFound, 1, MAX(High, 3)[0], 50, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.ThreeWhiteSoldiers:
					{
						#region Three White Soldiers
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush				= upColor;
								CandleOutlineBrushes[0] = downColor;
								BarBrushes[1]			= upColor;
								CandleOutlineBrushes[1] = downColor;
								BarBrushes[2]			= upColor;
								CandleOutlineBrushes[2] = downColor;
							}

							Draw.Text(this, "Three White Soldiers" + CurrentBar, false, "Three White\nSoldiers # " + ++numPatternsFound, 1, Low[2], -10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.UpsideGapTwoCrows:
					{
						#region Upside Gap Two Crows
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush				= downColor;
								BarBrushes[1]			= downColor;
								BarBrushes[2]			= upColor;
								CandleOutlineBrushes[2] = downColor;
							}

							Draw.Text(this, "Upside Gap Two Crows" + CurrentBar, false, "Upside Gap\nTwo Crows # " + ++numPatternsFound, 1, Math.Max(High[0], High[1]), 10, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
				case ChartPattern.UpsideTasukiGap:
					{
						#region Upside Tasuki Gap
						if (PatternFound[0] == 1)
						{
							if (ChartBars != null)
							{
								BarBrush				= downColor;
								BarBrushes[1]			= upColor;
								CandleOutlineBrushes[1] = downColor;
								BarBrushes[2]			= upColor;
								CandleOutlineBrushes[2] = downColor;
							}

							Draw.Text(this, "Upside Tasuki Gap" + CurrentBar, false, "Upside\nTasuki\nGap # " + ++numPatternsFound, 1, MIN(Low, 3)[0], -20, textColor, TextFont,
								TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
						}
						#endregion
						break;
					}
			}

			if (ShowPatternCount)
				Draw.TextFixed(this, "Count", numPatternsFound + " " + Pattern + "\n  patterns found", textBoxPosition, textColor, TextFont, Brushes.Transparent, Brushes.Transparent, 0);


			if (PatternFound[0] == 1 && ShowAlerts)
			{
				Alert("myAlert", Priority.Low, "Pattern(s) found: " + numPatternsFound + " " + Pattern + " on " + Instrument.FullName + " " + BarsPeriod.Value + " " +
				BarsPeriod.BarsPeriodType + " Chart", "Alert3.wav", 10, Brushes.DimGray, Brushes.DimGray);
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", Name, Pattern);
		}

		#region Properties
		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> PatternFound
		{
			get { return Values[0]; }
		}

		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SelectPattern", Description = "Choose a pattern to detect", GroupName = "NinjaScriptGeneral", Order = 1)]
		public ChartPattern Pattern
		{ get; set; }

		[Display(ResourceType = typeof(Custom.Resource), Name = "SendAlerts", Description = "Set true to send alert message to Alerts window", GroupName = "NinjaScriptGeneral", Order = 2)]
		public bool ShowAlerts
		{ get; set; }

		[Display(ResourceType = typeof(Custom.Resource), Name = "ShowPatternCount", Description = "Set true to display on chart the count of patterns found", GroupName = "NinjaScriptGeneral", Order = 3)]
		public bool ShowPatternCount
		{ get; set; }

		[Display(ResourceType = typeof(Custom.Resource), Name = "TextFont", Description = "select font, style, size to display on chart", GroupName = "NinjaScriptGeneral", Order = 4)]
		public Gui.Tools.SimpleFont TextFont
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "TrendStrength", Description = "Number of bars required to define a trend when a pattern requires a prevailing trend. \nA value of zero will disable trend requirement.",
		GroupName = "NinjaScriptGeneral", Order = 5)]
		public int TrendStrength
		{ get; set; }
		#endregion
	}

	public class CandleStickPatternLogic
	{
		private		bool				isInDownTrend;
		private		bool				isInUpTrend;
		private		MAX					max;
		private		MIN					min;
		private		NinjaScriptBase		ninjaScript;
		private		bool[]				prior = new bool[2];		// Check if there was any pattern in the last 2 bars. Ignore a match in case.
		private		Swing				swing;
		private		int					trendStrength;

		public CandleStickPatternLogic(NinjaScriptBase ninjaScript, int trendStrength)
		{
			this.ninjaScript	= ninjaScript;
			this.trendStrength	= trendStrength;
		}

		public bool Evaluate(ChartPattern pattern)
		{
			if (ninjaScript.CurrentBar < trendStrength || ninjaScript.CurrentBar < 2)
				return false;

			if (max == null && (pattern == ChartPattern.HangingMan || pattern == ChartPattern.InvertedHammer))
			{
				max = new Indicators.MAX();
				max.Period = trendStrength;
				try 
				{
					max.SetState(State.Configure);
				}
				catch (Exception exp)
				{
					Cbi.Log.Process(typeof(Resource), "CbiUnableToCreateInstance2", new object[] { max.Name, exp.InnerException != null ? exp.InnerException.ToString() : exp.ToString() }, Cbi.LogLevel.Error, Cbi.LogCategories.Default);
					max.SetState(State.Finalized);
				}

				max.Parent = ninjaScript;
				max.SetInput(ninjaScript.High);

				lock (ninjaScript.NinjaScripts)
					ninjaScript.NinjaScripts.Add(max);

				try
				{
					max.SetState(ninjaScript.State);
				}
				catch (Exception exp)
				{
					Cbi.Log.Process(typeof(Resource), "CbiUnableToCreateInstance2", new object[] { max.Name, exp.InnerException != null ? exp.InnerException.ToString() : exp.ToString() }, Cbi.LogLevel.Error, Cbi.LogCategories.Default);
					max.SetState(State.Finalized);
					return false;
				}
			}

			if (min == null && pattern == ChartPattern.Hammer)
			{
				min = new MIN();
				min.Period = trendStrength;
				try 
				{
					min.SetState(State.Configure);
				}
				catch (Exception exp)
				{
					Cbi.Log.Process(typeof(Resource), "CbiUnableToCreateInstance2", new object[] { min.Name, exp.InnerException != null ? exp.InnerException.ToString() : exp.ToString() }, Cbi.LogLevel.Error, Cbi.LogCategories.Default);
					min.SetState(State.Finalized);
				}

				min.Parent = ninjaScript;
				min.SetInput(ninjaScript.Low);

				lock (ninjaScript.NinjaScripts)
					ninjaScript.NinjaScripts.Add(min);

				try
				{
					min.SetState(ninjaScript.State);
				}
				catch (Exception exp)
				{
					Cbi.Log.Process(typeof(Resource), "CbiUnableToCreateInstance2", new object[] { min.Name, exp.InnerException != null ? exp.InnerException.ToString() : exp.ToString() }, Cbi.LogLevel.Error, Cbi.LogCategories.Default);
					min.SetState(State.Finalized);
					return false;
				}
			}

			if (pattern != ChartPattern.Doji
					&& pattern != ChartPattern.DownsideTasukiGap
					&& pattern != ChartPattern.EveningStar
					&& pattern != ChartPattern.FallingThreeMethods
					&& pattern != ChartPattern.MorningStar
					&& pattern != ChartPattern.RisingThreeMethods
					&& pattern != ChartPattern.StickSandwich
					&& pattern != ChartPattern.UpsideTasukiGap)
			{
				if (swing == null)
				{
					swing = new Swing();
					swing.Strength = trendStrength;
					try 
					{
						swing.SetState(State.Configure);
					}
					catch (Exception exp)
					{
						Cbi.Log.Process(typeof(Resource), "CbiUnableToCreateInstance2", new object[] { swing.Name, exp.InnerException != null ? exp.InnerException.ToString() : exp.ToString() }, Cbi.LogLevel.Error, Cbi.LogCategories.Default);
						swing.SetState(State.Finalized);
					}

					swing.Parent = ninjaScript;
					swing.SetInput(ninjaScript.Input);

					lock (ninjaScript.NinjaScripts)
						ninjaScript.NinjaScripts.Add(swing);

					try
					{
						swing.SetState(ninjaScript.State);
					}
					catch (Exception exp)
					{
						Cbi.Log.Process(typeof(Resource), "CbiUnableToCreateInstance2", new object[] { swing.Name, exp.InnerException != null ? exp.InnerException.ToString() : exp.ToString() }, Cbi.LogLevel.Error, Cbi.LogCategories.Default);
						swing.SetState(State.Finalized);
						return false;
					}
				}

				// Calculate up trend line
				int upTrendStartBarsAgo		= 0;
				int upTrendEndBarsAgo 		= 0;
				int upTrendOccurence 		= 1;

				while (ninjaScript.Low[upTrendEndBarsAgo] <= ninjaScript.Low[upTrendStartBarsAgo])
				{
					upTrendStartBarsAgo		= swing.SwingLowBar(0, upTrendOccurence + 1, ninjaScript.CurrentBar);
					upTrendEndBarsAgo		= swing.SwingLowBar(0, upTrendOccurence, ninjaScript.CurrentBar);

					if (upTrendStartBarsAgo < 0 || upTrendEndBarsAgo < 0)
						break;

					upTrendOccurence++;
				}

				// Calculate down trend line
				int downTrendStartBarsAgo	= 0;
				int downTrendEndBarsAgo 	= 0;
				int downTrendOccurence 		= 1;

				while (ninjaScript.High[downTrendEndBarsAgo] >= ninjaScript.High[downTrendStartBarsAgo])
				{

					downTrendStartBarsAgo	= swing.SwingHighBar(0, downTrendOccurence + 1, ninjaScript.CurrentBar);
					downTrendEndBarsAgo		= swing.SwingHighBar(0, downTrendOccurence, ninjaScript.CurrentBar);

					if (downTrendStartBarsAgo < 0 || downTrendEndBarsAgo < 0)
						break;

					downTrendOccurence++;
				}

				if (upTrendStartBarsAgo > 0 && upTrendEndBarsAgo > 0 && upTrendStartBarsAgo < downTrendStartBarsAgo)
				{
					isInDownTrend	= false;
					isInUpTrend		= true;
				}
				else if (downTrendStartBarsAgo > 0 && downTrendEndBarsAgo > 0 && upTrendStartBarsAgo > downTrendStartBarsAgo)
				{
					isInDownTrend	= true;
					isInUpTrend		= false;
				}
				else
				{
					isInDownTrend	= false;
					isInUpTrend		= false;
				}
			}

			bool			found	= false;
			NinjaScriptBase n		= ninjaScript;
			if (!prior[0] && !prior[1])				// no pattern found on the last 2 bars
				switch (pattern)
				{
					case ChartPattern.BearishBeltHold:		found = isInUpTrend && n.Close[1] > n.Open[1] && n.Open[0] > n.Close[1] + 5 * n.TickSize && n.Open[0] == n.High[0] && n.Close[0] < n.Open[0]; break;
					case ChartPattern.BearishEngulfing:		found = isInUpTrend && n.Close[1] > n.Open[1] && n.Close[0] < n.Open[0] && n.Open[0] > n.Close[1] && n.Close[0] < n.Open[1]; break;
					case ChartPattern.BearishHarami:		found = isInUpTrend && n.Close[0] < n.Open[0] && n.Close[1] > n.Open[1] && n.Low[0] >= n.Open[1] && n.High[0] <= n.Close[1]; break;
					case ChartPattern.BearishHaramiCross:	found = isInUpTrend && (n.High[0] <= n.Close[1]) && (n.Low[0] >= n.Open[1]) && n.Open[0] <= n.Close[1] && n.Close[0] >= n.Open[1]
																	&& ((n.Close[0] >= n.Open[0] && n.Close[0] <= n.Open[0] + n.TickSize) || (n.Close[0] <= n.Open[0] && n.Close[0] >= n.Open[0] - n.TickSize)); break;
					case ChartPattern.BullishBeltHold:		found = isInDownTrend && n.Close[1] < n.Open[1] && n.Open[0] < n.Close[1] - 5 * n.TickSize && n.Open[0] == n.Low[0] && n.Close[0] > n.Open[0]; break;
					case ChartPattern.BullishEngulfing:		found = isInDownTrend && n.Close[1] < n.Open[1] && n.Close[0] > n.Open[0] && n.Close[0] > n.Open[1] && n.Open[0] < n.Close[1]; break;
					case ChartPattern.BullishHarami:		found = isInDownTrend && n.Close[0] > n.Open[0] && n.Close[1] < n.Open[1] && n.Low[0] >= n.Close[1] && n.High[0] <= n.Open[1]; break;
					case ChartPattern.BullishHaramiCross:	found = isInDownTrend && (n.High[0] <= n.Open[1]) && (n.Low[0] >= n.Close[1]) && n.Open[0] >= n.Close[1] && n.Close[0] <= n.Open[1]
																	&& ((n.Close[0] >= n.Open[0] && n.Close[0] <= n.Open[0] + n.TickSize) || (n.Close[0] <= n.Open[0] && n.Close[0] >= n.Open[0] - n.TickSize)); break;
					case ChartPattern.DarkCloudCover:		found = isInUpTrend && n.Open[0] > n.High[1] && n.Close[1] > n.Open[1] && n.Close[0] < n.Open[0] && n.Close[0] <= n.Close[1] - (n.Close[1] - n.Open[1]) / 2 && n.Close[0] >= n.Open[1]; break;
					case ChartPattern.Doji:					found = Math.Abs(n.Close[0] - n.Open[0]) <= (n.High[0] - n.Low[0]) * 0.07; break;
					case ChartPattern.DownsideTasukiGap:	found = n.Close[2] < n.Open[2] && n.Close[1] < n.Open[1] && n.Close[0] > n.Open[0] && n.High[1] < n.Low[2]
																	&& n.Open[0] > n.Close[1] && n.Open[0] < n.Open[1] && n.Close[0] > n.Open[1] && n.Close[0] < n.Close[2]; break;
					case ChartPattern.EveningStar:			found = n.Close[2] > n.Open[2] && n.Close[1] > n.Close[2] && n.Open[0] < (Math.Abs((n.Close[1] - n.Open[1]) / 2) + n.Open[1]) && n.Close[0] < n.Open[0]; break;
					case ChartPattern.FallingThreeMethods:	found = n.Close[4] < n.Open[4] && n.Close[0] < n.Open[0] && n.Close[0] < n.Low[4] && n.High[3] < n.High[4] && n.Low[3] > n.Low[4]
																	&& n.High[2] < n.High[4] && n.Low[2] > n.Low[4] && n.High[1] < n.High[4] && n.Low[1] > n.Low[4]; break;
					case ChartPattern.Hammer:				found = isInDownTrend && min[0] == n.Low[0] && n.Low[0] < n.Open[0] - 5 * n.TickSize 
																	&& Math.Abs(n.Open[0] - n.Close[0]) < (0.10 * (n.High[0] - n.Low[0])) && (n.High[0] - n.Close[0]) < (0.25 * (n.High[0] - n.Low[0])); break;
					case ChartPattern.HangingMan:			found = isInUpTrend && max[0] == n.High[0] && n.Low[0] < n.Open[0] - 5 * n.TickSize 
																	&& Math.Abs(n.Open[0] - n.Close[0]) < (0.10 * (n.High[0] - n.Low[0])) && (n.High[0] - n.Close[0]) < (0.25 * (n.High[0] - n.Low[0])); break;
					case ChartPattern.InvertedHammer:		found = isInUpTrend && max[0] == n.High[0] && n.High[0] > n.Open[0] + 5 * n.TickSize 
																	&& Math.Abs(n.Open[0] - n.Close[0]) < (0.10 * (n.High[0] - n.Low[0])) && (n.Close[0] - n.Low[0]) < (0.25 * (n.High[0] - n.Low[0])); break;
					case ChartPattern.MorningStar:			found = n.Close[2] < n.Open[2] && n.Close[1] < n.Close[2] && n.Open[0] > (Math.Abs((n.Close[1] - n.Open[1]) / 2) + n.Open[1]) && n.Close[0] > n.Open[0]; break;
					case ChartPattern.PiercingLine:			found = isInDownTrend && n.Open[0] < n.Low[1] && n.Close[1] < n.Open[1] && n.Close[0] > n.Open[0] && n.Close[0] >= n.Close[1] + (n.Open[1] - n.Close[1]) / 2 && n.Close[0] <= n.Open[1]; break;
					case ChartPattern.RisingThreeMethods:	found = n.Close[4] > n.Open[4] && n.Close[0] > n.Open[0] && n.Close[0] > n.High[4] && n.High[3] < n.High[4] && n.Low[3] > n.Low[4]
																	&& n.High[2] < n.High[4] && n.Low[2] > n.Low[4] && n.High[1] < n.High[4] && n.Low[1] > n.Low[4]; break;
					case ChartPattern.ShootingStar:			found = isInUpTrend && n.High[0] > n.Open[0] && (n.High[0] - n.Open[0]) >= 2 * (n.Open[0] - n.Close[0]) && n.Close[0] < n.Open[0] && (n.Close[0] - n.Low[0]) <= 2 * n.TickSize; break;
					case ChartPattern.StickSandwich:		found = n.Close[2] == n.Close[0] && n.Close[2] < n.Open[2] && n.Close[1] > n.Open[1] && n.Close[0] < n.Open[0]; break;
					case ChartPattern.ThreeBlackCrows:		found = isInUpTrend && n.Close[0] < n.Open[0] && n.Close[1] < n.Open[1] && n.Close[2] < n.Open[2] && n.Close[0] < n.Close[1] && n.Close[1] < n.Close[2]
																	&& n.Open[0] < n.Open[1] && n.Open[0] > n.Close[1] && n.Open[1] < n.Open[2] && n.Open[1] > n.Close[2]; break;
					case ChartPattern.ThreeWhiteSoldiers:	found = isInDownTrend && n.Close[0] > n.Open[0] && n.Close[1] > n.Open[1] && n.Close[2] > n.Open[2] && n.Close[0] > n.Close[1] && n.Close[1] > n.Close[2]
																	&& n.Open[0] < n.Close[1] && n.Open[0] > n.Open[1] && n.Open[1] < n.Close[2] && n.Open[1] > n.Open[2]; break;
					case ChartPattern.UpsideGapTwoCrows:	found = isInUpTrend && n.Close[2] > n.Open[2] && n.Close[1] < n.Open[1] && n.Close[0] < n.Open[0] && n.Low[1] > n.High[2]
																	&& n.Close[0] > n.High[2] && n.Close[0] < n.Close[1] && n.Open[0] > n.Open[1]; break;
					case ChartPattern.UpsideTasukiGap:		found = n.Close[2] > n.Open[2] && n.Close[1] > n.Open[1] && n.Close[0] < n. Open[0] && n.Low[1] > n.High[2]
																	&& n.Open[0] < n.Close[1] && n.Open[0] > n.Open[1] && n.Close[0] < n.Open[1] && n.Close[0] > n.Close[2]; break;
				}
			prior[n.CurrentBars[0] % 2] = found;

			return found;
		}
	}
}

public enum ChartPattern
{
	BearishBeltHold,
	BearishEngulfing,
	BearishHarami,
	BearishHaramiCross,
	BullishBeltHold,
	BullishEngulfing,
	BullishHarami,
	BullishHaramiCross,
	DarkCloudCover,
	Doji,
	DownsideTasukiGap,
	EveningStar,
	FallingThreeMethods,
	Hammer,
	HangingMan,
	InvertedHammer,
	MorningStar,
	PiercingLine,
	RisingThreeMethods,
	ShootingStar,
	StickSandwich,
	ThreeBlackCrows,
	ThreeWhiteSoldiers,
	UpsideGapTwoCrows,
	UpsideTasukiGap,
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private CandlestickPattern[] cacheCandlestickPattern;
		public CandlestickPattern CandlestickPattern(ChartPattern pattern, int trendStrength)
		{
			return CandlestickPattern(Input, pattern, trendStrength);
		}

		public CandlestickPattern CandlestickPattern(ISeries<double> input, ChartPattern pattern, int trendStrength)
		{
			if (cacheCandlestickPattern != null)
				for (int idx = 0; idx < cacheCandlestickPattern.Length; idx++)
					if (cacheCandlestickPattern[idx] != null && cacheCandlestickPattern[idx].Pattern == pattern && cacheCandlestickPattern[idx].TrendStrength == trendStrength && cacheCandlestickPattern[idx].EqualsInput(input))
						return cacheCandlestickPattern[idx];
			return CacheIndicator<CandlestickPattern>(new CandlestickPattern(){ Pattern = pattern, TrendStrength = trendStrength }, input, ref cacheCandlestickPattern);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.CandlestickPattern CandlestickPattern(ChartPattern pattern, int trendStrength)
		{
			return indicator.CandlestickPattern(Input, pattern, trendStrength);
		}

		public Indicators.CandlestickPattern CandlestickPattern(ISeries<double> input , ChartPattern pattern, int trendStrength)
		{
			return indicator.CandlestickPattern(input, pattern, trendStrength);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.CandlestickPattern CandlestickPattern(ChartPattern pattern, int trendStrength)
		{
			return indicator.CandlestickPattern(Input, pattern, trendStrength);
		}

		public Indicators.CandlestickPattern CandlestickPattern(ISeries<double> input , ChartPattern pattern, int trendStrength)
		{
			return indicator.CandlestickPattern(input, pattern, trendStrength);
		}
	}
}

#endregion
