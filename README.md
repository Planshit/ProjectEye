# Project Eye

[![996.icu](https://img.shields.io/badge/link-996.icu-red.svg)](https://996.icu)
[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)

😎一个基于 20-20-20 规则的用眼休息提醒软件（ windows ），使用C# WPF编写。

程序非常简洁，只有一个全屏提示窗口，和一个托盘图标。每20分钟会弹出全屏提示窗口（可跳过）提醒你该休息了，休息结束后会有提示音（可在托盘菜单关闭）。有时候可能你需要长时间不间断工作，暂时不希望Project Eye打扰到你，可以在托盘菜单选择 “不要提醒”，直到你重新取消后才会继续。

<p align="center">
  <img alt="tipwindow" src="https://raw.githubusercontent.com/Planshit/ProjectEye/master/screenshot/tipwindow.jpg">
</p>

## 20-20-20

即每 20 分钟，休息 20 秒，将注意力集中在至少 20 英尺（ 6 米）远的地方。

[参考资料：https://opto.ca/health-library/the-20-20-20-rule](https://opto.ca/health-library/the-20-20-20-rule)

## 下载和反馈

你可以在这里 [Releases](https://github.com/Planshit/ProjectEye/releases) 下载所有发布版本编译好的EXE文件（免安装），一般是ProjectEye.zip；

如果你碰到了问题或者希望添加没有的功能，请在这 [Issues](https://github.com/Planshit/ProjectEye/issues) 提交你的想法。

## 运行环境

OS:Windows7/10（已测试）

Runtime:[.NET Framework 4.0+](https://www.microsoft.com/zh-cn/download/details.aspx?id=17718)

## 开发计划

开发计划会从Issues里的提交筛选，并按提交时间顺序进行排期。在没有Issue时我可能会把自己的想法列入，所有（将）进行的计划都会更新到 [TODO](https://github.com/Planshit/ProjectEye/blob/master/todo.md) 这里。如果你有任何建议或意见请在Issues里提交，我会仔细查看并及时反馈。我希望Project Eye能成为一个很棒的健康软件。😉