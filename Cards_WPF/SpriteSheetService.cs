using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Cards_WPF
{
    public class SpriteSheetService
    {
        public void CutImage(List<CardPicture> cardPicturesList)
        {
            var uriList = new List<Uri>
            {
                //new Uri("C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\Hjärter_Transp.png"),
                //new Uri("C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\Spader_Transp.png"),
                //new Uri("C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\Ruter_Transp.png"),
                //new Uri("C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\Klöver_Transp.png"),

                new Uri("C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\Hjärter_Transp.png"),
                new Uri("C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\Spader_Transp.png"),
                new Uri("C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\Ruter_Transp.png"),
                new Uri("C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\Klöver_Transp.png"),
            };

            var bitmapSources = new List<BitmapImage>();
            foreach (var address in uriList)
            {
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = address;
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();

                bitmapSources.Add(src);
            }


            var suitNumber = 1;
            foreach (var imageSource in bitmapSources)
            {
                var tempPicturesList = new List<CardPicture>();
                for (int y = 0; y < 3; y++)
                {
                    var yCoordinate = (127 + 7) * y;

                    for (int xCoordinate = 0; xCoordinate < 5; xCoordinate++)
                    {
                        if (y == 2 && xCoordinate >= 3)
                        {
                            continue;
                        }
                        if (y == 2)
                        {
                            yCoordinate--;
                        }

                        var newCardPicture = new CardPicture();
                        newCardPicture.Picture = new CroppedBitmap(imageSource, new Int32Rect(xCoordinate * 102, yCoordinate, 91, 127));
                        newCardPicture.Suit = (CardPicture.CardSuit)suitNumber;
                        tempPicturesList.Add(newCardPicture);

                        // 91 i bredd
                        // 127 i höjd
                        // 11 i bredd mellanrum
                        // 7 i höjd mellanrum

                    }
                }
                SetSuitAndRank(tempPicturesList, suitNumber, cardPicturesList);
                suitNumber++;
            }
        }

        private void SetSuitAndRank(List<CardPicture> tempPicturesList, int suitNumber, List<CardPicture> cardPicturesList)
        {
            var flyttaEss = false;
            var cardRankNumber = 2;

            foreach (var card in tempPicturesList)
            {
                if ((int)card.Suit == suitNumber)
                {
                    if (flyttaEss == false)
                    {
                        card.Rank = (CardPicture.CardRank)14;
                        flyttaEss = true;
                    }
                    card.Rank = (CardPicture.CardRank)cardRankNumber;
                    cardRankNumber++;

                    cardPicturesList.Add(card);
                }
            }
        }



        #region
        //int count = 0;

        //BitmapImage src = new BitmapImage();
        //src.BeginInit();
        //src.UriSource = new Uri(img, UriKind.Relative);
        //src.CacheOption = BitmapCacheOption.OnLoad;
        //src.EndInit();

        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 5; j++)
        //    {
        //        objImg[count++] = new CroppedBitmap(src, new Int32Rect(j * 120, i * 120, 120, 120));
        //    }
        //}
        #endregion
    }
}
