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
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
    public class ChartATM : Indicator
    {
        private Account myAccount;
        private ChartScale MyChartScale;
        private int OrderQty = 0;
        NinjaTrader.Gui.Tools.AccountSelector AcSelector;


        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Enter the description for your new custom Indicator here.";
                Name = "ChartATM";
                Calculate = Calculate.OnPriceChange;
                IsOverlay = true;
                DisplayInDataBox = false;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;
            }
            else if (State == State.DataLoaded)
            {
                if (ChartControl != null)
                    ChartControl.MouseLeftButtonDown += LeftMouseDown;
                ChartControl.KeyDown += new System.Windows.Input.KeyEventHandler(LeftShiftDown);
                ChartControl.KeyUp += new System.Windows.Input.KeyEventHandler(LeftShiftUp);

                if (ChartControl != null)
                    ChartPanel.MouseMove += ChartControl_MouseMove;


                if (myAccount != null)
                {
                    myAccount.OrderUpdate += OnOrderUpdate;
                    myAccount.ExecutionUpdate += OnExecutionUpdate;
                    myAccount.PositionUpdate += OnPositionUpdate;
                }
            }
            else if (State == State.Terminated)
            {
                if (ChartControl != null)
                    ChartControl.MouseLeftButtonDown -= LeftMouseDown;
                ChartControl.KeyDown -= LeftShiftDown;
                ChartControl.KeyUp -= LeftShiftUp;

                if (ChartControl != null)
                    ChartPanel.MouseMove -= ChartControl_MouseMove;

                if (myAccount != null)
                {
                    myAccount.OrderUpdate -= OnOrderUpdate;
                    myAccount.ExecutionUpdate -= OnExecutionUpdate;
                    myAccount.PositionUpdate -= OnPositionUpdate;
                }
            }
        }

        protected override void OnBarUpdate()
        {


        }

        private void OnOrderUpdate(object sender, OrderEventArgs e)
        {
            NinjaTrader.Code.Output.Process("", PrintTo.OutputTab1);
            NinjaTrader.Code.Output.Process("Order", PrintTo.OutputTab1);
            NinjaTrader.Code.Output.Process(e.Order.ToString(), PrintTo.OutputTab1);
        }

        private void OnExecutionUpdate(object sender, ExecutionEventArgs e)
        {
            NinjaTrader.Code.Output.Process("", PrintTo.OutputTab1);
            NinjaTrader.Code.Output.Process("Execution", PrintTo.OutputTab1);
            // Output the execution
            NinjaTrader.Code.Output.Process(string.Format("Instrument: {0}, Amt: {1}, Price: {2}",
                  e.Execution.Instrument.FullName, e.Quantity, e.Price), PrintTo.OutputTab1);
        }
        private void OnPositionUpdate(object sender, PositionEventArgs e)
        {
            NinjaTrader.Code.Output.Process("", PrintTo.OutputTab1);
            NinjaTrader.Code.Output.Process("Position", PrintTo.OutputTab1);
            // Output the order
            NinjaTrader.Code.Output.Process(e.Position.ToString(), PrintTo.OutputTab1);
        }

        protected void LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {

                TriggerCustomEvent(o =>
                {

                    AcSelector = Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlAccountSelector") as NinjaTrader.Gui.Tools.AccountSelector;

                    myAccount = AcSelector.SelectedAccount;



                    int Y = ChartingExtensions.ConvertToVerticalPixels(e.GetPosition(ChartControl as IInputElement).Y, ChartControl.PresentationSource);

                    double priceClicked = MyChartScale.GetValueByY(Y);

                    Order limitOrder = null;
                    Order stopOrder = null;

                    if (priceClicked > Close[0])
                    {
                        OrderQty = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown).Value;
                        limitOrder = myAccount.CreateOrder(Instrument, OrderAction.Sell, OrderType.Limit, OrderEntry.Manual, TimeInForce.Day, OrderQty, priceClicked, 0, "", "Entry", DateTime.MaxValue, null);
                    }
                    else
                    {
                        OrderQty = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown).Value;
                        limitOrder = myAccount.CreateOrder(Instrument, OrderAction.Buy, OrderType.Limit, OrderEntry.Manual, TimeInForce.Day, OrderQty, priceClicked, 0, "", "Entry", DateTime.MaxValue, null);
                    }


                    NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(ChartControl.OwnerChart.ChartTrader.AtmStrategy.Template, limitOrder);

                }, null);
                e.Handled = true;
            }
        }

        void LeftShiftDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {

                ChartCommands.ChangeCursor.Execute(CrosshairType.Local);

            }

        }

        void LeftShiftUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {

                ChartCommands.ChangeCursor.Execute(CrosshairType.Off);

            }


        }

        void ChartControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
            base.OnRender(chartControl, chartScale);

            MyChartScale = chartScale;
        }


    }

}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private ChartATM[] cacheChartATM;
        public ChartATM ChartATM()
        {
            return ChartATM(Input);
        }

        public ChartATM ChartATM(ISeries<double> input)
        {
            if (cacheChartATM != null)
                for (int idx = 0; idx < cacheChartATM.Length; idx++)
                    if (cacheChartATM[idx] != null && cacheChartATM[idx].EqualsInput(input))
                        return cacheChartATM[idx];
            return CacheIndicator<ChartATM>(new ChartATM(), input, ref cacheChartATM);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.ChartATM ChartATM()
        {
            return indicator.ChartATM(Input);
        }

        public Indicators.ChartATM ChartATM(ISeries<double> input)
        {
            return indicator.ChartATM(input);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.ChartATM ChartATM()
        {
            return indicator.ChartATM(Input);
        }

        public Indicators.ChartATM ChartATM(ISeries<double> input)
        {
            return indicator.ChartATM(input);
        }
    }
}

#endregion
