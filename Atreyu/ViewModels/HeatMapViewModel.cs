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
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

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
        /// TODO The _current frame.
        /// </summary>
        private int _currentFrame;

        /// <summary>
        /// TODO The _heat map plot model.
        /// </summary>
        private PlotModel _heatMapPlotModel;

        /// <summary>
        /// TODO The _num frames.
        /// </summary>
        private int _numFrames;

        /// <summary>
        /// TODO The heat map data.
        /// </summary>
        private UimfData heatMapData;

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
            ////this._eventAggregator = eventAggregator;
            ////this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Subscribe(this.UpdateFrameNumber);
            ////this._eventAggregator.GetEvent<UimfFileLoadedEvent>().Subscribe(this.InitializeUimfData);
            ////this._eventAggregator.GetEvent<SumFramesChangedEvent>().Subscribe(this.SumFrames);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// TODO The _heat map data.
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
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        private string currentFile = "Heatmap";

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
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// TODO The initialize uimf data.
        /// </summary>
        /// <param name="file">
        /// TODO The file.
        /// </param>
        public void InitializeUimfData(string file)
        {
            // this.HeatMapData.ReadFile(file);
            this.HeatMapData = new UimfData(file) { CurrentMinBin = 0 };
            this.HeatMapData.CurrentMaxBin = this.HeatMapData.TotalBins;
            this.SetUpPlot(1);

            ////this._eventAggregator.GetEvent<UimfFileChangedEvent>().Publish(this.HeatMapData);
            this._numFrames = this.HeatMapData.Frames;

            this._currentFrame = 1;
            this.CurrentFile= Path.GetFileNameWithoutExtension(file);
            ////this._eventAggregator.GetEvent<NumberOfFramesChangedEvent>().Publish(this._numFrames);
            ////this._eventAggregator.GetEvent<MinimumNumberOfFrames>().Publish(1);


            ////this._eventAggregator.GetEvent<FrameNumberChangedEvent>().Publish(frameNumber);
        }

        /// <summary>
        /// TODO The save heatmap image.
        /// </summary>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetHeatmapImage()
        {
            var stream = new MemoryStream();
            PngExporter.Export(this.HeatMapPlotModel, stream, (int)this.HeatMapPlotModel.Width, (int)this.HeatMapPlotModel.Height, OxyColors.White);
            
            Image image = new Bitmap(stream);
            return image;
        }

        /// <summary>
        /// TODO The set up plot.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void SetUpPlot(int frameNumber)
        {
            this._currentFrame = frameNumber;
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
            // horizontalAxis.AxisChanged += OnXAxisChange;
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

            verticalAxis.AxisChanged += this.OnYAxisChange;

            this.HeatMapPlotModel.Axes.Add(verticalAxis);

            var heatMapSeries1 = new HeatMapSeries
                                     {
                                         X0 = 0, 
                                         X1 = 359, 
                                         Y0 = 0, 
                                         Y1 = this.HeatMapData.MaxBins, 
                                         Data =
                                             this.HeatMapData.ReadData(
                                                 1, 
                                                 this.HeatMapData.MaxBins, 
                                                 this._currentFrame, 
                                                 this._currentFrame, 
                                                 this.Height, 
                                                 this.Width,
                                                 0, 
                                                 359), 
                                         Interpolate = false
                                     };

            this.HeatMapPlotModel.Series.Add(heatMapSeries1);

            this.HeatMapPlotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// TODO The sum frames.
        /// </summary>
        /// <param name="sumFrames">
        /// TODO The sum frames.
        /// </param>
        public async void SumFrames(FrameRange sumFrames)
        {
            if (sumFrames == null)
            {
                return;
            }

            if (this.HeatMapPlotModel == null)
            {
                return;
            }

            if (this.HeatMapData == null)
            {
                return;
            }

            LinearAxis yAxis = this._heatMapPlotModel.Axes[2] as LinearAxis;
            var series = this._heatMapPlotModel.Series[0] as HeatMapSeries;
            var xAxis = this._heatMapPlotModel.Axes[1];
            this.HeatMapData.CurrentMinBin = (int)yAxis.ActualMinimum;
            this.HeatMapData.CurrentMaxBin = (int)yAxis.ActualMaximum;

            var startScan = (int)xAxis.ActualMinimum;
            var endScan = (int)xAxis.ActualMaximum;

            if (series != null)
            {
                await
                    Task.Run(
                        () =>
                        series.Data =
                        this.HeatMapData.ReadData(
                            this.HeatMapData.CurrentMinBin, 
                            this.HeatMapData.CurrentMaxBin, 
                            sumFrames.StartFrame, 
                            sumFrames.EndFrame, 
                            this.Height, 
                            this.Width,
                            startScan, 
                            endScan));
                series.X0 = startScan;
                series.X1 = endScan;
                series.Y0 = this.HeatMapData.CurrentMinBin;
                series.Y1 = this.HeatMapData.CurrentMaxBin;
            }

            this._heatMapPlotModel.InvalidatePlot(true);

            ////this._eventAggregator.GetEvent<XAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
            ////this._eventAggregator.GetEvent<YAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
        }

        /// <summary>
        /// TODO The update frame number.
        /// </summary>
        /// <param name="frameNumber">
        /// TODO The frame number.
        /// </param>
        public void UpdateFrameNumber(int frameNumber)
        {
            this._currentFrame = frameNumber;

            if (this.HeatMapPlotModel == null)
            {
                return;
            }

            var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
            if (series != null)
            {
                series.Data = this.HeatMapData.ReadData(
                    this.HeatMapData.CurrentMinBin, 
                    this.HeatMapData.CurrentMaxBin, 
                    this._currentFrame, 
                    this._currentFrame, 
                    this.Height, 
                    this.Width,
                    (int)this._heatMapPlotModel.Axes[1].ActualMinimum, 
                    (int)this._heatMapPlotModel.Axes[1].ActualMaximum);
                this.HeatMapPlotModel.InvalidatePlot(true);

                ////this._eventAggregator.GetEvent<XAxisChangedEvent>()
                ////    .Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
                ////this._eventAggregator.GetEvent<YAxisChangedEvent>()
                ////    .Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
            }
        }

        /// <summary>
        /// TODO The update plot new height.
        /// </summary>
        /// <param name="height">
        /// TODO The height.
        /// </param>
        /// <param name="width">
        /// TODO The Width.
        /// </param>
        public void UpdatePlotSize(int height, int width)
        {
            this.Height = height;
            this.Width = width;
            if (this.HeatMapPlotModel == null)
            {
                return;
            }

            var series = this.HeatMapPlotModel.Series[0] as HeatMapSeries;
            if (series == null)
            {
                return;
            }

            series.Data = this.HeatMapData.ReadData(
                this.HeatMapData.CurrentMinBin, 
                this.HeatMapData.CurrentMaxBin, 
                this._currentFrame, 
                this._currentFrame, 
                height,
                width,
                (int)this._heatMapPlotModel.Axes[1].ActualMinimum, 
                (int)this._heatMapPlotModel.Axes[1].ActualMaximum);
            this.HeatMapPlotModel.InvalidatePlot(true);

            ////this._eventAggregator.GetEvent<XAxisChangedEvent>()
            ////    .Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
            ////this._eventAggregator.GetEvent<YAxisChangedEvent>()
            ////    .Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The on y axis change.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        protected void OnYAxisChange(object sender, AxisChangedEventArgs e)
        {
            var series = this._heatMapPlotModel.Series[0] as HeatMapSeries;
            if (e.ChangeType == AxisChangeTypes.Reset)
            {
                this.HeatMapData.CurrentMinBin = 0;
                this.HeatMapData.CurrentMaxBin = this.HeatMapData.MaxBins;
                const int StartScan = 0;
                var endScan = this.HeatMapData.Scans - 1;
                if (series != null)
                {
                    series.Data = this.HeatMapData.ReadData(
                        this.HeatMapData.CurrentMinBin, 
                        this.HeatMapData.CurrentMaxBin, 
                        this._currentFrame, 
                        this._currentFrame, 
                        this.Height, 
                        this.Width,
                        StartScan, 
                        endScan);
                    series.X0 = StartScan;
                    series.X1 = endScan;
                    series.Y0 = this.HeatMapData.CurrentMinBin;
                    series.Y1 = this.HeatMapData.CurrentMaxBin;
                }
            }
            else
            {
                LinearAxis yAxis = sender as LinearAxis;

                var xAxis = this._heatMapPlotModel.Axes[1];
                this.HeatMapData.CurrentMinBin = (int)yAxis.ActualMinimum;
                this.HeatMapData.CurrentMaxBin = (int)yAxis.ActualMaximum;

                var startScan = (int)xAxis.ActualMinimum;
                var endScan = (int)xAxis.ActualMaximum;

                if (series != null)
                {
                    series.Data = this.HeatMapData.ReadData(
                        this.HeatMapData.CurrentMinBin, 
                        this.HeatMapData.CurrentMaxBin, 
                        this._currentFrame, 
                        this._currentFrame, 
                        this.Height,
                        this.Width,
                        startScan, 
                        endScan);
                    series.X0 = startScan;
                    series.X1 = endScan;
                    series.Y0 = this.HeatMapData.CurrentMinBin;
                    series.Y1 = this.HeatMapData.CurrentMaxBin;
                }
            }

            this._heatMapPlotModel.InvalidatePlot(true);

            ////this._eventAggregator.GetEvent<XAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[1] as LinearAxis);
            ////this._eventAggregator.GetEvent<YAxisChangedEvent>().Publish(this._heatMapPlotModel.Axes[2] as LinearAxis);
        }

        public bool AxisVisible { get; set; }

        #endregion
    }
}