﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedHeatmapView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for CombinedHeatmapView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Atreyu.Views
{
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Atreyu.ViewModels;

    using Falkor.Views.Atreyu;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for CombinedHeatmapView
    ///  </summary>
    public partial class CombinedHeatmapView : UserControl, IViewFor<CombinedHeatmapViewModel>
    {
        #region Fields

        /// <summary>
        /// TODO The high slider view.
        /// </summary>
        private GateSlider highSliderView;

        /// <summary>
        /// TODO The low slider view.
        /// </summary>
        private GateSlider LowSliderView;

        /// <summary>
        /// TODO The frame manipulation view.
        /// </summary>
        private FrameManipulationView frameManipulationView;

        /// <summary>
        /// TODO The heat map view.
        /// </summary>
        private HeatMapView heatMapView;

        /// <summary>
        /// TODO The mz spectra view.
        /// </summary>
        private MzSpectraView mzSpectraView;

        /// <summary>
        /// TODO The total ion chromatogram view.
        /// </summary>
        private TotalIonChromatogramView totalIonChromatogramView;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedHeatmapView"/> class.
        /// </summary>
        public CombinedHeatmapView()
        {
            this.InitializeComponent();
            this.DataContextChanged += this.CombinedHeatmapView_DataContextChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public CombinedHeatmapViewModel ViewModel { get; set; }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        object IViewFor.ViewModel
        {
            get
            {
                return this.ViewModel;
            }

            set
            {
                this.ViewModel = value as CombinedHeatmapViewModel;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// TODO The combined heatmap view_ data context changed.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private void CombinedHeatmapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ViewModel = e.NewValue as CombinedHeatmapViewModel;

            this.heatMapView = new HeatMapView(this.ViewModel.HeatMapViewModel);
            Grid.SetColumn(this.heatMapView, 1);
            Grid.SetRow(this.heatMapView, 1);
            Grid.SetColumnSpan(this.heatMapView, 2);
            this.MainGrid.Children.Add(this.heatMapView);

            this.frameManipulationView = new FrameManipulationView(this.ViewModel.FrameManipulationViewModel);
            Grid.SetColumn(this.frameManipulationView, 1);
            Grid.SetRow(this.frameManipulationView, 0);
            this.MainGrid.Children.Add(this.frameManipulationView);

            this.mzSpectraView = new MzSpectraView(this.ViewModel.MzSpectraViewModel);
            Grid.SetColumn(this.mzSpectraView, 0);
            Grid.SetRow(this.mzSpectraView, 1);
            this.MainGrid.Children.Add(this.mzSpectraView);

            this.totalIonChromatogramView = new TotalIonChromatogramView(this.ViewModel.TotalIonChromatogramViewModel);
            Grid.SetColumn(this.totalIonChromatogramView, 1);
            Grid.SetRow(this.totalIonChromatogramView, 2);
            Grid.SetColumnSpan(this.totalIonChromatogramView, 2);
            this.MainGrid.Children.Add(this.totalIonChromatogramView);

            this.LowSliderView = new GateSlider(this.ViewModel.LowValueGateSliderViewModel);
            Grid.SetRow(this.LowSliderView, 1);
            Grid.SetColumn(this.LowSliderView, 3);
            this.MainGrid.Children.Add(this.LowSliderView);

            this.highSliderView = new GateSlider(this.ViewModel.HighValueGateSliderViewModel);

            // Grid.SetRow(this.HighSliderView, 1);
            // Grid.SetColumn(this.HighSliderView, 4);
            // this.MainGrid.Children.Add(this.HighSliderView);
            this.AllowDrop = true;
            this.PreviewDrop += this.MainTabControlPreviewDragEnter;
        }

        /// <summary>
        /// TODO The load file.
        /// </summary>
        /// <param name="fileName">
        /// TODO The file name.
        /// </param>
        private async Task LoadFile(string fileName)
        {
            await this.ViewModel.InitializeUimfData(fileName);
            this.ViewModel.FrameManipulationViewModel.CurrentFrame = 1;
        }

        /// <summary>
        /// TODO The main tab control_ preview drag enter.
        /// </summary>
        /// <param name="sender">
        /// TODO The sender.
        /// </param>
        /// <param name="e">
        /// TODO The e.
        /// </param>
        private async void MainTabControlPreviewDragEnter(object sender, DragEventArgs e)
        {
            var isCorrect = true;
            string[] filenames = { };
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }

                    var info = new FileInfo(filename);

                    if (info.Extension.ToLower() != ".uimf")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }

            e.Effects = isCorrect ? DragDropEffects.All : DragDropEffects.None;

            if (isCorrect)
            {
                await this.LoadFile(filenames[0]);
            }

            e.Handled = true;
        }

        #endregion
    }
}