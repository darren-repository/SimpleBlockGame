using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleBlockSlider
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CNumberBlockSequenceGame simpleGame;
        private global::Windows.UI.Xaml.Controls.Button[] ctlArrayOfButtons;

        public MainPage()
        {
            this.InitializeComponent();
            //Initialize new instance of the game
            simpleGame = new CNumberBlockSequenceGame();

            simpleGame.InitializeGame(3, 3, 300, 300);

            //Set the RelativePanel alignments


            CreateGameButtons((Panel)xrpGameBlocks);

            /***** More Research Needed *******
            //After initializing components then create the game buttons
            if (CreateGameButtons() == false)
            {

                MessageDialog msgFailure = new MessageDialog("Failed to create Game Buttons!");
                msgFailure.Commands.Add(new UICommand("Ok", null, 0));
                msgFailure.DefaultCommandIndex = 0;


                var msgReturn = await msgFailure.ShowAsync();

            }
            *********************************/

        }

        private bool CreateGameButtons(Panel ctlPanel)
        {
            int iNumberButtons = 0;

            //Check to see if an array of buttons already exists, remove them from the UI panel then and delete them
            if (ctlArrayOfButtons != null)
            {
                ctlPanel.Children.Clear();

                for (int iButtonIdx = 0; iButtonIdx < ctlArrayOfButtons.Length; iButtonIdx++)
                {
                    ctlArrayOfButtons[iButtonIdx] = null;
                }

                ctlArrayOfButtons = null;
            }

            //Get the number of the Game Blocks reduced by 1 because the '0' block isn't drawn
            iNumberButtons = (simpleGame.NumberOfGameBlocks - 1);

            //Initialize Array of Game Buttons (for arranging the numbers)
            ctlArrayOfButtons = new Button[iNumberButtons];

            //Verify allocated object memory
            if (ctlArrayOfButtons == null)
            {
                return false; //failed to allocate memory
            }

            //Iterate through each newly created button to set it up for game play
            for (int iButton = 0; iButton < iNumberButtons; iButton++)
            {
                //Allocate each button
                ctlArrayOfButtons[iButton] = new Button();

                //Verify creation of Button object
                if (ctlArrayOfButtons[iButton] == null)
                {
                    //Try and force a GC on the class object
                    ctlArrayOfButtons = null;

                    //Failed to allocate, exit with failure
                    return false;
                }

                //Add a click event to each button
                ctlArrayOfButtons[iButton].Click += GameButtonClick;

                //Adjust miscellaneous settings
                ctlArrayOfButtons[iButton].Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                ctlArrayOfButtons[iButton].Foreground.Opacity = 1.0;
                ctlArrayOfButtons[iButton].Background = new SolidColorBrush(Windows.UI.Colors.Beige);
                ctlArrayOfButtons[iButton].Background.Opacity = 1.0;
                ctlArrayOfButtons[iButton].AllowFocusOnInteraction = false;
                ctlArrayOfButtons[iButton].ClickMode = ClickMode.Release;
                ctlArrayOfButtons[iButton].FocusVisualPrimaryBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                ctlArrayOfButtons[iButton].FocusVisualPrimaryBrush.Opacity = 1.0;
                ctlArrayOfButtons[iButton].FocusVisualSecondaryBrush = new SolidColorBrush(Windows.UI.Colors.Beige);
                ctlArrayOfButtons[iButton].FocusVisualSecondaryBrush.Opacity = 1.0;
                ctlArrayOfButtons[iButton].Opacity = 1.0;
                ctlArrayOfButtons[iButton].UseLayoutRounding = true;
                ctlArrayOfButtons[iButton].UseSystemFocusVisuals = false;

                ctlPanel.Children.Add(ctlArrayOfButtons[iButton]);
            }

            xrpBlockSliderGame.UseLayoutRounding = true;
            xrpGameSelection.UseLayoutRounding = true;
            xrpGameBlocks.UseLayoutRounding = true;

            return true;
        }

        private bool UpdateGameButtons()
        {
            int iBlockIdx = 0;
            int iNumberGameBlocks = 0;
            int iNumberButtons = 0;
            CGameBlock[] aGameBlocks = null;

            //Check for valid array of blocks
            if (simpleGame == null || simpleGame.IsInitialized() == false)
            {
                //Either the array of blocks is null or the number of blocks is 0
                return false;
            }

            //Verify button array is allocated
            if (ctlArrayOfButtons == null)
            {
                //The array of buttons does not exist
                return false;
            }

            iNumberGameBlocks = simpleGame.NumberOfGameBlocks;
            iNumberButtons = ctlArrayOfButtons.Length;


            if (iNumberButtons == 0 || iNumberGameBlocks == 0)
            {
                return false;
            }

            aGameBlocks = simpleGame.GameBlocks;

            //Iterate through each newly created button to set it up for game play
            for (int iButtonIdx = 0; iButtonIdx < iNumberButtons; iButtonIdx++)
            {
                //Check for '0' Block, do not draw or adjust it just iterate to the next block
                if (aGameBlocks[iBlockIdx].iContent == 0)
                {
                    iBlockIdx++;

                    if (iBlockIdx >= iNumberGameBlocks)
                    {
                        //Something major went wrong
                        return false;
                    }
                }

                //Adjust what the button caption (now the content) reads
                ctlArrayOfButtons[iButtonIdx].Content = aGameBlocks[iBlockIdx].iContent;

                //Adjust the button size
                ctlArrayOfButtons[iButtonIdx].Width = simpleGame.GameBlockSize.X;
                ctlArrayOfButtons[iButtonIdx].Height = simpleGame.GameBlockSize.Y;

                //Adjust the button location
                ctlArrayOfButtons[iButtonIdx].Margin = new Thickness(aGameBlocks[iBlockIdx].Position.X,
                                                                     aGameBlocks[iBlockIdx].Position.Y,
                                                                     0, 0);

                //Increment the block index for getting information from the next block
                iBlockIdx++;
            }


            return true;
        }


        //Whenever the relative panel object is updated, update the child control's size and position
        private void xrpBlockSliderGame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (xrpGameSelection == null || xrpGameBlocks == null)
            {
                return;
            }

            xrpGameSelection.Width = xrpBlockSliderGame.ActualWidth - 20;
            xrpGameBlocks.Width = xrpBlockSliderGame.ActualWidth - 20;
            xrpGameBlocks.Height = xrpBlockSliderGame.ActualHeight - 120;
        }

        private void xrpGameBlocks_LayoutUpdated(object sender, object e)
        {
        }

        private void xrpGameBlocks_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Validate that the xrpGameBlocks object indeed exists before attempting to adjust sizes
            if (xrpGameBlocks == null)
            {
                return;
            }

            //Update the game with the new panel dimensions
            simpleGame.AdjustGameBoardSize(new CInt2D((int)xrpGameBlocks.ActualWidth, (int)xrpGameBlocks.ActualHeight));

            //Update and repaint game buttons
            UpdateGameButtons();
        }


        //Controls movement of the buttons after they have been clicked by the user
        private void GameButtonClick(object sender, RoutedEventArgs e)
        {
            //get the sender information to determine which button was pressed
            var btnSender = sender as Button;
            int iContent = int.Parse(btnSender.Content.ToString());

            //Update the board with the appropriate move (if valid)
            simpleGame.TryMovePiece(iContent);

            //Update and repaint game buttons
            UpdateGameButtons();

            //Force a paint of the parent panel
            xrpGameBlocks.InvalidateMeasure();

        }

        private void cboColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (simpleGame != null && simpleGame.IsInitialized())
            {
                simpleGame.AdjustNumberOfCols(int.Parse(((ComboBoxItem)cboColumns.SelectedItem).Content.ToString()));
                //Setup the size and position of the game blocks
                simpleGame.AdjustGameBlocks();
                //Create new array of Game Buttons (need some kind of check to avoid exessive UI CPU usage)
                CreateGameButtons((Panel)xrpGameBlocks);
                //Adjust the UI game buttons (game blocks)
                UpdateGameButtons();
                //Force a paint of the parent panel
                xrpGameBlocks.InvalidateMeasure();
            }

        }

        private void cboRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (simpleGame != null && simpleGame.IsInitialized())
            {
                simpleGame.AdjustNumberOfRows(int.Parse(((ComboBoxItem)cboRows.SelectedItem).Content.ToString()));
                //Setup the size and position of the game blocks
                simpleGame.AdjustGameBlocks();
                //Create new array of Game Buttons (need some kind of check to avoid exessive UI CPU usage)
                CreateGameButtons((Panel)xrpGameBlocks);
                //Adjust the UI game buttons (game blocks)
                UpdateGameButtons();
                //Force a paint of the parent panel
                xrpGameBlocks.InvalidateMeasure();
            }
        }
    }
}
