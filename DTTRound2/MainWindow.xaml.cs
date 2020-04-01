using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Spire.Xls;
using Microsoft.Win32;
using System.Windows.Media.Animation;

namespace DTTRound2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DTTRound2.MediaAct mediaAct = new MediaAct();
        List<string> questionList = new List<string>();
        List<string> questionImageList = new List<string>();
        List<string> answerList = new List<string>();
        List<List<Image>> allAnswerImageList = new List<List<Image>>();
        List<List<Image>> answerImageList = new List<List<Image>>();
        List<Image> obstaclesQuestionBtnImage = new List<Image>();
        List<Image> numberImage = new List<Image>();
        List<TextBlock> numberTxtBlock = new List<TextBlock>();
        string FinalHint = string.Empty;
        double time = 0;
        int maxObstacleNumber = 0;
        int currentQuestion = 0;
        int IdHint = 5;
        bool IsQuestionShown = false;
        int currentRound = 1;
        void Init()
        {
            mediaAct.Upload(TickSound, "TickSound.mp3");
            mediaAct.Upload(boxTimeImg, "BoxTime.png");
            mediaAct.Upload(ObstaclesBoxTimeImg, "BoxTime.png");
            Thread thread = new Thread(TimeEvent);
            thread.Start();
        }

        string StandardizeTime()
        {
            string result = string.Empty;
            int minute = (int)Math.Round(time / 60 - 0.5, 0);
            int second = (int)Math.Round(time - minute * 60 - 0.5, 0);
            if (minute < 10)
                result = "0";
            result += minute.ToString() + ":";
            if (second < 10)
                result += "0";
            result += second.ToString();
            if (result != txtBlockClock.Text)
                mediaAct.Play(TickSound);
            return result;
        }

        void TimeEvent()
        {
            while (true)
                if (time > 0)
                {
                    DateTime start = new DateTime();
                    start = DateTime.Now;
                    this.Dispatcher.Invoke(() =>
                    {
                        txtBlockClock.Text = StandardizeTime();
                        round2ClockTxtBlock.Text = StandardizeTime();
                    });
                    DateTime end = new DateTime();
                    end = DateTime.Now;
                    time = time - (double)(end.Ticks - start.Ticks) / 10000000;
                    IsQuestionShown = true;
                }
                else
                {
                    time = 0;
                    this.Dispatcher.Invoke(() =>
                    {
                        if(IsQuestionShown && currentRound==1)
                        {
                            HideAllGrid();
                            Round1Grid.Visibility = Visibility.Visible;
                            for (int i = 0; i < answerImageList[currentQuestion].Count; i++)
                                answerImageList[currentQuestion][i].Visibility = Visibility.Hidden;
                            if (currentQuestion != IdHint)
                            {
                                DoubleAnimation doubleAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
                                numberImage[currentQuestion].BeginAnimation(Image.OpacityProperty, doubleAnimation);
                                numberTxtBlock[currentQuestion].BeginAnimation(TextBlock.OpacityProperty, doubleAnimation);
                                for (int i = 0; i < answerImageList[currentQuestion].Count; i++)
                                {
                                    allAnswerImageList[currentQuestion][i].BeginAnimation(Image.OpacityProperty, doubleAnimation);
                                }
                            }

                            IsQuestionShown = false;
                        }
                        txtBlockClock.Text = StandardizeTime();
                        round2ClockTxtBlock.Text = StandardizeTime();
                    });
                }
        }

        public MainWindow()
        {
            InitializeComponent();
            GetQuestionFromExcel();
            Init();
        }

        void GetQuestionFromExcel()
        {
            for (int i = maxObstacleNumber; i < Math.Max(12, maxObstacleNumber); i++)
            {
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                gridAllObstacles.ColumnDefinitions.Add(column);
                ColumnDefinition column1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                gridObstaclesQuestion.ColumnDefinitions.Add(column1);
            }
            maxObstacleNumber = 12;

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "Excel(*.xlsv,*.xls,*.csv,*.xlsx)|*.xlsv;*.xls;*.csv;*.xlsx";
            while (openFile.ShowDialog()!=true)
            {
                openFile = new OpenFileDialog();
                openFile.Multiselect = true;
                openFile.Filter = "Excel(*.xlsv,*.xls,*.csv,*.xlsx)|*.xlsv;*.xls;*.csv;*.xlsx";
            }
            string fileName = openFile.FileNames[0];
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet worksheet = workbook.Worksheets[0];
            for (int i = 3; i <= worksheet.Rows.Length; i++)
            {
                for (int j = 2; j < 4; j++)
                    if (worksheet[i, j].NumberValue.ToString() != "NaN")
                        worksheet[i, j].Text = worksheet[i, j].NumberValue.ToString();
                questionList.Add(worksheet[i, 2].Text);
                SetUpObstacle(worksheet[i, 3].Text, i - 3);
            }
            IdHint = worksheet.Rows.Length - 3 + 1;
            for (int j = 2; j < 4; j++)
                if (worksheet[2, j].NumberValue.ToString() != "NaN")
                    worksheet[2, j].Text = worksheet[2, j].NumberValue.ToString();
            SetUpObstacle(worksheet[2, 3].Text, IdHint);
            questionList.Add(worksheet[2, 2].Text);
        }
        
        void MouseEvent(object sender, EventArgs e)
        {
            HideAllGrid();
            currentQuestion = int.Parse((sender as Image).Uid);
            Round1QuestionGrid.Visibility = Visibility.Visible;
            txtBlockQuestion.Text = questionList[currentQuestion];
            for (int i = 0; i < answerImageList[currentQuestion].Count; i++)
            {
                answerImageList[currentQuestion][i].Visibility = Visibility.Visible;
            }
        }

        void SetUpObstacle(string answer,int QuestionID)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Background = Brushes.Transparent;
            if (QuestionID == IdHint)
                textBlock.Text = "Từ Khóa";
            else textBlock.Text = "Câu " + (QuestionID + 1).ToString();
            textBlock.FontFamily = new FontFamily("Barlow Semi Condensed");
            textBlock.FontSize = 50;
            textBlock.Foreground = Brushes.White;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.SetValue(Grid.RowProperty,QuestionID);
            numberTxtBlock.Add(textBlock);

            Image btnImage = new Image();
            mediaAct.Upload(btnImage, "Obstacles_BoxNumberImage.png");
            btnImage.SetValue(Grid.RowProperty, QuestionID);
            btnImage.Uid = QuestionID.ToString();
            btnImage.Stretch = Stretch.Fill;
            btnImage.MouseLeftButtonDown += MouseEvent;
            numberImage.Add(btnImage);

            RowDefinition numberRow = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            gridQuestionNumber.RowDefinitions.Add(numberRow);
            gridQuestionNumber.Children.Add(btnImage);
            gridQuestionNumber.Children.Add(textBlock);

            RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            gridAllObstacles.RowDefinitions.Add(row);
            int numberOfObstalce = 0;
            for (int i = 0; i < answer.Length; i++)
                if (answer[i] != ' ')
                    numberOfObstalce++;
            for(int i=maxObstacleNumber; i<Math.Max(numberOfObstalce,maxObstacleNumber);i++)
            {
                ColumnDefinition column = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                gridAllObstacles.ColumnDefinitions.Add(column);
                ColumnDefinition column1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                gridObstaclesQuestion.ColumnDefinitions.Add(column1);
            }
            maxObstacleNumber = Math.Max(numberOfObstalce + 1, maxObstacleNumber);
            List<Image> images = new List<Image>();
            List<Image> images1 = new List<Image>();
            for (int i = 0; i < numberOfObstalce; i++)
            {
                Image image = new Image();
                mediaAct.Upload(image, "Obstacles_ObstacleImage.png");
                image.Visibility = Visibility.Visible;
                image.SetValue(Grid.RowProperty, QuestionID);
                image.SetValue(Grid.ColumnProperty, i + 1);
                gridAllObstacles.Children.Add(image);
                images.Add(image);
                Image image1 = new Image();
                mediaAct.Upload(image1, "Obstacles_ObstacleChosenImage.png");
                image1.Visibility = Visibility.Hidden;
                image1.SetValue(Grid.ColumnProperty, (maxObstacleNumber - numberOfObstalce) / 2 + i);
                gridObstaclesQuestion.Children.Add(image1);
                images1.Add(image1);
            }
            allAnswerImageList.Add(images);
            answerImageList.Add(images1);
        }

        void HideAllGrid()
        {
            Round1Grid.Visibility = Visibility.Hidden;
            Round1QuestionGrid.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Hidden;
        }

        private void QuestionStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion == IdHint)
                time = 10;
            else time = 15;
        }

        private void Round1Btn_Click(object sender, RoutedEventArgs e)
        {
            HideAllGrid();
            Round1Grid.Visibility = Visibility.Visible;
        }

        private void Round1BackToMainBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllGrid();
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            round1Btn.BeginAnimation(Button.OpacityProperty, doubleAnimation);
            MainGrid.Visibility = Visibility.Visible;
        }

        private void Round2Btn_Click(object sender, RoutedEventArgs e)
        {
            HideAllGrid();
            round2Grid.Visibility = Visibility.Visible;
            currentRound = 2;
        }

        private void Round2StartBtn_Click(object sender, RoutedEventArgs e)
        {
            time = 2400;
        }
    }
}
