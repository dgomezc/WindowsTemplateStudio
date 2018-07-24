namespace Localization
{
    public static class HelpStrings
    {
        public static readonly string MainHelp = @"
For more information on a specific command, type HELP command-name

EXT       Extract localizable items for different cultures.
GEN       Generates Project Templates for different cultures.
VERIFY    Verify if exist localizable items for different cultures.
HELP      Provides Help information for Windows Template Studio Localization Tool.
";

        public static readonly string ExtCommand = @"
Extract localizable items for different cultures.

Localization ext -o ""original_WTS_folder"" -a ""actual_WTS_folder"" -d ""destinationDirectory""

    original_WTS_folder      - path to the folder that contains
                               old version of WTS to compare

    actual_WTS_folder        - path to the folder that contains
                               actual version of WTS to compare

    destinationDirectory     - path to the folder in which will be
                               saved all extracted items.

Example:

    Localization ext -o ""C:\\Projects\\wts_old"" - a ""C:\\Projects\\wts"" - d ""C:\\MyFolder\\Extracted""
";

        public static readonly string GenCommand = @"
Generates Project Templates for different cultures.

Localization gen -s ""sourceDirectory"" -d ""destinationDirectory""

    sourceDirectory          - path to the folder that contains
                               source files for Project Templates
                               (it's root project folder).

    destinationDirectory     - path to the folder in which will be
                               saved all localized Project
                               Templates (parent for CSharp.UWP.
                               2017.Solution directory).

Example:

    Localization gen -s ""C:\\MyFolder\\wts"" -d ""C:\\MyFolder\\Generated\\ProjectTemplates""
";

        public static readonly string VerifyCommand = @"
Verify if exist localizable items for different cultures.

Localization verify -s ""sourceDirectory""

    sourceDirectory          - path to the folder that contains
                               source files for verify
                               (it's root project folder).

Example:

    Localization verify -s ""C:\\MyFolder\\wts""

";

        public static readonly string HelpCommand = @"
Provides Help information for Windows Template Studio Localization Tool.

Localization help [command]

    command - displays help information on that command.
";
    }
}
