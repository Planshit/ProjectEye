# Project Eye

一个基于`20-20-20`规则的用眼休息提醒软件（Windows），使用C# WPF编写。

软件免安装且不污染注册表，默认每隔20分钟弹出一个覆盖全屏幕的窗口提醒你该休息了，休息结束后会发出提示音（可关闭和自定义）以保证你能快速回到工作状态。当然，如果你觉得每20分钟提醒一次过于频繁，可以在选项中更改，祝你心明眼亮。

<p align="center">
  <img alt="tipwindow" src="https://raw.githubusercontent.com/Planshit/ProjectEye/master/screenshot/tipwindow.jpg">
</p>

## 什么是20-20-20规则

即每 **20** 分钟，将注意力集中在至少 **20** 英尺（ **6** 米）远的地方 **20** 秒。遵循这个规则可以有效地缓解你的用眼疲劳，保护你的视力健康。

[参考资料：https://opto.ca/health-library/the-20-20-20-rule](https://opto.ca/health-library/the-20-20-20-rule)

## 亮点

- 全屏状态免打扰；
- 进程跳过名单设置；
- 多个扩展显示器支持（提醒窗口在所有连接的显示器中同步显示）；
- 数据统计；
- 离开监听（当检测到用户离开电脑时停止计时）；
- 深色模式

*部分功能需要自行在选项中开启才生效。*

## 下载

你可以在这里 [Releases](https://github.com/Planshit/ProjectEye/releases) 下载所有发布版本编译好的EXE文件压缩包，一般是ProjectEye.zip。随后可通过软件手动检测更新。

## 运行环境

OS:Windows7/10（已测试）

Runtime:[.NET Framework 4.5+](https://dotnet.microsoft.com/download/dotnet-framework)

## 其他

程序出现问题时建议提交issue或查看是否有可升级的版本。由于使用WPF，内存占用可能较大。