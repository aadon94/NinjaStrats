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
	public class TrendRiderOriginal : Strategy
	{
		private SMA SMA1;
		private SMA SMA2;
		private SMA SMA3;
		
		private SMA SMA4;
		private SMA SMA5;
		private SMA SMA6;

        private Swing Swing1;

        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"A strategy that focuses on multi-timeframe MA crosses (4HR to confirm), daily and weekly to filter. Goes for big swing moves.";
				Name										= "TrendRider";
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
			}
			else if (State == State.Configure)
			{
                AddDataSeries(Data.BarsPeriodType.Minute, 60);
                AddDataSeries(Data.BarsPeriodType.Minute, 240);
                AddDataSeries(Data.BarsPeriodType.Day, 1);
				AddDataSeries(Data.BarsPeriodType.Week, 1);
            }
            else if (State == State.DataLoaded)
			{				
				SMA1				= SMA(Closes[2], Convert.ToInt32(FastMA));
				SMA2				= SMA(Closes[2], Convert.ToInt32(SlowMA));
				SMA3				= SMA(Closes[3], Convert.ToInt32(FastMA));
				SMA4				= SMA(Closes[3], Convert.ToInt32(SlowMA));
				SMA5				= SMA(Closes[4], Convert.ToInt32(FastMA));
				SMA6				= SMA(Closes[4], Convert.ToInt32(SlowMA));
				SetStopLoss("", CalculationMode.Percent, 3, false);
                Swing1 = Swing(Closes[2], 12);
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

            /* Set 1
             * Long signal if:
             *     Fast MA crosses above Slow MA on 4HR AND
             *     Fast MA > Slow MA Daily AND
             *     Fast MA > Slow MA Weekly
             */
            if ((CrossAbove(SMA1, SMA2, 1))
				 && (SMA3[0] > SMA4[0]))
				 //&& (SMA5[0] > SMA6[0]))
			{
				Draw.ArrowUp(this, @"TrendRider Arrow up_1", true, 0, 0, Brushes.Lime);
                if (AutoTrading)
                {
                    EnterLong(Convert.ToInt32(OrderQuantity), @"Long");
                    ExitShort(Convert.ToInt32(Position.Quantity), @"ExitShort", @"Short");
                }
            }

            /* Set 2
             * Short signal if:
             *     Fast MA crosses below Slow MA on 4HR AND
             *     Fast MA < Slow MA Daily AND
             *     Fast MA < Slow MA Weekly
             */
            if ((CrossBelow(SMA1, SMA2, 1))
                 && (SMA3[0] < SMA4[0]))
                 //&& (SMA5[0] < SMA6[0]))
            {
				Draw.ArrowDown(this, @"TrendRider Arrow down_1", false, 0, 0, Brushes.Red);
                if (AutoTrading)
                {
                    EnterShort(Convert.ToInt32(OrderQuantity), @"Short");
                    ExitLong(Convert.ToInt32(Position.Quantity), @"ExitLong", @"Long");
                }				
			}

            /* Set 3
             * Close Long Positions if:
             *     Fast MA crosses below Slow MA on 4HR
             *     
             * TODO:    SL hit if 1HR price closes below 4HR Swing Low
             *          if (Swing1.SwingHigh[0] > Close[0])
             */
            if (CrossBelow(SMA1, SMA2, 1))
            {
                if (AutoTrading)
                {
                    ExitLong(Convert.ToInt32(Position.Quantity), @"ExitLongMACross", @"Long");
                }
            }

            /* Set 3
             * Close Long Positions if:
             *     SL hit if 1HR price closes below 4HR Swing Low 
             */
            if (Close[0] <= Swing1.SwingLow[0])
            {
                if (AutoTrading)
                {
                    ExitLong(Convert.ToInt32(Position.Quantity), @"ExitLongSwingLHit", @"Long");
                }
            }


            /* Set 4
             * Close Short Positions if:
             *     Fast MA crosses above Slow MA on 4HR
             */
            if (CrossAbove(SMA1, SMA2, 1))
            {
                if (AutoTrading)
                {
                    ExitShort(Convert.ToInt32(Position.Quantity), @"ExitShortMACross", @"Short");
                }
            }

            /* Set 3
             * Close Short Positions if:
             *     SL hit if 1HR price closes above 4HR Swing High 
             */
            if (Close[0] >= Swing1.SwingHigh[0])
            {
                if (AutoTrading)
                {
                    ExitShort(Convert.ToInt32(Position.Quantity), @"ExitShortSwingLHit", @"Short");
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
		#endregion

	}
}
