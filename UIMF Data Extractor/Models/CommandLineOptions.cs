namespace UimfDataExtractor.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// The command line options.
    /// </summary>
    public class CommandLineOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to get the data from all frames, ignoring the single frame option.
        /// </summary>
        [Option('a', "allframes", HelpText = "Outputs all frames to csv instead of just the first one.")]
        public bool AllFrames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to output a bulk peak comparison file.
        /// </summary>
        [Option('b', "bulkpeakcomparison", 
            HelpText = "Outputs a file that lists all peak's location and Full Width Half Max")]
        public bool BulkPeakComparison { get; set; }

        /// <summary>
        /// Gets or sets the frame to output.
        /// </summary>
        [Option('f', "frame", HelpText = "Outputs a specific frame", Default = 1)]
        public int Frame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to get the heat map.
        /// </summary>
        [CommandLine.Option('e', "extraction types", 
            HelpText = "Specifies that you want the two-dimensional heatmap data")]
        public Extraction[] ExtractionTypes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to get ms ms data.
        /// </summary>
        [Option('s', "msms", HelpText = "Get msms data instead of ms data when fetching the XiC")]
        public bool Getmsms { get; set; }

        /// <summary>
        /// Gets or sets the input path.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Specify the directory containing the UIMFs to process")]
        public string InputPath { get; set; }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [Option('o', "output", 
            HelpText =
                "Specify the output directory. If left empty results will be written into the same directory as the input directory"
            )]
        public string OutputPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to peak find and print out information.
        /// </summary>
        [Option('p', "peakfind", 
            HelpText = "Prints out a file listing the peaks for the m/z and/or TiC based on what output is selected")]
        public bool PeakFind { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to recursively process through subdirectories or not.
        /// </summary>
        [Option('r', "recursive", HelpText = "Recurse through files in sub directories.")]
        public bool Recursive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to flood the command line with relatively useless information.
        /// </summary>
        [Option('v', "verbose", HelpText = "Print details during execution. *NotImplementedWellYet*")]
        public bool Verbose { get; set; }

        public List<XicTarget> XicTargetList { get; }

        #endregion

        public CommandLineOptions()
        {
            this.XicTargetList = new List<XicTarget>();
        }

        #region Public Methods and Operators

        /// <summary>
        /// Explains how to use the program.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetUsage()
        {
            var help = new HelpText
                           {
                               Heading =
                                   new HeadingInfo(
                                   "UIMF Data Extractor", 
                                   typeof(Program).Assembly.GetName().Version.ToString()), 
                               Copyright = new CopyrightInfo("Battelle Memorial Institute", 2016), 
                               AdditionalNewLineAfterOption = true, 
                               AddDashesToOption = true
                           };

            help.AddPreOptionsLine(string.Empty);
            help.AddPreOptionsLine("    This application batch processes Unified Ion Mobility Files (UIMF)");
            help.AddPreOptionsLine("    to output the raw data into Comma Seperated Value (csv) format");
            help.AddPreOptionsLine(string.Empty);
            help.AddPreOptionsLine("    Usage:");
            help.AddPreOptionsLine("      UIMFDataExtractor.exe -i SOURCEFOLDER is the minimum requirement to run");
            help.AddPreOptionsLine("      If you do not specifiy an output format (m/z or tic) than the ");
            help.AddPreOptionsLine("      program will simply print what files it found.");
            help.AddPreOptionsLine("      If no output directory is specified, then it will default to the same");
            help.AddPreOptionsLine("      folder as the UIMF");

            //if (this.LastParserState.Errors.Any())
            //{
            //    var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces

            //    if (!string.IsNullOrEmpty(errors))
            //    {
            //        help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
            //        help.AddPreOptionsLine(errors);
            //    }
            //}

            help.AddEnumValuesToHelpText = true;

            return help;
        }

        #endregion
    }
}