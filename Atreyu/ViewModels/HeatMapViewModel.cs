﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeatMapViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The heat map view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.ViewModels
{
    using System;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;

    using Atreyu.Models;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Wpf;

    using ReactiveUI;

    using HeatMapSeries = OxyPlot.Series.HeatMapSeries;
    using LinearAxis = OxyPlot.Axes.LinearAxis;
    using LinearColorAxis = OxyPlot.Axes.LinearColorAxis;

    /// <summary>
    /// TODO The heat map view model.
    /// </summary>
    [Export]
    public class HeatMapViewModel : ReactiveObject
    {
        #region Fields

        /// <summary>
        /// TODO The _heat map plot model.
        /// </summary>
        private PlotModel _heatMapPlotModel;

        /// <summary>
        /// TODO The current file.
        /// </summary>
        private string currentFile = "Heatmap";

        /// <summary>
        /// TODO The current max bin.
        /// </summary>
        private int currentMaxBin;

        /// <summary>
        /// TODO The current max scan.
        /// </summary>
        private int currentMaxScan;

        /// <summary>
        /// TODO The current min bin.
        /// </summary>
        private int currentMinBin;

        /// <summary>
        /// TODO The current min scan.
        /// </summary>
        private int currentMinScan;

        /// <summary>
        /// TODO The data array.
        /// </summary>
        private double[,] dataArray;

        /// <summary>
        /// TODO The heat map data.
        /// </summary>
        private UimfData heatMapData;

        /// <summary>
        /// TODO The height.
        /// </summary>
        private int height;

        /// <summary>
        /// TODO The high threshold.
        /// </summary>
        private double highThreshold;

        /// <summary>
        /// The backing field for <see cref="LowThreshold"/>.
        /// </summary>
        private double lowThreshold;

        /// <summary>
        /// TODO The width.
        /// </summary>
        private int width;

        #endregion

        ///// <summary>
        ///// TODO The _sum frames.
        ///// </summary>
        // private FrameRange _sumFrames;
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// TODO The event aggregator.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [ImportingConstructor]
        public HeatMapViewModel()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether axis visible.
        /// </summary>
        public bool AxisVisible { get; set; }

        /// <summary>
        /// Gets or sets the bin to mz map.
        /// </summary>
        public double[] BinToMzMap { get; set; }

        /// <summary>
        /// Gets or sets the current file.
        /// </summary>
        public string CurrentFile
        {
            get
            {
                return this.currentFile;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentFile, value);
            }
        }

        /// <summary>
        /// Gets or sets the current max bin.
        /// </summary>
        public int CurrentMaxBin
        {
            get
            {
                return this.currentMaxBin;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMaxBin, value);
            }
        }

        /// <summary>
        /// Gets or sets the current max scan.
        /// </summary>
        public int CurrentMaxScan
        {
            get
            {
                return this.currentMaxScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMaxScan, value);
            }
        }

        /// <summary>
        /// Gets or sets the current min bin.
        /// </summary>
        public int CurrentMinBin
        {
            get
            {
                return this.currentMinBin;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMinBin, value);
            }
        }

        /// <summary>
        /// Gets or sets the current min scan.
        /// </summary>
        public int CurrentMinScan
        {
            get
            {
                return this.currentMinScan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentMinScan, value);
            }
        }

        /// <summary>
        /// Gets The heat map data (<seealso cref="UimfData"/>).
        /// </summary>
        public UimfData HeatMapData
        {
            get
            {
                return this.heatMapData;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref this.heatMapData, value);
            }
        }

        /// <summary>
        /// Gets or sets the heat map plot model.
        /// </summary>
        public PlotModel HeatMapPlotModel
        {
            get
            {
                return this._heatMapPlotModel;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this._heatMapPlotModel, value);
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.height, value);
            }
        }

        /// <summary>
        /// Gets or sets the high threshold.
        /// </summary>
        public double HighThreshold
        {
            get
            {
                return this.highThreshold;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.highThreshold, value);
            }
        }

        /// <summary>
        /// Gets or sets the threshold, the value at which intensities will not be added to the map (inclusive).
        /// </summary>
        public double LowThreshold
        {
            get
            {
                return this.lowThreshold;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.lowThreshold, value);
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.width, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The get compressed data in view.
        /// </summary>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public double[,] GetCompressedDataInView()
        {
            var minScan = this.currentMinScan;

            var exportData = new double[this.dataArray.GetLength(0) + 1, this.dataArray.GetLength(1) + 1];

            // populate the scan numbers along one axis (the vertical)
            for (var x = 1; x < exportData.GetLength(0); x++)
            {
                var scan = x - 1 + minScan;
                exportData[x, 0] = scan;
            }

            // populate the m/zs on the other axis
            for (var y = 1; y < exportData.GetLength(1); y++)
            {
                var bin = y - 1;
                var mz = this.BinToMzMap[bin];
                exportData[0, y] = mz;
            }

            // fill the reast of the array with the intensity values (0,0 of the array never assigned, but defaults to "0.0")
            for (var mz = 1; mz < exportData.GetLength(1); mz++)
            {
                for (var scan = 1; scan < exportData.GetLength(0); scan++)
                {
                    exportData[scan, mz] = this.dataArray[scan - 1, mz - 1];
                }
            }

            return exportData;
        }

        /// <summary>
        /// TODO The save heat map image.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetHeatmapImage()
        {
            var stream = new MemoryStream();
            PngExporter.Export(
                this.HeatMapPlotModel, 
                stream, 
                (int)this.HeatMapPlotModel.Width, 
                (int)this.HeatMapPlotModel.Height, 
                OxyColors.White);

            Image image = new Bitmap(stream);
            return image;
        }

        /// <summary>
        /// TODO The set up plot.
        /// </summary>
        public void SetUpPlot()
        {
            this.HeatMapPlotModel = new PlotModel();

            var linearColorAxis1 = new LinearColorAxis
                                       {
                                           HighColor = OxyColors.Purple, 
                                           LowColor = OxyColors.Black, 
                                           Position = AxisPosition.Right, 
                                           Minimum = 1, 
                                           Title = "Intensity", 
                                           IsAxisVisible = this.AxisVisible
                                       };

            this.HeatMapPlotModel.Axes.Add(linearColorAxis1);

            var horizontalAxis = new LinearAxis
                                     {
                                         Position = AxisPosition.Bottom, 
                                         AbsoluteMinimum = 0, 
                                         AbsoluteMaximum = this.HeatMapData.Scans, 
                                         MinimumRange = 10, 
                                         MaximumPadding = 0, 
                                         Title = "Mobility Scans", 
                                         IsAxisVisible = this.AxisVisible
                                     };

            horizontalAxis.AxisChanged += this.PublishXAxisChange;

            this.HeatMapPlotModel.Axes.Add(horizontalAxis);

            var verticalAxis = new LinearAxis
                                   {
                                       AbsoluteMinimum = 0, 
                                       AbsoluteMaximum = this.HeatMapData.MaxBins, 
                                       MinimumRange = 10, 
                                       MaximumPadding = 0, 
                                       Title = "TOF Bins", 
                                       TickStyle = TickStyle.Inside, 
                                       AxisDistance = -2, 
                                       TextColor = OxyColors.Red, 
                                       TicklineColor = OxyColors.Red, 
                                       Layer = AxisLayer.AboveSeries, 
                                       IsAxisVisible = this.AxisVisible
                                   };

            verticalAxis.AxisChanged += this.PublishYAxisChange;

            this.HeatMapPlotModel.Axes.Add(verticalAxis);

            var heatMapSeries1 = new HeatMapSeries
                                     {
                                         X0 = 0, 
                                         X1 = this.HeatMapData.Scans, 
                                         Y0 = 0, 
                                         Y1 = this.HeatMapData.MaxBins, 
                                         Interpolate = false
                                     };

            this.HeatMapPlotModel.Series.Add(heatMapSeries1);
        }

        /// <summary>
        /// TODO The update data.
        /// </summary>
        /// <param name="framedata">
        /// TODO The framedata.
        /// </param>
        public void UpdateData(double[,] framedata)
        {
            if (framedata == null)
            {
                return;
            }

            var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
            if (series == null)
            {
                return;
            }

            this.dataArray = framedata;

            series.Data = this.dataArray;

            // scans
            series.X0 = this.currentMinScan;
            series.X1 = this.currentMaxScan;

            // bins
            series.Y0 = this.currentMinBin;
            series.Y1 = this.currentMaxBin;

            this.HeatMapPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// TODO The update reference.
        /// </summary>
        /// <param name="uimfData">
        /// TODO The uimf data.
        /// </param>
        public void UpdateReference(UimfData uimfData)
        {
            if (uimfData == null)
            {
                return;
            }

            this.heatMapData = uimfData;
            this.currentMinBin = 1;
            this.currentMaxBin = this.heatMapData.MaxBins;
            this.currentMinScan = 0;
            this.currentMaxScan = this.heatMapData.Scans;

            this.SetUpPlot();
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The publish x axis change.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected void PublishXAxisChange(object sender, AxisChangedEventArgs e)
        {
            var axis = sender as LinearAxis;
            if (axis == null)
            {
                return;
            }

            this.CurrentMinScan = (int)axis.ActualMinimum;
            this.CurrentMaxScan = (int)axis.ActualMaximum;
        }

        /// <summary>
        /// TODO The publish y axis change.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected void PublishYAxisChange(object sender, AxisChangedEventArgs e)
        {
            var axis = sender as LinearAxis;
            if (axis == null)
            {
                return;
            }

            this.CurrentMinBin = (int)axis.ActualMinimum;
            this.CurrentMaxBin = (int)axis.ActualMaximum;
        }

        #endregion
    }
}