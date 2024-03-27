' 阵名: FE 无冰瓜十八炮
' 节奏: C9u: PPDD | IPP-PP | PPDD | IPP-PP | PPDD | PPDD | NPP (6, 12, 6, 12, 6, 6, 6)

Import "pvz.mql"
Import "Thread.lua"

SetScreenScale 1080, 2400


pvz.UpdateAddr("ABCD1234", "entry")


Dim ICE_SHROOM = 1   ' 冰川菇
Dim M_ICE_SHROOM = 2 ' 模仿者冰川菇
Dim LILY_PAD = 3     ' 莲叶
Dim DOOM_SHROOM = 4  ' 末日菇
Dim CHERRY_BOMB = 5  ' 樱桃炸弹
Dim PUFF_SHROOM = 6  ' 小喷菇
Dim SUN_SHROOM = 7   ' 阳光菇
Dim BLOVER = 8       ' 三叶草
Dim WALL_NUT = 9     ' 坚果墙

Dim pao_list = Array(Array(3, 1), Array(4, 1), Array(2, 1), Array(5, 1), Array(1, 1), Array(6, 1), _
                     Array(3, 5), Array(4, 5), Array(2, 5), Array(5, 5), Array(1, 5), Array(6, 5), _
                     Array(3, 7), Array(4, 7), Array(2, 7), Array(5, 7), Array(1, 7), Array(6, 7))
pvz.UpdateCobList(pao_list)


' 用冰

Thread.SetShareVar("index", 0)

Sub UseIce()
    Dim index = Thread.GetShareVar("index")
    Dim ICE = 1
    Dim M_ICE = 2
    If index = 0 Then
        pvz.Sleep(320)
        pvz.Card(ICE, 3, 4)
    Else
        pvz.Card(M_ICE, 3, 4)
    End If
    index = (index + 1) Mod 2
    Thread.SetShareVar("index", index)
End Sub


pvz.Sleep(300)
pvz.LetsRock()
pvz.Sleep(400)

For wave = 1 To 20
    If wave = 20 Then
        pvz.Prejudge(-450, wave)
    Else
        pvz.Prejudge(-150, wave)
    End If

    ' PPDD
    If wave = 1 Or wave = 3 Or wave = 5 Or wave = 6 Or wave = 8 Or _
       wave = 10 Or wave = 12 Or wave = 14 Or wave = 15 Or wave = 17 Or wave = 19 Then
        pvz.WaitUntil(-95)
        pvz.PP(2, 9, 5, 9)
        pvz.Sleep(106)
        pvz.PP(2, 9, 5, 9)

        ' 下一波预判冰
        If wave = 1 Or wave = 3 Or wave = 8 Or _
           wave = 10 Or wave = 12 Or wave = 17 Then
            pvz.WaitUntil(601 + 225 - 100 - 320)
            Thread.Start(UseIce)

        ' 第 19 波收尾
        ElseIf wave = 19 Then
            pvz.WaitUntil(601 - 95)
            pvz.PP(2, 9, 5, 9)
            pvz.Sleep(106)
            pvz.PP(2, 9, 5, 9)
            pvz.WaitUntil(601 + 601 - 95)
            pvz.PP(2, 9, 5, 9)
        End If

    ' IPP-PP
    ElseIf wave = 2 Or wave = 4 Or wave = 9 Or _
           wave = 11 Or wave = 13 Or wave = 18 Then
        pvz.WaitUntil(225 - 373)
        If wave = 9 Then
            pvz.PP(2, 9, 5, 9)
        Else
            pvz.PP(2, 8.4, 5, 8.4)
        End If
        pvz.WaitUntil(1200 - 200 - 373)
        pvz.PP(2, 9, 5, 9)

        ' 第 9 波收尾
        If wave = 9 Then
            pvz.WaitUntil(1400 - 95)
            pvz.PP(2, 9, 5, 9)
            pvz.Sleep(220)
            pvz.PP(2, 9, 5, 9)
            pvz.WaitUntil(1400 + 601 - 95)
            pvz.PP(2, 9, 5, 9)
        End If

    ' PPN
    ElseIf wave = 7 Or wave = 16 Then
        pvz.WaitUntil(-95 + 106)
        pvz.PP(2, 9, 5, 9)
        pvz.WaitUntil(-95 + 373 - 100)
        If (wave = 7) Then
            pvz.Card(LILY_PAD, 3, 9)
            pvz.Card(DOOM_SHROOM, 3, 9)
        ElseIf (wave = 16) Then
            pvz.Card(LILY_PAD, 4, 9)
            pvz.Card(DOOM_SHROOM, 4, 9)
        End If

    ' 第 20 波收尾
    ElseIf wave = 20 Then
        pvz.WaitUntil(50 - 100 - 320)
        Thread.Start(UseIce)
        pvz.WaitUntil(225 - 373)
        pvz.PP(2, 9, 5, 9)
        pvz.WaitUntil(1400 - 373)
        pvz.PPSS(2, 9, 5, 9)
        pvz.Sleep(220)
        pvz.PP(2, 9, 5, 9)
        pvz.WaitUntil(1400 - 95)
        pvz.PP(2, 9, 5, 9)
    End If

    pvz.Sleep(20)
Next