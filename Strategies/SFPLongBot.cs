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
	public class SFPLongBot : Strategy
	{
		private int OrderSize;
		private bool SwingLowHit;
		private bool OpenPosition;
		private double Entry;
		private double TargetLevel;
		private double StopLossLevel;

		private Swing Swing1;
		

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Long any SFP bullish candles with automatic order handling.";
				Name										= "SFPLongBot";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
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
				RiskPercentage					= 3;
				TargetDistance					= 20;
				OrderSize					= 10000;
				SwingLowHit					= false;
				OpenPosition					= false;
				Entry					= 1;
				TargetLevel					= 1;
				StopLossLevel					= 1;
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Minute, 5);
			}
			else if (State == State.DataLoaded)
			{				
				Swing1				= Swing(Close, 60);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1
			|| CurrentBars[1] < 1)
			return;

			 // Set 1
			if (Low[0] < Swing1.SwingLow[1])
			{
				SwingLowHit = true;
			}
			
			 // Set 2
			if ((SwingLowHit == true)
				 && (Close[0] >= Close[1]))
			{
				EnterLong(Convert.ToInt32(OrderSize), @"LongOrder");
				Entry = GetCurrentAsk(0);
				OpenPosition = true;
				StopLossLevel = Swing1.SwingLow[1];
				TargetLevel = Entry + TargetDistance;
			}
			
			 // Set 3
			if ((OpenPosition == true)
				 // Target or Stop Loss Level Hit
				 && ((GetCurrentBid(1) >= TargetLevel)
				 || (GetCurrentAsk(1) <= StopLossLevel)))
			{
				ExitLong(Convert.ToInt32(Position.Quantity), @"LongOrder", "");
				OpenPosition = false;
				SwingLowHit = false;
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.25, double.MaxValue)]
		[Display(Name="RiskPercentage", Description="Portfolio Risk % Per Trade", Order=1, GroupName="Parameters")]
		public double RiskPercentage
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TargetDistance", Description="Target is X pips away from entry.", Order=2, GroupName="Parameters")]
		public int TargetDistance
		{ get; set; }
		#endregion

	}
}
