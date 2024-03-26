' 阵名: PE 双冰十六炮
' 节奏: ch6: IPP-PP | PPDD | IPP-PP | PPDD (12, 6, 12, 6)

Import "pvz.mql"
Import "Thread.lua"

SetScreenScale 1080, 2400


pvz.UpdateAddr("ABCD1234", "entry")


Dim ICE_SHROOM = 1   ' 冰川菇
Dim M_ICE_SHROOM = 2 ' 模仿者冰川菇
Dim COFFEE_BEAN = 3  ' 咖啡豆
Dim SQUASH = 4       ' 窝瓜
Dim CHERRY_BOMB = 5  ' 樱桃炸弹
Dim LILY_PAD = 6     ' 莲叶
Dim DOOM_SHROOM = 7  ' 末日菇
Dim TALL_NUT = 8     ' 高坚果
Dim BLOVER = 9       ' 三叶草

Dim pao_list = Array(Array(1, 7), Array(6, 7), Array(1, 5), Array(6, 5), _
                     Array(2, 7), Array(5, 7), Array(2, 5), Array(5, 5), _
                     Array(3, 7), Array(4, 7), Array(3, 5), Array(4, 5), _
                     Array(3, 3), Array(4, 3), Array(3, 1), Array(4, 1))
pvz.UpdateCobList(pao_list)


' 存冰
Sub FillIceThread(ice_index, m_ice_index)
    For count = 1 To 4
        pvz.TapSeed(m_ice_index)
        pvz.TapGrid(1, 2)
        pvz.TapGrid(6, 2)
        pvz.TapGrid(2, 1)
        pvz.TapSeed(ice_index)
        pvz.TapGrid(1, 2)
        pvz.TapGrid(6, 2)
        pvz.TapGrid(2, 1)
        If (count < 4) Then
            pvz.Sleep(5200)
        End If
    Next
End Sub

' 点冰
Sub Coffee()
    pvz.TapSeed(COFFEE_BEAN)
    pvz.TapGrid(2, 1)
    pvz.TapGrid(5, 1)
    pvz.TapGrid(1, 2)
    pvz.TapGrid(6, 2)
End Sub


pvz.Sleep(300)
pvz.LetsRock()
pvz.Sleep(400)

For wave = 1 To 20
    If (wave = 1) Then
        pvz.Prejudge(-550, wave)
        Thread.Start(FillIceThread, ICE_SHROOM, M_ICE_SHROOM) ' 启动存冰子线程
        pvz.WaitUntil(-180)
    Else
        pvz.Prejudge(-180, wave)
    End If

    ' PPDD
    If (wave = 1 Or wave = 3 Or wave = 5 Or wave = 7 Or wave = 9 Or _
        wave = 10 Or wave = 12 Or wave = 14 Or wave = 16 Or wave = 18) Then
        pvz.WaitUntil(-95)
        pvz.PP(2, 9, 5, 9)
        pvz.Sleep(106)
        pvz.PP(2, 9, 5, 9)

        ' 第 9 波收尾
        If (wave = 9) Then
            pvz.WaitUntil(601 + 225 - 373)
            pvz.PP(2, 9, 5, 9)
            pvz.WaitUntil(601 + 225 - 298)
            Call Coffee()
            pvz.WaitUntil(601 + 1200 - 200 - 373)
            pvz.PP(2, 9, 5, 9)
        End If

    ' IPP-PP
    ElseIf (wave = 2 Or wave = 4 Or wave = 6 Or wave = 8 Or _
            wave = 11 Or wave = 13 Or wave = 15 Or wave = 17 Or wave = 19) Then
        pvz.WaitUntil(225 - 373)
        If (wave = 19) Then
            pvz.PP(2, 9, 5, 9)
        Else
            pvz.PP(2, 8.4, 5, 8.4)
        End If
        pvz.WaitUntil(225 - 298)
        Call Coffee()
        pvz.WaitUntil(1200 - 200 - 373)
        pvz.PP(2, 9, 5, 9)

        ' 第 19 波收尾
        If (wave = 19) Then
            pvz.WaitUntil(1200 - 95)
            pvz.PP(2, 9, 5, 9)
            pvz.Sleep(106)
            pvz.PP(2, 9, 5, 9)
        End If

    ' 第 20 波
    ElseIf (wave = 20) Then
        pvz.WaitUntil(-75)
        pvz.Card(LILY_PAD, 3, 9)
        pvz.Card(DOOM_SHROOM, 3, 9)
        pvz.Card(COFFEE_BEAN, 3, 9)
        pvz.PPSS(2, 9, 5, 9)
        pvz.Sleep(106)
        pvz.PP(2, 9, 5, 9)
        pvz.Sleep(20)
        pvz.Card(M_ICE_SHROOM, 2, 1)
        pvz.Card(ICE_SHROOM, 5, 1)
    End If

    pvz.Sleep(40)
Next