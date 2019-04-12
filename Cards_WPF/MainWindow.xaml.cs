using Cards;
using Cards.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cards_WPF
{
    public partial class MainWindow : Window
    {
        public int NumberOfSticks { get; set; } = 6;
        public int NumberOfPlayedRounds { get; set; } = 1;
        public List<VisualStuff> VisualStuffList { get; set; }
        public List<CardPicture> CardPicturesList { get; set; } = new List<CardPicture>();
        public bool GåUppåt { get; set; } = false;
        public List<ScoreboardData> ScoreboardItemsSource { get; set; } = new List<ScoreboardData>();

        public GameService GameService { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            GameService = new GameService();

            ClickOnHumanCards();

            CreateCroppedBitmapCards(CardPicturesList);
            ClearFrontEndStuff();
        }

        private void ClearFrontEndStuff()
        {
            Label_Westn_Won.Content = "";
            Label_North_Won.Content = "";
            Label_Eastn_Won.Content = "";
            Label_Playa_Won.Content = "";
        }

        private void StartNewGame()
        {
            GameService.PresentTextInfo += Event_PresentTextInfo;
            GameService.ShowHumanStick += Event_ShowHumanStick;
            GameService.PlayPlayerCard += Event_PlayPlayerCard;
            GameService.InvalidSticksCount += Event_InvalidSticksCount;
            GameService.ShowPlayedCard += Event_ShowPlayedCard;
            GameService.InvalidPlayedCard += Event_InvalidPlayedCard;
            GameService.ResetPlayerCards += Event_ResetPlayerCards;
            GameService.UpdateCrowne += Event_UpdateCrowne;
            GameService.RoundIsFinished += Event_RoundIsFinished;

            StartGame_BackEnd();

            StartGame_FrontEnd(GameService);

        }

        private void Event_PresentTextInfo(object sender, string textFromBackend)
        {
            GameInfo_Label.Content = textFromBackend;
        }

        private void Event_RoundIsFinished(object sender, int e)
        {
            // TODO: Kolla så man inte är färdig med hela spelet
            if (GåUppåt == false)
            {
                NumberOfSticks--;
                if (NumberOfSticks == 0)
                {
                    NumberOfSticks++;
                    GåUppåt = true;
                }
            }
            if (GåUppåt == true)
            {
                NumberOfSticks++;
            }

            this.Dispatcher.Invoke(() =>
            {

                // Nollställa platsen på skärmen för de 5 korten på hand
                var listOfCards = new List<Image>
                {
                    Image_Playa0,
                    Image_Playa1,
                    Image_Playa2,
                    Image_Playa3,
                    Image_Playa4,
                };

                foreach (var card in listOfCards)
                {
                    var marg = card.Margin;
                    marg.Top = 630;
                    card.Margin = marg;
                }

                NextRound_Button.IsEnabled = true;

                Scoreboard_Datagrid.Visibility = Visibility.Visible;
                ScoreboardItemsSource.Add(LoadScoreboardData());
                Scoreboard_Datagrid.ItemsSource = null;
                Scoreboard_Datagrid.ItemsSource = ScoreboardItemsSource;

                PlayCard_Button.Visibility = Visibility.Hidden;
                PlayCard_Button.IsEnabled = false;

                if (GåUppåt == true && NumberOfSticks > 5)
                {
                    NextRound_Button.IsEnabled = false;
                    NextRound_Button.Visibility = Visibility.Hidden;

                    // TODO: END GAME!!!
                }
            });
        }

        private ScoreboardData LoadScoreboardData()
        {
            var result = new ScoreboardData
            {
                RoundNumber = GameService.NumberOfSticksThisRound,
                West = GameService.Scoreboarden.PlayerScoreList.Find(x => x.Name == "West").TotalScore,
                North = GameService.Scoreboarden.PlayerScoreList.Find(x => x.Name == "North").TotalScore,
                East = GameService.Scoreboarden.PlayerScoreList.Find(x => x.Name == "East").TotalScore,
                Player1 = GameService.Scoreboarden.PlayerScoreList.Find(x => x.Name == "Player1").TotalScore,
            };

            return result;
        }

        private void Event_UpdateCrowne(object sender, Player player)
        {
            this.Dispatcher.Invoke(() =>
            {
                Crowne_Show(player);

                Label_Westn_Won.Content = GameService.Players.Find(x => x.Name == "West").TricksWon;
                Label_North_Won.Content = GameService.Players.Find(x => x.Name == "North").TricksWon;
                Label_Eastn_Won.Content = GameService.Players.Find(x => x.Name == "East").TricksWon;
                Label_Playa_Won.Content = GameService.Players.Find(x => x.Name == "Player1").TricksWon;
            });
        }

        private void Event_ResetPlayerCards(object sender, int e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var uri = GetUriForCard("101");
                //var uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\101.jpeg");

                Image_WestnPlayed.Source = new BitmapImage(uri);
                Image_NorthPlayed.Source = new BitmapImage(uri);
                Image_EastnPlayed.Source = new BitmapImage(uri);
                Image_PlayaPlayed.Source = new BitmapImage(uri);
            });
        }

        private void Event_InvalidPlayedCard(object sender, int e)
        {
            // Human har valt ett kort som man inte får spela ut
            MessageBox.Show("That card is not a valid pick!\nPlease choose a valid card.");
        }

        private void Event_ShowPlayedCard(object sender, Card e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (sender is HumanPlayer)
                {
                    var indexOfPlayedCard = VisualStuffList.FindIndex(x => x.CardSelected == true);
                    var cardThatWasPlayed = VisualStuffList[indexOfPlayedCard];

                    this.Dispatcher.Invoke(() =>
                    {
                        var cardToHide = FindName(cardThatWasPlayed.CardName) as Image;
                        cardToHide.Visibility = Visibility.Hidden;
                        cardToHide.IsEnabled = false;
                        cardThatWasPlayed.CardSelected = false;
                    });
                }

                ShowPlayersTrickCard(GameService, sender as Player, e);
            });
        }

        private void Event_PlayPlayerCard(object sender, Card e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ShowHandText(GameService);
            });
        }

        private void Event_InvalidSticksCount(object sender, int e)
        {
            MessageBox.Show("Invalid number of tricks! Can't be the same as the number of cards this round. \nPlease change your number of tricks.");
        }

        private void Event_ShowHumanStick(object sender, int tricksCount)
        {
            if (sender is AiPlayer)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var nameOfPlayer = (Player)sender;
                    switch (nameOfPlayer.Name)
                    {
                        case "West":
                            Label_WestnTricks.Content = $"({tricksCount})";
                            break;

                        case "North":
                            Label_NorthTricks.Content = $"({tricksCount})";
                            break;

                        case "East":
                            Label_EastnTricks.Content = $"({tricksCount})";
                            break;

                        //case "Player1":
                        //    Label_PlayaTricks.Content = $"({tricksCount})";
                        //    break;

                        default:
                            break;
                    }
                });
            }
            else
            {
                // OM VI KOMMER HIT EN ANDRA GÅNG SÅ VILL VI SE TILL SÅ ATT HUMAN SER KNAPPARNA
                this.Dispatcher.Invoke(() =>
                {
                    PlayCard_Button.IsEnabled = false;
                    PlayCard_Button.Visibility = Visibility.Hidden;
                    ChooseStick_Button.IsEnabled = true;
                    ChooseStick_Button.Visibility = Visibility.Visible;

                    var numberOfTricks = VisualStuffList.Count(x => x.CardSelected == true).ToString();
                    Label_PlayaTricks.Content = $"({numberOfTricks})";
                });
            }
        }

        public void StartGame_BackEnd()
        {
            Label_Number.Content = NumberOfPlayedRounds.ToString();

            GameService.CreateRound(NumberOfSticks);

            NumberOfPlayedRounds++;
        }

        private void StartGame_FrontEnd(GameService currentGame)
        {
            ShowHandText(currentGame);

            ShowImageCards(currentGame);

            if (currentGame.ValidHumanTricksCount == true)
            {
                MessageBox.Show("Last player's tricks equals the same as the amount of tricks for the current round...");
            }
        }

        private void ShowHandText(GameService currentGame)
        {
            ListBox_West.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "West").Hand);
            ListBox_North.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "North").Hand);
            ListBox_East.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "East").Hand);
            ListBox_Player1.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "Player1").Hand);
        }

        private void ShowImageCards(GameService currentGame)
        {
            string cardNumber = "";

            for (int j = 0; j < currentGame.Players.Find(x => x.Name == "Player1").Hand.Count; j++)
            {
                cardNumber = CardImageNumber(currentGame.Players.Find(x => x.Name == "Player1").Hand.ElementAt(j));
                Uri uri = GetUriForCard(cardNumber);
                Image img = FindName("Image_Playa" + j) as Image;
                img.Source = new BitmapImage(uri);
                img.Visibility = Visibility.Visible; // Behövs för alla andra omgångar man kommer hit
                img.IsEnabled = true;

                // För att återställa de korten som man klickade bort förra omgången
                ResetHumanCards();
            }
        }

        public void ShowPlayersTrickCard(GameService currentGame, Player player = null, Card cardToDisplay = null)
        {
            var cardNumber = cardToDisplay == null ? "101" : CardImageNumber(cardToDisplay);
            var uri = GetUriForCard(cardNumber);

            switch (player.Name)
            {
                case "West":
                    {
                        Image_WestnPlayed.Source = new BitmapImage(uri);
                        break;
                    }

                case "North":
                    {
                        Image_NorthPlayed.Source = new BitmapImage(uri);
                        break;
                    }

                case "East":
                    {
                        Image_EastnPlayed.Source = new BitmapImage(uri);
                        break;
                    }

                case "Player1":
                    {
                        Image_PlayaPlayed.Source = new BitmapImage(uri);
                        break;
                    }
            }
        }

        private void StartAnotherRound_Click(object sender, RoutedEventArgs e)
        {
            NextRound_Button.IsEnabled = false;
            ClearFrontEndStuff();

            StartGame_BackEnd();
            StartGame_FrontEnd(GameService);
        }

        private string CardImageNumber(Card cardToNum)
        {
            int suit = (int)cardToNum.Suit;
            int rank = (int)cardToNum.Rank;
            string temp = rank.ToString();
            if (rank < 10)
            {
                temp = "0" + rank.ToString();
            }
            return suit.ToString() + temp;
        }

        private void ClickOnHumanCards()
        {
            VisualStuffList = new List<VisualStuff>
            {
                new VisualStuff
                {
                    CardName = "Image_Playa0",
                    CardIndex = 0,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa1",
                    CardIndex = 1,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa2",
                    CardIndex = 2,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa3",
                    CardIndex = 3,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa4",
                    CardIndex = 4,
                },
            };
        }

        private void Image_PlayaCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var cardOfSender = (Image)sender;
            var nameOfSender = cardOfSender.Name;
            var marginOfSender = cardOfSender.Margin;

            foreach (var bild in VisualStuffList)
            {
                if (bild.CardName == nameOfSender)
                {
                    if (bild.CardSelected == true)
                    {
                        marginOfSender.Top += 30;
                        cardOfSender.Margin = marginOfSender;
                        bild.CardSelected = false;
                    }
                    else
                    {
                        marginOfSender.Top -= 30;
                        cardOfSender.Margin = marginOfSender;
                        bild.CardSelected = true;
                    }

                    return;
                }
            }

            Label_PlayaTricks.Content = VisualStuffList.Count(x => x.CardSelected == true).ToString();
        }

        private void Crowne_Show(Player player)
        {
            Crown_Westn_Image.Visibility = Visibility.Hidden;
            Crown_North_Image.Visibility = Visibility.Hidden;
            Crown_Eastn_Image.Visibility = Visibility.Hidden;
            Crown_Playa_Image.Visibility = Visibility.Hidden;

            switch (player.Name)
            {
                case "West":
                    Crown_Westn_Image.Visibility = Visibility.Visible;
                    break;
                case "North":
                    Crown_North_Image.Visibility = Visibility.Visible;
                    break;
                case "East":
                    Crown_Eastn_Image.Visibility = Visibility.Visible;
                    break;
                case "Player1":
                    Crown_Playa_Image.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ChooseTricks_Click(object sender, RoutedEventArgs e)
        {
            PlayCard_Button.IsEnabled = true;
            PlayCard_Button.Visibility = Visibility.Visible;
            ChooseStick_Button.IsEnabled = false;
            ChooseStick_Button.Visibility = Visibility.Hidden;

            this.Dispatcher.Invoke(() =>
            {
                var numberOfTricks = 0;

                foreach (var humanCard in VisualStuffList)
                {
                    if (humanCard.CardSelected == true)
                    {
                        numberOfTricks++;
                    }
                }

                Label_PlayaTricks.Content = $"({numberOfTricks})";
            });

            var indexOfTricksCardsSelected = new List<int>();

            foreach (var humanCard in VisualStuffList)
            {
                if (humanCard.CardSelected)
                {
                    indexOfTricksCardsSelected.Add(humanCard.CardIndex);
                }

            }

            this.Dispatcher.Invoke(() =>
            {
                ResetHumanCards();
            });

            GameService.HumanPickedTricks(indexOfTricksCardsSelected);
        }

        private void ResetHumanCards()
        {
            foreach (var humanCard in VisualStuffList)
            {
                var nameOfCard = FindName(humanCard.CardName) as Image;
                MouseButtonEventArgs arg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                if (humanCard.CardSelected == true)
                {
                    Image_PlayaCard_MouseUp(nameOfCard, arg);
                }
            }
        }

        private void PlayTricks_Click(object sender, RoutedEventArgs e)
        {
            // Se till att det bara är ett kort som spelas
            if (VisualStuffList.Count(x => x.CardSelected == true) != 1)
            {
                MessageBox.Show("You can only pick ONE card!");
                return;
            }

            var indexOfPlayedCard = VisualStuffList.Find(x => x.CardSelected == true);
            var cardImage = FindName(indexOfPlayedCard.CardName) as Image;

            GameService.HumanPlayedCardCheckForValidity(cardImage.Source.ToString());
        }

        private void CreateCroppedBitmapCards(List<CardPicture> cardPicturesList)
        {
            var spriteSheetService = new SpriteSheetService();
            spriteSheetService.CutImage(cardPicturesList);
        }

        private void Image_Test_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var numberOfCards = CardPicturesList.Count;
            var randomCard = new Random();
            var randomNumber = randomCard.Next(0, 52);
            Image_Test.Source = CardPicturesList[randomNumber].Picture;
        }

        private void StartGame_Button_Click(object sender, RoutedEventArgs e)
        {
            NumberOfSticks--;
            StartGame_Button.IsEnabled = false;
            StartGame_Button.Visibility = Visibility.Hidden;

            StartNewGame();
        }

        private static Uri GetUriForCard(string cardNumber)
        {
            return new Uri($@"{Directory.GetCurrentDirectory()}\..\..\Graphics\{cardNumber}.jpeg");
        }
    }

    public class ScoreboardData
    {
        public int RoundNumber { get; set; }
        public int West { get; set; }
        public int North { get; set; }
        public int East { get; set; }
        public int Player1 { get; set; }
    }
}