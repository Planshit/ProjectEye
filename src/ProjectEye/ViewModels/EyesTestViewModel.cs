using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.ViewModels
{
    public class EyesTestViewModel : EyesTestModel
    {
        public Command ShowTestCommand { get; set; }
        public Command TestCommand { get; set; }
        public Command EndCommand { get; set; }
        public Command SaveCommand { get; set; }

        private double[] Size = { 10, 15, 20, 25, 30, 40 };
        private bool isTested = false;

        private readonly EyesTestService eyesTestService;
        public EyesTestViewModel(EyesTestService eyesTestService)
        {
            this.eyesTestService = eyesTestService;

            ShowTestCommand = new Command(new Action<object>(ShowTestCommand_Action));
            TestCommand = new Command(new Action<object>(TestCommand_Action));
            EndCommand = new Command(new Action<object>(EndCommand_Action));
            SaveCommand = new Command(new Action<object>(SaveCommand_Action));

            FontSize = Size[0];
            Score = Size.Length;

            GetData();
        }

        private void SaveCommand_Action(object obj)
        {
            
        }

        private void EndCommand_Action(object obj)
        {
            //保存分数
            eyesTestService.SetTodayData(Score);
            //结束操作
            End();
            //刷新数据
            GetData();
        }

        private void TestCommand_Action(object obj)
        {
            if (obj.ToString() == "0")
            {
                //能看到
                Test(true);
            }
            else
            {
                //看不到
                Test(false);
            }
            Next();
            if (isTested)
            {
                //测试结束
                SetScoreInfo();
                InfoVisibility = System.Windows.Visibility.Hidden;
                TestVisibility = System.Windows.Visibility.Hidden;
                ScoreVisibility = System.Windows.Visibility.Visible;
            }
        }

        private void ShowTestCommand_Action(object obj)
        {
            InfoVisibility = System.Windows.Visibility.Hidden;
            ScoreVisibility = System.Windows.Visibility.Hidden;
            TestVisibility = System.Windows.Visibility.Visible;
            isTested = false;
        }
        private void End()
        {
            //测试结束
            Index = 1;
            FontSize = Size[0];
            Score = Size.Length;


            InfoVisibility = System.Windows.Visibility.Visible;
            TestVisibility = System.Windows.Visibility.Hidden;
            ScoreVisibility = System.Windows.Visibility.Hidden;
        }
        private void GetData()
        {
            EyesData = eyesTestService.GetChartData();
            Labels = eyesTestService.GetChartLabels();
        }
        private void Next()
        {
            int i = Index - 1;
            if (i + 1 >= Size.Length)
            {
                //测试结束
                isTested = true;
            }
            else
            {
                Index++;
                FontSize = Size[Index - 1];
            }
        }
        private void Test(bool isCanSee)
        {

            if (!isTested)
            {
                if (!isCanSee)
                {
                    Score--;
                }
                else
                {
                    isTested = true;
                }
            }
        }
        private void SetScoreInfo()
        {
            switch (Score)
            {
                case 0:
                    ScoreInfo = "您的视力测试结果非常不乐观，近视程度非常严重。请注意保护眼睛。";
                    break;
                case 1:
                    ScoreInfo = "您的近视程度非常严重。请注意保护眼睛。";
                    break;
                case 2:
                    ScoreInfo = "您的近视程度很严重。请注意保护眼睛。";
                    break;
                case 3:
                    ScoreInfo = "您的近视程度严重。请注意保护眼睛。";
                    break;
                case 4:
                    ScoreInfo = "您有较为严重的近视。请注意保护眼睛。";
                    break;
                case 5:
                    ScoreInfo = "您有轻微的近视。请注意保护眼睛。";
                    break;
                case 6:
                    ScoreInfo = "您的视力水平非常健康！请注意保护眼睛。";
                    break;
            }
        }
    }
}
