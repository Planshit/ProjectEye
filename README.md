# Project Eye

一个基于 `20-20-20` 规则的用眼休息提醒软件（Windows），帮助你保持健康的工作状态，追踪每天的用眼情况。

你可以设定好提醒间隔时间（默认20分钟）和休息时间（默认20秒），当程序启动后开始计时，每到达一次提醒间隔时间，就会弹出一个覆盖全屏幕的窗口提示该休息了。默认情况下你可以选择 `跳过` 或 `开始休息` ，选择 `跳过` 窗口将关闭并重新开始计时，选择 `开始休息` 程序会以从设定的休息时间（秒）开始倒计时，此时你应该将视线离开屏幕眺望远方至少6米远处放松双眼，当倒计时结束程序会播放提示音通知你。

<p align="center">
  <img alt="tipwindow" src="https://raw.githubusercontent.com/Planshit/ProjectEye/master/screenshot/tipwindow.jpg">
</p>

## 什么是20-20-20规则

即每 **20** 分钟，将注意力集中在至少 **20** 英尺（ **6** 米）远的地方 **20** 秒。遵循这个规则可以有效地缓解你的用眼疲劳，保护视力健康。

[参考资料：https://opto.ca/health-library/the-20-20-20-rule](https://opto.ca/health-library/the-20-20-20-rule)

## 亮点

- 全屏状态（全屏游戏、全屏看视频）免打扰功能；
- 进程跳过白名单设置功能，在运行白名单中的程序时不弹出提醒；
- 多个扩展显示器支持；
- 数据统计，用眼时长、休息时长、跳过次数；
- 离开监听，当检测到用户离开电脑时停止计时直到回来；
- 所见即所得地自定义（设计）全屏提示窗口；
- 番茄时钟功能；

*部分功能需要自行在选项中开启才生效。*

## 下载安装

你可以在这里 [Releases](https://github.com/Planshit/ProjectEye/releases) 下载所有发布版本编译好的EXE文件压缩包，一般是ProjectEye.zip，解压后直接双击ProjectEye.exe即可运行，无需安装。

成功启动后你将在右下角的状态栏中看到 😎 图标，右键显示菜单，双击启动或关闭番茄时钟模式。

## 运行环境

OS: Windows7/10

Runtime: [.NET Framework 4.5+](https://dotnet.microsoft.com/download/dotnet-framework) 

## 其他

[帮助文档](https://littlepanda.gitbook.io/project-eye/)

##### 是否对你有帮助？

一杯☕就是维护的动力

![](https://raw.githubusercontent.com/Planshit/ProjectEye/master/donate.jpg)