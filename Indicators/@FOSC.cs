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

//This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// The Forecast Oscillator (FOSC) is an extension of the linear regression based indicators
	/// made popular by Tushar Chande. The Forecast Oscillator plots the percentage difference
	/// between the forecast price (generated by an x-period linear regression line) and the actual
	/// price. The oscillator is above zero when the forecast price is greater than the actual price.
	/// Conversely, it's less than zero if its below. In the rare case when the forecast price and the
	///  actual price are the same, the oscillator would plot zero. Actual prices that are persistently
	///  below the forecast price suggest lower prices ahead.  Likewise, actual prices that are persistently
	///  above the forecast price suggest higher prices ahead. Short-term traders should use shorter time
	///  periods and perhaps more relaxed standards for the required length of time above or below the
	///  forecast price. Long-term traders should use longer time periods and perhaps stricter standards
	///  for the required length of time above or below the forecast price. Chande also suggests plotting
	///  a three-day moving average trigger line of the Forecast Oscillator to generate early warnings of
	/// changes in trend. When the oscillator crosses below the trigger line, lower prices are suggested.
	/// When the oscillator crosses above the trigger line, higher prices are suggested.
	/// </summary>
	public class FOSC : Indicator
	{
		private TSF	tsf;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionFOSC;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameFOSC;
				IsSuspendedWhileInactive	= true;
				IsOverlay					= false;
				Period						= 14;

				AddPlot(Brushes.DarkCyan,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameFOSC);
				AddLine(Brushes.DarkGray,	0,	NinjaTrader.Custom.Resource.NinjaScriptIndicatorZeroLine);
			}
			else if (State == State.DataLoaded)
				tsf = TSF(Inputs[0], 0, Period);
		}

		protected override void OnBarUpdate()
		{
			double input0	= Input[0];
			Value[0] 		= 100 * ((input0 - tsf[0]) / input0);
		}

		#region Properties
		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Period
		{ get; set; }
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private FOSC[] cacheFOSC;
		public FOSC FOSC(int period)
		{
			return FOSC(Input, period);
		}

		public FOSC FOSC(ISeries<double> input, int period)
		{
			if (cacheFOSC != null)
				for (int idx = 0; idx < cacheFOSC.Length; idx++)
					if (cacheFOSC[idx] != null && cacheFOSC[idx].Period == period && cacheFOSC[idx].EqualsInput(input))
						return cacheFOSC[idx];
			return CacheIndicator<FOSC>(new FOSC(){ Period = period }, input, ref cacheFOSC);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.FOSC FOSC(int period)
		{
			return indicator.FOSC(Input, period);
		}

		public Indicators.FOSC FOSC(ISeries<double> input , int period)
		{
			return indicator.FOSC(input, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.FOSC FOSC(int period)
		{
			return indicator.FOSC(Input, period);
		}

		public Indicators.FOSC FOSC(ISeries<double> input , int period)
		{
			return indicator.FOSC(input, period);
		}
	}
}

#endregion