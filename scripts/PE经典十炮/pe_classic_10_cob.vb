' 阵型: PE 经典十炮
' 节奏: P3: PP-PP | PP-PP | PP-PP (15, 15, 15)
' 视频: https://www.bilibili.com/video/BV1py421i7uY

' 该脚本适用于慢速关.
' 一个单引号 ' 代表一条注释的开头, 每一行的单引号后面的字不会被按键精灵识别到.

' 导入命令库
Import "pvz.mql"

' SetScreenScale 可以设置当前脚本开发环境的屏幕分辩率，使脚本适配不同分辩率的设备.
' 作者在编写命令库时, 屏幕坐标参考的是分辨率设置为"1080x2400 DPI:420"的安卓虚拟机,
' 也就是说, 如果你使用的不是竖屏设备, 可能会很不准确.
SetScreenScale 1080, 2400 ' 不用管这条命令


' 请在括号中的第一个双引号里填入北美修改版的终端模拟器里看到的“游戏中状态入口”的地址.
' 建议英文字母用大写, “0x”前缀可加可不加.
pvz.UpdateAddr("ABCD1234", "entry")


' 选卡顺序.
' 需要手动选卡, 选好卡后启动脚本, 然后等待 3 秒后点一下“开始游戏”,
' 然后就可以开始摸鱼了.
' 本脚本仅用到咖啡豆、冰川菇和三叶草, 其他可以随便选.
Dim COFFEE_BEAN = 1 ' 咖啡豆
Dim ICE_SHROOM = 2  ' 冰川菇
Dim LILY_PAD = 3    ' 莲叶
Dim KERNEL_PULT = 4 ' 玉米投手
Dim COB_CANNON = 5  ' 玉米加农炮
Dim BLOVER = 6      ' 三叶草
Dim SQUASH = 7      ' 窝瓜
Dim PUFF_SHROOM = 8 ' 小喷菇
Dim CHERRY_BOMB = 9 ' 樱桃炸弹

' 更新炮列表.
' 炮列表很长的时候可以用换行符“_”换行.
Dim pao_list = Array(Array(3, 1), Array(4, 1), _
                     Array(3, 3), Array(4, 3), _
                     Array(3, 5), Array(4, 5), _
                     Array(2, 5), Array(5, 5), _
                     Array(1, 5), Array(6, 5))
pvz.UpdateCobList(pao_list)


' 选好卡后启动脚本, 等待 3 秒, 会自动点击“开始游戏”, 然后等待游戏开始.
' 每次打开按键精灵后的第一次运行脚本时, 这一步都要手动完成, 否则会卡住.
pvz.Sleep(300)
pvz.LetsRock()
pvz.Sleep(400)

' 主循环.
' 这里用了 For 循环, 实际上你也可以像下面这样:
' pvz.Prejudge(-190, 1)
' pvz.PP(2, 7.6, 5, 7.6)
' pvz.Prejudge(-190, 2)
' pvz.PP(2, 7.6, 5, 7.6)
' 这样子逐波写, 然后一直写到第 20 波.
For wave = 1 To 20

    ' Prejudge() 的作用是读取下一波的倒计时, 假如还有 x 厘秒(cs)刷新,
    ' Prejudge(-190, wave) 就会把 x 秒后设定为第 wave 波的刷新时间点,
    ' 然后在刷新前的第 190 秒开始本波的操作
    pvz.Prejudge(-190, wave)
    ShowMessage "第" & wave & "波" ' 用来显示现在是第几波

    ' 第 20 波冰消珊瑚
    ' 等到第 20 波才会执行这条命令(If 的意思是如果)
    ' 这里是一到刷新前的第 190 秒就放冰点冰, 冰放在第一路第一列
    If wave = 20 Then
        pvz.Card(ICE_SHROOM, 1, 1)
        pvz.Card(COFFEE_BEAN, 1, 1)
    End If ' 结束判断

    ' 主体节奏
    pvz.WaitUntil(550 - 373) ' 刷新后 177 cs
    pvz.PP(2, 7.6, 5, 7.6) ' 炸 2-7.6、5-7.6
    pvz.WaitUntil(1500 - 200 - 373) ' 刷新后 927 cs
    pvz.PP(2, 8.5, 5, 8.5)

    ' 每两波放一次三叶草.
    ' (wave Mod 2) 得到的是波数除以 2 后得到的余数.
    If (wave Mod 2) = 0 Then
        pvz.Card(BLOVER, 2, 1)
    End If

    ' 收尾波额外多炸两轮
    If wave = 9 Or wave = 19 Or wave = 20 Then
        For 2
            pvz.Sleep(750) ' 等待 750 cs
            pvz.PP(2, 8.5, 5, 8.5)
        Next
    End If

    ' 在完成每波的所有操作后等待 20 cs,
    ' 防止被下一波的 Prejudge() 卡住操作
    pvz.Sleep(20)
Next
