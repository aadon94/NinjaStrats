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
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class TrendRider02 : Strategy
	{
		private SMA SMA1;
		private SMA SMA2;
		private SMA SMA3;
		
		private SMA SMA4;
		private SMA SMA5;
		private SMA SMA6;

        private Swing Swing1;
        private MACD MACD1;

        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"A strategy that focuses on trending markets (rather than ranging),
                                                                1D to confirm, ignore ranging by watching for 4HR MA cross tangles,
                                                                trade direction of trend when price retests sup/res. SL trails behind.";
				Name										= "TrendRider 0.2";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.UniqueEntries;
				IsExitOnSessionCloseStrategy				= false;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				AutoTrading					= false;
				FastMA					= 10;
				SlowMA					= 20;
				OrderQuantity					= 10000;
				StopLossPercent					= 1;
                LTF = 2;
                MTF = 3;
                HTF = 4;
                RFStrength = 0.05;
                SwingStrength = 12;
			}
			else if (State == State.Configure)
			{
                AddDataSeries(Data.BarsPeriodType.Minute, 15);
                AddDataSeries(Data.BarsPeriodType.Minute, 60);
                AddDataSeries(Data.BarsPeriodType.Minute, 240);
                AddDataSeries(Data.BarsPeriodType.Day, 1);
				AddDataSeries(Data.BarsPeriodType.Week, 1);
            }
            else if (State == State.DataLoaded)
			{				
				SMA1				= SMA(Closes[MTF], Convert.ToInt32(FastMA)); // 4 hour
				SMA2				= SMA(Closes[MTF], Convert.ToInt32(SlowMA)); // 4 hour
                SMA3				= SMA(Closes[HTF], Convert.ToInt32(FastMA)); // 1 day
                SMA4				= SMA(Closes[HTF], Convert.ToInt32(SlowMA)); // 1 day
				SMA5				= SMA(Closes[4], Convert.ToInt32(FastMA)); // 1 week
				SMA6				= SMA(Closes[4], Convert.ToInt32(SlowMA)); // 1 week
				SetStopLoss("", CalculationMode.Percent, 3, false);
                Swing1 = Swing(Closes[MTF], SwingStrength);
                MACD1 = MACD(Close, 12, 26, 9);
            }
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1
			|| CurrentBars[1] < 1
			|| CurrentBars[2] < 1)
			return;

            /* 
             * Long signal if:
             *     Fast MA > Slow MA on Daily AND
             *     MACD Average (Histogram) has a value of > 0.05 or < 0.05 --> This helps filter out range bound markets. AND
             *     Price is at old resistance or support
             */
            if (SMA3[0] > SMA4[0] && //Fast MA > Slow MA on Daily
                (MACD1.Avg[0] > RFStrength || MACD1.Avg[0] < (RFStrength * -1))) //MACD Average (Histogram) has a value of > 0.05 or < 0.05
            {
                if (Low[0] < Swing1.SwingLow[1] && Close[0] > Swing1.SwingLow[1]) // Low of current candle broke last swing low but the close of it was above last swing low.
                {
                    Draw.VerticalLine(this, @"TrendRider Vertical line_1", 0, Brushes.Lime, DashStyleHelper.Solid, 2);
                    if (AutoTrading)
                    {
                        EnterLong(Convert.ToInt32(OrderQuantity), @"Long");
                        ExitShort(Convert.ToInt32(Position.Quantity), @"ExitShort", @"Short");
                    }
                }
            }

            /* 
             * Short signal if:
             *     Fast MA < Slow MA on Daily AND
             *     MACD Average (Histogram) has a value of > 0.05 or < 0.05 --> This helps filter out range bound markets. AND
             *     Price is at old resistance or support
             */
            if (SMA3[0] < SMA4[0] && //Fast MA > Slow MA on Daily
                (MACD1.Avg[0] > RFStrength || MACD1.Avg[0] < (RFStrength * -1))) //MACD Average (Histogram) has a value of > 0.05 or < 0.05
            {
                if (High[0] < Swing1.SwingHigh[1] && Close[0] < Swing1.SwingHigh[1]) // High of current candle broke last swing high but the close of it was below last swing high.
                {
                    Draw.VerticalLine(this, @"TrendRider Vertical line_1", 0, Brushes.Red, DashStyleHelper.Solid, 2);
                    if (AutoTrading)
                    {
                        EnterShort(Convert.ToInt32(OrderQuantity), @"Short");
                        ExitLong(Convert.ToInt32(Position.Quantity), @"ExitLong", @"Long");
                    }
                }
            }


        }

		#region Properties
		[NinjaScriptProperty]
		[Display(Name="AutoTrading", Description="If set to true then the bot actively trades instead of just signalling.", Order=1, GroupName="Parameters")]
		public bool AutoTrading
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="FastMA", Order=2, GroupName="Parameters")]
		public int FastMA
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="SlowMA", Order=3, GroupName="Parameters")]
		public int SlowMA
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OrderQuantity", Description="Contract position size. This is only relevent if AutoTrading is enabled.", Order=4, GroupName="Parameters")]
		public int OrderQuantity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopLossPercent", Order=5, GroupName="Parameters")]
		public int StopLossPercent
		{ get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "LTF", Description = "Lowest Time Frame (e.g. 1, 2, 3)", Order = 6, GroupName = "Parameters")]
        public int LTF
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "MTF", Description = "Middle Time Frame (e.g. 15M, 60M, 240M, 1D)", Order = 7, GroupName = "Parameters")]
        public int MTF
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "HTF", Description = "Highest Time Frame (e.g. 15M, 60M, 240M, 1D)", Order = 8, GroupName = "Parameters")]
        public int HTF
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "SwingStrength", Description = "Lookback period for swings.", Order = 9, GroupName = "Parameters")]
        public int SwingStrength
        { get; set; }

        [NinjaScriptProperty]
        [Range(0.05, double.MaxValue)]
        [Display(Name = "Range Filter Strength", Description = "Strength to filter out range bound markets. Higher the number, the more filtering. E.g. 0.05 -> 0.2", Order = 10, GroupName = "Parameters")]
        public double RFStrength
        { get; set; }

        #endregion

    }
}
