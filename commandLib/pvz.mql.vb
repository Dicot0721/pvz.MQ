' 作者: 双子叶植物
' 日期: 2024-3-27
' 版本: 0.1.6


' 获取当前应用(也就是PvZ)的应用包名.
Dim pvz_apk_name = Sys.GetFront()


' 将原始地址与偏移值相加, 得到目标地址.
Function AddOffset(raw_addr, offset)

    ' 如果原始地址没有加"0x"前缀, 补加上
    If Left(raw_addr, 2) <> "0x" Then
        raw_addr = "0x" & raw_addr
    End If

    ' 先分别将原始地址和偏移值由字符串转换为长整型整数, 再相加
    Dim long_target_addr = CLng(raw_addr) + CLng(offset)

    ' 先将目标地址由长整型整数转换为转化为表示十六进制数字值的字符串, 再在字符串前面添加前缀"0x"
    Dim target_addr = Hex(long_target_addr)
    AddOffset = "0x" & target_addr
End Function


' 更新内存地址

' Dim slots_count_addr = AddOffset(AddOffset(base_addr, "0x144"), "0x24") ' 卡槽格数
Dim game_scene_addr, game_clock_addr, current_wave_addr, wave_countdown_addr, wave_init_countdown_addr, huge_wave_countdown_addr
Dim game_scene = 2

Sub UpdateAddr(base_addr, base_addr_type)

    ' 如果输入的是“游戏中状态入口”的地址
    If base_addr_type = "entry" Then
        game_scene_addr = AddOffset(base_addr, "0x56A8")          ' 场景类型
        game_clock_addr = AddOffset(base_addr, "0x56C4")          ' 游戏计时(不包括选卡停留的时间)
        current_wave_addr = AddOffset(base_addr, "0x56DC")        ' 已刷新的波数
        wave_countdown_addr = AddOffset(base_addr, "0x56F8")      ' 下一波僵尸倒计时
        wave_init_countdown_addr = AddOffset(base_addr, "0x56FC") ' 下一波僵尸倒计时初始值
        huge_wave_countdown_addr = AddOffset(base_addr, "0x5700") ' 大波僵尸刷新倒计时

    ' 如果输入的是用 GG 修改器找到的阳光值的地址
    ElseIf base_addr_type = "sun" Then
        game_scene_addr = AddOffset(base_addr, "-0x14")
        game_clock_addr = AddOffset(base_addr, "0x8")
        current_wave_addr = AddOffset(base_addr, "0x20")
        wave_countdown_addr = AddOffset(base_addr, "0x3C")
        wave_init_countdown_addr = AddOffset(base_addr, "0x40")
        huge_wave_countdown_addr = AddOffset(base_addr, "0x44")

    Else
        ShowMessage("地址类型有误, 请重新输入")
        Delay(3000)
        ExitScript
    End If

    game_scene = Sys.MemoryRead(pvz_apk_name, game_scene_addr, "i32")
End Sub


' 手动设置场景地图
Sub SetGameScene(_game_scene)
    game_scene = _game_scene
End Sub


'' 读取常用信息
'' 注意: 因为某些未知原因, 读取出来的内存数值的数值类型与按键精灵中的数值的数值类型不同, 比较时需要使用减法等操作来隐式转换.

' @返回值 (int): 游戏场景/场地/地图.
' 0: 白天, 1: 黑夜, 2: 泳池, 3: 浓雾, 4: 屋顶, 5: 月夜, 6: 蘑菇园, 7: 禅境花园, 8: 水族馆, 9: 智慧树.
Function GameScene()
    GameScene = Sys.MemoryRead(pvz_apk_name, game_scene_addr, "i32") ' "i32"代表32位有符号整型
End Function


' @返回值 (int): 内部时钟, 游戏暂停和选卡时会暂停计时.
Function GameClock()
    GameClock = Sys.MemoryRead(pvz_apk_name, game_clock_addr, "i32")
End Function


' @返回值 (int): 已刷新波数.
Function CurrentWave()
    CurrentWave = Sys.MemoryRead(pvz_apk_name, current_wave_addr, "i32")
End Function


' @返回值 (int): 下一波刷新倒计时, 触发刷新时重置为 200, 减少至 1 后刷出下一波.
Function WaveCountdown()
    WaveCountdown = Sys.MemoryRead(pvz_apk_name, wave_countdown_addr, "i32")
End Function


' @返回值 (int): 刷新倒计时初始值.
Function WaveInitCountdown()
    WaveInitCountdown = Sys.MemoryRead(pvz_apk_name, wave_init_countdown_addr, "i32")
End Function


' @返回值 (int): 大波刷新倒计时, 对于旗帜波, 刷新倒计时减少至 4 后停滞, 由该值代替减少.
Function HugeWaveCountdown()
    HugeWaveCountdown = Sys.MemoryRead(pvz_apk_name, huge_wave_countdown_addr, "i32")
End Function


'' 阻塞延时

' 等待指定时间
Sub Sleep(time_cs)
    If time_cs > 0 Then
        Delay time_cs * 10
    ElseIf time_cs < 0 Then
        ShowMessage("等待时间不能小于零")
    End If
End Sub


' 当前波次刷新时间点
Dim refresh_time_point = 0


' 用 GameDelay() 会导致上一个操作被卡住.
' 只能在战斗界面使用, 游戏暂停时计时同样暂停.
' @参数 time_cs(int): 时间, 单位 cs, 精度 1.
Sub GameDelay(time_cs)
    If time_cs > 0 Then
        Dim clock = GameClock()
        While GameClock() - clock < time_cs
            Delay(1)
        Wend
    ElseIf time_cs < 0 Then
        ShowMessage("游戏延时参数不能小于零")
    End If
End Sub


' 等待直至刷新倒计时数值达到指定值.
' 调用时需要保证上一波已经刷出. 该函数仅保留兼容旧式写法, 已不建议使用.
' @参数 time_cs(int): 倒计时数值, 单位 cs, 精度 1. 建议范围 [200, 1].
' 第一波最早 599, 旗帜波最早 750.
' @参数 wave(int): 波数. 用于内部判断刷新状况以及本波是否为旗帜波.
' @示例:
' >>> Countdown(100, 1)  # 第 1 波 100cs 预判
' >>> Countdown(55, 10)  # 第 10 波 55cs 预判
' >>> Countdown(95, wave)  # 第 wave 波 95cs 预判
Sub Countdown(time_cs, wave)
    If wave = 10 Or wave = 20 Then
        While WaveCountdown() - 5 > 0 ' 这里用 5 而不用 4, 为了支持大波 750cs 预判
            Delay(1)
        Wend
        While HugeWaveCountdown() - time_cs > 0
            Delay(1)
        Wend
    Else
        While WaveCountdown() - time_cs > 0
            Delay(1)
        Wend
    End If
End Sub


' 倒计时(大波倒计时/初始倒计时)小于等于 200(750/599) 才算激活刷新
Dim refresh_trigger = Array(599, 200, 200, 200, 200, 200, 200, 200, 200, 750, 200, 200, 200, 200, 200, 200, 200, 200, 200, 750)


' 读内存获取刷新状况, 等待直至与设定波次刷新时间点的差值达到指定值.
' 该函数须在每波操作开始时执行一次. 通常用于预判 (在设定波次刷新前调用), 也可以在设定波次刷新之后调用.
' @参数 time_relative_cs(int): 与刷新时间点的相对时间, 单位 cs, 精度 1. 建议范围 [-200, 400].
' 第一波最早 -599, 旗帜波最早 -750. 为了方便可统一给每波设置类似 -180 等数值.
' 因为脚本语言的精度问题, 设置成 -599/-750/-200 等过早的边界值时可能会因为实际达到时间已经超过该值而引起报错.
' @参数 wave(int): 波数. 用于内部判断刷新状况以及本波是否为旗帜波.
' @示例:
' >>> Prejudge(-100, wave)  # 第 wave 波刷新前 100cs 预判
' >>> Prejudge(-150, 20)  # 第 20 波预判炮炸珊瑚时机
' >>> Prejudge(300, wave)  # 第 wave 波刷新后 300cs
' >>> Prejudge(900 - 200 - 373, wave)  # 第 wave 波 900cs 波长激活炸时机
Sub Prejudge(time_relative_cs, wave)

    ' 设定波次的刷新时间点
    ' Dim refresh_time_point

    Dim _game_clock, _current_wave, _wave_countdown, _wave_init_countdown, _huge_wave_countdown
    Dim time_to_wait
    _current_wave = CurrentWave()

    ' 设定波次还未刷出
    If _current_wave - wave < 0 Then

        ' 等待设定预判波次的上一波刷出
        If _current_wave - (wave - 1) < 0 Then
            While CurrentWave() - (wave - 1) < 0
                Delay(1)
            Wend
        End If

        ' 等到本波触发刷新
        ' 这里原本想直接调用 Countdown() 的, 但是这样写的话按键精灵会报错
        If wave = 10 Or wave = 20 Then
            While WaveCountdown() - 5 > 0 ' 这里用 5 而不用 4, 为了支持大波 750cs 预判
                Delay(1)
            Wend
            While HugeWaveCountdown() - refresh_trigger(wave - 1) > 0
                Delay(1)
            Wend
        Else
            While WaveCountdown() - refresh_trigger(wave - 1) > 0
                Delay(1)
            Wend
        End If

        ' 计算实际倒计时数值
        _wave_countdown = WaveCountdown()
        _huge_wave_countdown = HugeWaveCountdown()
        Dim countdown
        If wave = 10 Or wave = 20 Then
            If (_wave_countdown - 4 = 0) Or (_wave_countdown - 5 = 0) Then
                countdown = _huge_wave_countdown
            Else
                countdown = _wave_countdown - 5 + 750
            End If
        Else
            countdown = _wave_countdown
        End If

        ' 计算刷新时间点(倒计时变为下一波初始值时)的时钟数值
        _game_clock = GameClock()
        refresh_time_point = _game_clock + countdown

        ' 等待 目标相对时间 和 当前相对时间(即倒计时数值负值) 的差值
        time_to_wait = time_relative_cs + countdown
        If time_to_wait >= 0 Then
            Call Sleep(time_to_wait)
        Else
            ShowMessage("第" & wave & "波设定时间" & time_relative_cs & "已经过去, 当前相对时间" & (time_relative_cs - time_to_wait))
        End If

    ' 设定波次已经刷出
    ElseIf _current_wave - wave = 0 Then

        ' 获取当前时钟/倒计时数值/倒计时初始数值
        _game_clock = GameClock()
        _wave_countdown = WaveCountdown()
        _wave_init_countdown = WaveInitCountdown()

        If _wave_countdown - 200 <= 0 Then
            ShowMessage("设定波次" & wave & "的下一波即将刷新, 请调整脚本写法")
        End If

        ' 计算刷新时间点(倒计时变为下一波初始值时)的时钟数值
        refresh_time_point = _game_clock - (_wave_init_countdown - _wave_countdown)

        ' 等到设定时间
        time_to_wait = time_relative_cs - (_wave_init_countdown - _wave_countdown)
        If time_to_wait >= 0 Then
            Call Sleep(time_to_wait)
        Else
            ShowMessage("第" & wave & "波设定时间" & time_relative_cs & "已经过去, 当前相对时间" & (time_relative_cs - time_to_wait))
        End If

    ' 设定波次的下一波已经刷出
    Else
        ShowMessage("设定波次" & wave & "的下一波已经刷新, 请调整脚本写法")
    End If
End Sub


' 等待直至当前时间戳与本波刷新时间点的差值达到指定值.
' 该函数需要配合 Prejudge() 使用. 多个 WaitUntil() 连用时注意调用顺序必须符合时间先后顺序.
' @参数 time_relative_cs(int): 相对时间, 单位 cs, 精度 1. 建议范围 [-200, 2300].
' @示例:
' >>> WaitUntil(-95)  # 刷新前 95cs
' >>> WaitUntil(180)  # 刷新后 180cs
' >>> WaitUntil(-150)  # 炮炸珊瑚可用时机
' >>> WaitUntil(444 - 373)  # 444cs 生效炮发射时机
' >>> WaitUntil(601 + 20 - 298)  # 加速波下一波 20cs 预判冰点咖啡豆时机
' >>> WaitUntil(601 + 5 - 100 - 320)  # 加速波下一波 5cs 预判冰复制冰种植时机
' >>> WaitUntil(1200 - 200 - 373)  # 1200cs 波长激活炸时机
' >>> WaitUntil(4500 - 5)  # 收尾波拖满时红字出现时机
' >>> WaitUntil(5500)  # 最后一大波白字出现时机
Sub WaitUntil(time_relative_cs)
    Dim time_to_wait = time_relative_cs - (GameClock() - refresh_time_point)
    If time_to_wait >= 0 Then
        Call Sleep(time_to_wait)
    Else
        ShowMessage("设定时间" & time_relative_cs & "已经过去, 当前相对时间" & (time_relative_cs - time_to_wait))
    End If
End Sub


' 坐标转换
Function RowColToGrid(row, col)
    Dim x, y

    If (game_scene - 2 = 0) Or (game_scene - 3 = 0) Then
        x = 1080 - 99 - (153 * row)
    ElseIf (game_scene - 4 = 0) Or (game_scene - 5 = 0) Then
        If col >= 6 Then
            x = 1080 - 81 - (153 * row)
        Else
            x = 1080 - 81 - (153 * row) - (36 * (6 - col))
        End If
    Else
        x = 1080 - 72 - (180 * row)
    End If

    y = 266.7 + (213.3 * col)

    RowColToGrid = Array(x, y)
End Function


'' 点击

' 相当于按键精灵自带的“Tap(x, y)”命令
Sub TapScene(x, y)
    TouchDownEvent(x, y, 1)
    TouchUpEvent(1)
End Sub

' 点击 "Let's Rock" 按钮
Sub LetsRock()
    TouchDownEvent(72, 905, 1)
    Delay(20)
    TouchUpEvent(1)
    Delay(20)
End Sub

' 点击卡片
Sub TapSeed(index)
    Call TapScene(1136 - (118 * index), 130)
End Sub

' 点击铲子
Sub TapShovel()
    Call TapScene(1032, 842)
End Sub

' 点击场上格点
Sub TapGrid(row, col)
    Dim grid = RowColToGrid(row, col)
    Call TapScene(grid(0), grid(1))
End Sub

' 用卡
Sub Card(index, row, col)
    Call TapSeed(index)
    Call TapGrid(row, col)
End Sub

' 用铲
Sub Shovel(row, col)
    Call TapShovel()
    Call TapGrid(row, col)
End Sub


'' 用炮

' 更新炮列表

Dim cob_list()
Dim cob_count = 0
Dim cob_index = 0

Sub UpdateCobList(_cob_list)
    cob_list = _cob_list
    cob_count = UBound(cob_list) + 1 ' UBound() 用来获取数组最大下标
    cob_index = 0
End Sub


' 发射一门炮

' 直接发炮 (发射位于指定位置的炮)
Sub RawPao(cob_row, cob_col, fall_row, fall_col)
    Dim cob_grid = RowColToGrid(cob_row, cob_col)
    Dim fall_grid = RowColToGrid(fall_row, fall_col)
    For 3 ' 为了避免被阳光钱币挡住, 重复 3 次
        TouchDownEvent(cob_grid(0), cob_grid(1), 1)
        TouchMoveEvent(fall_grid(0), fall_grid(1), 1)
        TouchUpEvent(1)
    Next
End Sub

' 炮列表发炮 (按顺序发射炮列表中的炮)
Sub Pao(row, col)
    Dim cob_grid = cob_list(cob_index)
    Call RawPao(cob_grid(0), cob_grid(1), row, col)
    cob_index = (cob_index + 1) Mod cob_count
End Sub


' 同时发射两门炮

Sub RawPP(cob1_row, cob1_col, cob2_row, cob2_col, fall1_row, fall1_col, fall2_row, fall2_col)
    Dim cob1_grid = RowColToGrid(cob1_row, cob1_col)
    Dim cob2_grid = RowColToGrid(cob2_row, cob2_col)
    Dim fall1_grid = RowColToGrid(fall1_row, fall1_col)
    Dim fall2_grid = RowColToGrid(fall2_row, fall2_col)
    For 3
        TouchDownEvent(cob1_grid(0), cob1_grid(1), 1)
        TouchDownEvent(cob2_grid(0), cob2_grid(1), 2)
        TouchMoveEvent(fall1_grid(0), fall1_grid(1), 1)
        TouchMoveEvent(fall2_grid(0), fall2_grid(1), 2)
        TouchUpEvent(1)
        TouchUpEvent(2)
    Next
End Sub

Sub PP(row1, col1, row2, col2)
    Dim cob2_index = (cob_index + 1) Mod cob_count
    Dim cob1_grid = cob_list(cob_index)
    Dim cob2_grid = cob_list(cob2_index)
    Call RawPP(cob1_grid(0), cob1_grid(1), cob2_grid(0), cob2_grid(1), row1, col1, row2, col2)
    cob_index = (cob2_index + 1) Mod cob_count
End Sub


' 同时发射三门炮

Sub RawPPS(cob1_position, cob2_position, cob3_position, fall1_row, fall1_col, fall2_row, fall2_col, fall3_row, fall3_col)
    Dim cob1_grid = RowColToGrid(cob1_position(0), cob1_position(1))
    Dim cob2_grid = RowColToGrid(cob2_position(0), cob2_position(1))
    Dim cob3_grid = RowColToGrid(cob3_position(0), cob3_position(1))
    Dim fall1_grid = RowColToGrid(fall1_row, fall1_col)
    Dim fall2_grid = RowColToGrid(fall2_row, fall2_col)
    Dim fall3_grid = RowColToGrid(fall3_row, fall3_col)
    For 3
        TouchDownEvent(cob1_grid(0), cob1_grid(1), 1)
        TouchDownEvent(cob2_grid(0), cob2_grid(1), 2)
        TouchDownEvent(cob3_grid(0), cob3_grid(1), 3)
        TouchMoveEvent(fall1_grid(0), fall1_grid(1), 1)
        TouchMoveEvent(fall2_grid(0), fall2_grid(1), 2)
        TouchMoveEvent(fall3_grid(0), fall3_grid(1), 3)
        TouchUpEvent(1)
        TouchUpEvent(2)
        TouchUpEvent(3)
    Next
End Sub

Sub PPS(row1, col1, row2, col2, row3, col3)
    Dim cob2_index = (cob_index + 1) Mod cob_count
    Dim cob3_index = (cob2_index + 1) Mod cob_count
    Call RawPPS(cob_list(cob_index), cob_list(cob2_index), cob_list(cob3_index), row1, col1, row2, col2, row3, col3)
    cob_index = (cob3_index + 1) Mod cob_count
End Sub


' 同时发射四门炮

Sub RawPPSS(cob1_position, cob2_position, cob3_position, cob4_position, fall1_row, fall1_col, fall2_row, fall2_col)
    Dim cob1_grid = RowColToGrid(cob1_position(0), cob1_position(1))
    Dim cob2_grid = RowColToGrid(cob2_position(0), cob2_position(1))
    Dim cob3_grid = RowColToGrid(cob3_position(0), cob3_position(1))
    Dim cob4_grid = RowColToGrid(cob4_position(0), cob4_position(1))
    Dim fall1_grid = RowColToGrid(fall1_row, fall1_col)
    Dim fall2_grid = RowColToGrid(fall2_row, fall2_col)
    For 3
        TouchDownEvent(cob1_grid(0), cob1_grid(1), 1)
        TouchDownEvent(cob2_grid(0), cob2_grid(1), 2)
        TouchDownEvent(cob3_grid(0), cob3_grid(1), 3)
        TouchDownEvent(cob4_grid(0), cob4_grid(1), 4)
        TouchMoveEvent(fall1_grid(0), fall1_grid(1), 1)
        TouchMoveEvent(fall1_grid(0), fall1_grid(1), 2)
        TouchMoveEvent(fall2_grid(0), fall2_grid(1), 3)
        TouchMoveEvent(fall2_grid(0), fall2_grid(1), 4)
        TouchUpEvent(1)
        TouchUpEvent(2)
        TouchUpEvent(3)
        TouchUpEvent(4)
    Next
End Sub

Sub PPSS(row1, col1, row2, col2)
    Dim cob2_index = (cob_index + 1) Mod cob_count
    Dim cob3_index = (cob2_index + 1) Mod cob_count
    Dim cob4_index = (cob3_index + 1) Mod cob_count
    Call RawPPSS(cob_list(cob_index), cob_list(cob2_index), cob_list(cob3_index), cob_list(cob4_index), row1, col1, row2, col2)
    cob_index = (cob4_index + 1) Mod cob_count
End Sub


' 跳过 n 门炮
Sub SkipCob(n)
    cob_index = (cob_index + n) Mod cob_count
End Sub