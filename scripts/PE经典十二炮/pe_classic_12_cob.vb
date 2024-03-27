' 阵名: PE 经典十二炮
' 节奏: P6: PP | PP | PP | PP | PP | PP (6, 6, 6, 6, 6, 6)

' 该脚本仅适用于快速关.
' 启动脚本前, 需确保卡槽的最下面一格(第9格)是樱桃炸弹.

Import "pvz.mql"

' SetScreenScale 可以设置当前脚本开发环境的屏幕分辩率，使脚本适配不同分辩率的设备
' 作者在编写命令库时, 屏幕坐标参考的是分辨率设置为"1080x2400 DPI:420"的安卓虚拟机
' 也就是说, 如果你使用的不是竖屏设备, 可能会很不准确
SetScreenScale 1080, 2400


' 请在括号中的第一个双引号内填入“游戏中状态入口”的地址.
' 建议英文字母用大写, “0x”前缀可加可不加.
pvz.UpdateAddr("ABCD1234", "entry")


' 选卡顺序(需要手动选卡).
' 本脚本仅用到樱桃炸弹, 其他可以随便选.
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
                     Array(1, 5), Array(6, 5), _
                     Array(3, 7), Array(4, 7))
pvz.UpdateCobList(pao_list)


' 启动脚本后, 等待 3 秒, 自动点击“开始游戏”, 等待游戏开始.
pvz.Sleep(300)
pvz.LetsRock()
pvz.Sleep(400)

' 主循环
For wave = 1 To 20
    pvz.Prejudge(-180, wave)
    ShowMessage "第" & wave & "波"

    ' 关底炮炸珊瑚
    If wave = 20 Then
        pvz.WaitUntil(-150) ' 刷新前 150 cs
        pvz.Pao(4, 7.625)
    End If

    ' 每波预判炸
    pvz.WaitUntil(-95)
    If wave = 10 Or wave = 20 Then
        pvz.WaitUntil(-30)
    End If
    pvz.PP(2, 9, 5, 9)

    ' 旗帜波加樱桃消延迟
    If wave = 10 Then
        pvz.WaitUntil(-30 + 373 - 100)
        pvz.Card(CHERRY_BOMB, 2, 9)
    End If

    ' 收尾额外多炸两轮
    If wave = 9 Or wave = 19 Or wave = 20 Then
        For 2
            pvz.Sleep(601)
            pvz.PP(2, 9, 5, 9)
        Next
    End If

    ' 防止被下一波的 Prejudge() 卡住操作
    pvz.Sleep(20)
Next