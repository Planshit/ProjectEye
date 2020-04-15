# Project Eye

一个基于`20-20-20`规则的用眼休息提醒软件（Windows），帮助你保持健康的工作状态，追踪每天的用眼数据。

你可以设定一个提醒间隔和休息时间，每当到达提醒间隔时，会通过一个小弹窗或全屏弹窗提示该休息了，可以选择跳过或进入本次休息。选择跳过提示窗口将关闭，选择进入则开始倒计时休息。默认是以每间隔 20 分钟休息 20 秒的提醒规则。

<p align="center">
  <img alt="tipwindow" src="https://raw.githubusercontent.com/Planshit/ProjectEye/master/screenshot/tipwindow.jpg">
</p>

## 什么是20-20-20规则

即每 **20** 分钟，将注意力集中在至少 **20** 英尺（ **6** 米）远的地方 **20** 秒。遵循这个规则可以有效地缓解你的用眼疲劳，保护你的视力健康。

[参考资料：https://opto.ca/health-library/the-20-20-20-rule](https://opto.ca/health-library/the-20-20-20-rule)

## 亮点

- 全屏状态（全屏游戏、全屏看视频）免打扰；
- 进程跳过白名单设置；
- 多个扩展显示器支持（提醒窗口在所有连接的显示器中同步显示）；
- 数据统计（以SQLite方式保存在本地）；
- 离开监听（当检测到用户离开电脑时停止计时）；
- 深色模式
- 自定义（设计）全屏提示窗口

*部分功能需要自行在选项中开启才生效。*

## 下载

你可以在这里 [Releases](https://github.com/Planshit/ProjectEye/releases) 下载所有发布版本编译好的EXE文件压缩包，一般是ProjectEye.zip。随后可通过软件主程序手动检测更新升级（在选项 > 关于 > 检查更新）。

## 运行环境

OS:Windows7/10（已测试）

Runtime:[.NET Framework 4.5+](https://dotnet.microsoft.com/download/dotnet-framework)

## 其他

程序出现问题时建议提交issue或查看是否有可升级的版本。

由于使用WPF，内存占用可能较大。

程序除了在`选项 > 关于 > 检查更新`时会请求到Github的API（https://api.github.com/repos/planshit/projecteye/releases/latest）检查更新外不会有任何的后台联网行为，更不会记录、上传你的隐私信息，请放心使用。